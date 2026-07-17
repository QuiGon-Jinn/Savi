using System.ComponentModel.DataAnnotations;

namespace SaviWebApi.Models
{
    public class PaymentRunInput
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public Data.Models.Practice Practice { get; set; }
    }
}
