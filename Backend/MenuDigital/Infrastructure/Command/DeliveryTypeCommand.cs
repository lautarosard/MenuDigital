using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IDeliveryType.Repository;
using Domain.Entities;
using Infrastructure.Data;


namespace Infrastructure.Command
{
    public class DeliveryTypeCommand : IDeliveryTypeCommand
    {
        private readonly MenuDigitalDbContext _context;
        public DeliveryTypeCommand(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public Task InsertDeliveryType(DeliveryType deliveryType)
        {
            _context.DeliveryTypes.Add(deliveryType);
            return Task.CompletedTask;
        }

        public Task UpdateDeliveryType(DeliveryType deliveryType)
        {
            _context.DeliveryTypes.Update(deliveryType);
            return Task.CompletedTask;
        }

        public Task RemoveDeliveryType(DeliveryType deliveryType)
        {
            _context.DeliveryTypes.Remove(deliveryType);
            return Task.CompletedTask;
        }

    }
}
