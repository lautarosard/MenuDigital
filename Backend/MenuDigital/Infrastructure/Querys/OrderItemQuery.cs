using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class OrderItemQuery
    {
        private readonly MenuDigitalDbContext _context;
        public OrderItemQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task<OrderItem?> GetOrderItemById(int id)
        {
            return await _context.OrderItems.FindAsync(id).AsTask();
        }
        public async Task<List<OrderItem>> GetAllOrderItems()
        {
            return await _context.OrderItems.ToListAsync();
        }
    }
}
