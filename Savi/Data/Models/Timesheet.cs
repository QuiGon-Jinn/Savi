using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Data.Models
{
    public class Timesheet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        public int? UnpaidBreakMinutes { get; set; }

        public string? Notes { get; set; }

        public string? BusinessReference { get; set; }

        // Reference to Shift
        [Required]
        public Guid ShiftId { get; set; }

        [ForeignKey(nameof(ShiftId))]
        public Shift? Shift { get; set; }

        // Reference to the user who created/submitted the timesheet
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
