using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IOrder.Repository;
using Infrastructure.Data;


namespace Infrastructure.Command
{
    public class OrderCommand : IOrderCommand
    {
        private readonly MenuDigitalDbContext _context;
        public OrderCommand(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task InsertOrder(Domain.Entities.Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

        }
        public Task RemoveOrder(Domain.Entities.Order order)
        {
            _context.Orders.Remove(order);
            return Task.CompletedTask;
        }
        public Task UpdateOrder(Domain.Entities.Order order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }
    }
}
