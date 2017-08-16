using System;
using System.Threading.Tasks;

namespace MatOrderingService.Services.Orders
{
    public interface IOrderingService
    {
        Task<OrderInfo[]> GetAll();

        Task<OrderInfo> Get(int id);

        Task<OrderInfo> Create(NewOrder order);

        Task<OrderInfo> Update(int id, EditOrder value);

        Task<bool> Delete(int id);
    }

    public class EditOrder
    {
        public string OrderDetails { get; set; }
    }

    public class NewOrder
    {
        public string OrderDetails { get; set; }

        public string CreatorId { get; set; }
    }

    public class OrderInfo
    {
        public int Id { get; set; }

        public string OrderDetails { get; set; }

        public String Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatorId { get; set; }
    }
}