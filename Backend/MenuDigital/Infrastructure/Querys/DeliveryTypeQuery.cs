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
    public class DeliveryTypeQuery
    {
        private readonly MenuDigitalDbContext _context;
        public DeliveryTypeQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task<DeliveryType?> GetDeliveryTypeById(int id)
        {
            return await _context.DeliveryTypes.FindAsync(id).AsTask();
        }
        public async Task<List<DeliveryType>> GetAllDeliveryTypes()
        {
            return await _context.DeliveryTypes.ToListAsync();
        }
    }
}
