using System.ComponentModel.DataAnnotations;

namespace MatOrderingService.Models
{
    public class OrderItemValue
    {
        [Required]
        [StringLength(200)]
        public string Value { get; set; }
    }
}