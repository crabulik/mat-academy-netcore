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
                    OrderItems = new[] {
                        new OrderItem{Value = "A statue of Sonic"},
                        new OrderItem{Value = "A statue of Sonic"}
                    },
                    Status = OrderStatus.Promoted
                },
                new Order
                {
                    CreateDate = DateTime.Now,
                    CreatorId = "Budy@mail.com",
                    Id = 2,
                    IsDeleted = false,
                    OrderItems = new[] {
                        new OrderItem{Value = "A cup"},
                    },
                    Status = OrderStatus.Promoted
                }
            };
        }

        public IList<Order> GetAllOrders() => _ordersList;
    }
}
