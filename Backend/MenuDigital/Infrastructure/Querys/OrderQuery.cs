using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IOrder.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Querys
{
    public class OrderQuery : IOrderQuery
    {
        private readonly MenuDigitalDbContext _context;
        public OrderQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task<Order?> GetOrderById(long id)
        {
            return await _context.Orders
                        .Include(o => o.OverallStatus)
                        .Include(o => o.DeliveryType)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Dish)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Status)
                        .FirstOrDefaultAsync(o => o.OrderId == id);

        }
        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }
        public async Task<IEnumerable<Order?>> GetOrderWithFilter(int? statusId, DateTime? from, DateTime? to)
        {
            var query = _context.Orders
                .Include(o => o.OverallStatus)
                .Include(o => o.DeliveryType)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems) 
                .ThenInclude(oi => oi.Status)
            .AsNoTracking().AsQueryable();
            if (statusId.HasValue)
            {
                query = query.Where(o => o.StatusId == statusId.Value);
            }
            if (from.HasValue)
            {
                query = query.Where(o => o.CreateDate >= from.Value);
            }
            if (to.HasValue)
            {
                query = query.Where(o => o.CreateDate <= to.Value);
            }
            return await query.ToListAsync();
        }
    }
}
