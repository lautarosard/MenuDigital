using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IOrderItem.Repository;
using Domain.Entities;
using Infrastructure.Data;


namespace Infrastructure.Command
{
    public class OrderItemCommand : IOrderItemCommand
    {
        private readonly MenuDigitalDbContext _context;
        public OrderItemCommand(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task InsertOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
        }
        public async Task InsertOrderItemRange(List<OrderItem> orderItems)
        {
            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveOrderItem(IEnumerable<OrderItem> orderItem)
        {
            _context.OrderItems.RemoveRange(orderItem);
            await _context.SaveChangesAsync();
            
        }

        public Task UpdateOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            return Task.CompletedTask;
        }
    }
}
