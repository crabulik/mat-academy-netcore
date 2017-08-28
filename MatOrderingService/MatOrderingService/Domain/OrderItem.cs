using System;

namespace MatOrderingService.Domain
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Value { get; set; }

        #region Navigation Properties
        public Order Order { get; set; }
        
        #endregion
    }
}
