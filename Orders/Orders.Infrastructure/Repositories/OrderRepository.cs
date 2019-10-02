using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orders.Domain;
using Orders.Domain.Aggregates.Order;

namespace Orders.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public OrderRepository(OrdersContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            var added = await _context.Orders.AddAsync(order);
            return added.Entity;
        }

        public async Task<Order> GetAsync(long orderId)
        {
            var order = await _context.Orders.FromSqlInterpolated($"SELECT * FROM public.orders WHERE \"Id\"={orderId} FOR UPDATE")
                .SingleAsync().ConfigureAwait(false);
            return order;
        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }
    }
}