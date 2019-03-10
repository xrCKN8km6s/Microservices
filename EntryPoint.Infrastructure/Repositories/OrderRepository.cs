﻿using System.Threading.Tasks;
using EntryPoint.Domain;
using EntryPoint.Domain.Aggregates.Order;
using Microsoft.EntityFrameworkCore;

namespace EntryPoint.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MicroserviceContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public OrderRepository(MicroserviceContext context)
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
            var order = await _context.Orders.FromSql("SELECT * FROM public.orders WHERE \"Id\"={0} FOR UPDATE", orderId)
                .SingleAsync();
            return order;
        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }
    }
}