using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Shift
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Practice Practice { get; set; }

        [Required] 
        public DateOnly Date { get; set; }

        [Required] 
        public TimeOnly StartTime { get; set; }

        [Required] 
        public TimeOnly EndTime { get; set; }

        [Required] 
        public double HourlyRate { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;        

        public bool Completed { get; set; } = false;
    }
}
