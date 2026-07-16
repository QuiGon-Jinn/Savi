using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SaviWebApi.Models
{
    public class TimesheetInput
    {    
        [Required, DefaultValue("9:00")]
        public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0);

        [Required, DefaultValue("17:00")]
        public TimeOnly EndTime { get; set; } = new TimeOnly(17, 0);

        [DefaultValue(0)]
        public int UnpaidBreakMinutes { get; set; } = 0;

        [DefaultValue("")]
        public string? Notes { get; set; } = string.Empty;

        // External GUID of the Shift (maps to Shift.ExternalId)
        [Required]
        public Guid Shift { get; set; }
    }
}
