using MatOrderingService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Services.Storage
{
    public interface IOrdersList
    {
        IList<Order> GetAllOrders();
    }
}
