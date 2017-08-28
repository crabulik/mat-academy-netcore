using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Models
{
    public class NewOrder
    {
        [Required]
        public OrderItemValue[] OrderItems { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatorId { get; set; }
    }
}
