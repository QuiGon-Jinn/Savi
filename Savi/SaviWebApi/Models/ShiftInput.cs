using Data.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SaviWebApi.Models
{
    public class ShiftInput
    {   
        public Practice Practice { get; set; } = Practice.Smile;

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [DefaultValue("9:00")]
        public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0);

        [DefaultValue("17:00")]
        public TimeOnly EndTime { get; set; } = new TimeOnly(17, 0);

        public double HourlyRate { get; set; }

        [DefaultValue(nameof(UserRole.Clinician))]
        public UserRole Role { get; set; } = UserRole.Clinician;

        public string Location { get; set; } = string.Empty;
    }
}
