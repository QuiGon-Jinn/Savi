using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SaviWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Globalization;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Register endpoint explorer (useful for OpenAPI/Swagger generation)
builder.Services.AddEndpointsApiExplorer();

// JWT settings
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? "not-secure-key-change-me";
var jwtIssuer = jwtSection["Issuer"] ?? "Savi";
var jwtAudience = jwtSection["Audience"] ?? "SaviApi";
var jwtExpiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var m) ? m : 60;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole(UserRole.Admin.ToString()));
    options.AddPolicy("PracticeManager", p => p.RequireRole(UserRole.PracticeManager.ToString(), UserRole.Admin.ToString()));
    options.AddPolicy("Clinician", p => p.RequireRole(UserRole.Clinician.ToString(), UserRole.Admin.ToString()));
});

#region DB Context
var conString = builder.Configuration.GetConnectionString("SqlDatabase") ??
    throw new InvalidOperationException("Connection string 'SqlDatabase' not found.");

builder.Services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(conString));
#endregion DB Context

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName);
    options.UseInlineDefinitionsForEnums();
    // Add JWT Bearer and OAuth2 (password) support so Swagger UI can obtain tokens from /auth/token
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Description = $"Use /auth/token and paste the token here",
        Name = "Authorization",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    //options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.OpenApiSecurityScheme
    //{
    //    Type = Microsoft.OpenApi.SecuritySchemeType.OAuth2,
    //    Flows = new Microsoft.OpenApi.OpenApiOAuthFlows
    //    {
    //        Password = new Microsoft.OpenApi.OpenApiOAuthFlow
    //        {
    //            TokenUrl = new Uri("/auth/token", UriKind.Relative),
    //            Scopes = new Dictionary<string, string>()
    //        }
    //    }
    //});

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
        //[new OpenApiSecuritySchemeReference("oauth2", document)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Savi API V1");
        // Swagger will use the Bearer definition; use the Authorize button to paste the token.
    });
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Minimal endpoint to insert a Shift
app.MapPost("/shifts", async (ShiftInput input, SqlDbContext db, ClaimsPrincipal user) =>
{
    if (input == null)
        return Results.BadRequest();

    // determine authenticated user
    var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var creatorId))
        return Results.Unauthorized();

    var creator = await db.Users.FindAsync(creatorId);
    if (creator == null)
        return Results.Unauthorized();

    // If the creator is a PracticeManager they can only create shifts for their own practice
    if (creator.Role == UserRole.PracticeManager && creator.Practice.HasValue && creator.Practice.Value != input.Practice)
    {
        return Results.Forbid();
    }

    var shift = new Shift
    {
        Id = Guid.NewGuid(),
        Practice = input.Practice,
        Date = input.Date,
        StartTime = input.StartTime,
        EndTime = input.EndTime,
        HourlyRate = input.HourlyRate,
        Role = input.Role,
        Location = input.Location ?? string.Empty,
        Completed = false
    };

    db.Shifts.Add(shift);
    await db.SaveChangesAsync();

    return Results.Created($"/shifts/{shift.Id}", shift);
}).RequireAuthorization("PracticeManager");

// Minimal endpoint to insert a Timesheet
app.MapPost("/timesheets", async (TimesheetInput input, SqlDbContext db, ClaimsPrincipal user) =>
{
    if (input == null)
        return Results.BadRequest();

    var shift = await db.Shifts.FirstOrDefaultAsync(s => s.Id == input.Shift);
    if (shift == null)
        return Results.NotFound("Shift not found");

    // determine the authenticated user id from token
    var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        return Results.Unauthorized();

    var dbUser = await db.Users.FindAsync(userId);
    if (dbUser == null)
        return Results.Unauthorized();

    if (shift.Completed)
    {
        var existingTimesheet = db.Timesheets.FirstOrDefault(t => t.ShiftId == shift.Id);
        if (existingTimesheet != null)
        {
            existingTimesheet.BusinessReference = (string.IsNullOrWhiteSpace(existingTimesheet.BusinessReference) ? "" : "\n") 
                + $"Duplicate timesheet ignored: {dbUser.Username}, {input.StartTime} - {input.EndTime}, break: {input.UnpaidBreakMinutes}";
            await db.SaveChangesAsync();
        }
        return Results.BadRequest("Shift already completed");
    }    

    // mark shift as completed
    shift.Completed = true;

    var timesheet = new Timesheet
    {
        Id = Guid.NewGuid(),
        StartTime = input.StartTime,
        EndTime = input.EndTime,
        UnpaidBreakMinutes = input.UnpaidBreakMinutes,
        Notes = input.Notes,
        ShiftId = shift.Id,
        UserId = userId
    };

    db.Timesheets.Add(timesheet);
    await db.SaveChangesAsync();

    return Results.Created($"/timesheets/{timesheet.Id}", timesheet);
}).RequireAuthorization("Clinician");

// Payment run - aggregates timesheets for a practice and date range
app.MapPost("/paymentrun", async (PaymentRunInput input, SqlDbContext db, ClaimsPrincipal user) =>
{
    if (input == null)
        return Results.BadRequest();

    // determine authenticated user
    var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var creatorId))
        return Results.Unauthorized();

    var creator = await db.Users.FindAsync(creatorId);
    if (creator == null)
        return Results.Unauthorized();

    // If the creator is a PracticeManager they can only create shifts for their own practice
    if (creator.Role == UserRole.PracticeManager && creator.Practice.HasValue && creator.Practice.Value != input.Practice)
    {
        return Results.Forbid();
    }

    // Load timesheets with their shifts and users that match the practice and date range
    var timesheets = await db.Timesheets
        .Include(t => t.Shift)
        .Include(t => t.User)
        .Where(t => t.Shift != null && t.Shift.Practice == input.Practice && t.Shift.Date >= input.StartDate && t.Shift.Date <= input.EndDate)
        .ToListAsync();

    var gb = new CultureInfo("en-GB");

    decimal totalHours = 0m;
    decimal totalFees = 0m;

    var lineItems = timesheets.Select(t =>
    {
        var shift = t.Shift!;
        // calculate raw minutes
        var duration = t.EndTime.ToTimeSpan() - t.StartTime.ToTimeSpan();
        var minutes = (decimal)duration.TotalMinutes - (t.UnpaidBreakMinutes ?? 0);
        if (minutes < 0) minutes = 0;

        var hours = minutes / 60m;        

        var rate = Convert.ToDecimal(shift.HourlyRate);
        var fees = hours * rate;

        totalHours += hours;
        totalFees += fees;

        return new
        {
            Clinician = t.User?.Username ?? "",
            HoursWorked = Math.Round(hours, 2, MidpointRounding.AwayFromZero),
            HourlyRate = rate.ToString("C", gb),
            Fees = Math.Round(fees, 2, MidpointRounding.AwayFromZero).ToString("C", gb)
        };
    }).ToList();

    // Round totals similarly
    totalHours = Math.Round(totalHours, 2, MidpointRounding.AwayFromZero);
    totalFees = Math.Round(totalFees, 2, MidpointRounding.AwayFromZero);

    var result = new
    {
        Practice = input.Practice,
        Period = new { From = input.StartDate, To = input.EndDate },
        TotalHoursWorked = totalHours,
        TotalFees = totalFees.ToString("C", gb),
        LineItems = lineItems
    };

    return Results.Ok(result);
}).RequireAuthorization("PracticeManager");

// Register user (open for testing) - creates a user with hashed password
app.MapPost("/auth/register", async (RegisterRequest reg, SqlDbContext db) =>
{
    if (await db.Users.AnyAsync(u => u.Username == reg.Username))
        return Results.Conflict("User already exists");

    var user = new User
    {
        Id = Guid.NewGuid(),
        Username = reg.Username,
        Role = reg.Role,
        Practice = reg.Practice
    };
    user.SetPassword(reg.Password);

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Role });
});

app.MapPost("/auth/token", async (AuthRequest req, SqlDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
    if (user == null)
        return Results.Unauthorized();

    if (!user.VerifyPassword(req.Password))
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var expires = DateTime.UtcNow.AddMinutes(jwtExpiresMinutes);

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: expires,
        signingCredentials: creds);

    var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new AuthResponse { Token = tokenStr, Expires = expires });
});


app.Run();





