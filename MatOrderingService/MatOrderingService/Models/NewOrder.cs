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
        public string OrderDetails { get; set; }

        [Required]
        public string CreatorId { get; set; }
    }
}
