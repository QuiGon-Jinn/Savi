using System.ComponentModel.DataAnnotations;
using Data.Models;

namespace SaviWebApi.Models
{
    public class AuthRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Clinician;

        // Optional practice association for the new user
        public Practice? Practice { get; set; } = Data.Models.Practice.None;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
    }
}
