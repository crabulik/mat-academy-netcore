using System;
using System.Collections.Generic;
using MatOrderingService.Domain;

namespace MatOrderingService.Services.Storage.Impl
{
    class OrdersList : IOrdersList
    {
        private List<Order> _ordersList;

        public OrdersList()
        {
            _ordersList = new List<Order>
            {
                new Order
                {
                    CreateDate = DateTime.Now,
                    CreatorId = "Budy@mail.com",
                    Id = 1,
                    IsDeleted = false,
                    OrderDetails = "2 statues of Sonic",
                    Status = OrderStatus.Promoted
                },
                new Order
                {
                    CreateDate = DateTime.Now,
                    CreatorId = "Budy@mail.com",
                    Id = 2,
                    IsDeleted = false,
                    OrderDetails = "A cup",
                    Status = OrderStatus.Promoted
                }
            };
        }

        public IList<Order> GetAllOrders() => _ordersList;
    }
}
