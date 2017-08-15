using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Domain
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderDetails { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatorId { get; set; }

        public bool IsDeleted { get; set; }
    }

    public enum OrderStatus
    {
        New = 1,
        Promoted = 2
    }
}
