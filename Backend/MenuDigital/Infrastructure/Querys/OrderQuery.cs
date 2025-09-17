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
            return await _context.Orders.FindAsync(id).AsTask();

        }
        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }
    }
}
