using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Models
{
    public class OrderInfo
    {
        public int Id { get; set; }

        public OrderItemValue[] OrderItems { get; set; }

        public String Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatorId { get; set; }
    }
}
