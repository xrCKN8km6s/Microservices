using System.Threading.Tasks;

namespace Orders.Domain.Aggregates.Order
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> AddAsync(Order order);

        Task<Order> GetAsync(long orderId);

        void Update(Order order);
    }
}