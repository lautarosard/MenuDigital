using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.IDish;

namespace Infrastructure.Querys
{
    public class DishQuery : IDishQuery
    {
        private readonly MenuDigitalDbContext _context;
        public DishQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task<List<Dish>> GetAllDishes()
        {
            return await _context.Dishes.ToListAsync();
        }
        public async Task<Dish?> GetDishById(Guid id)
        {
            return await _context.Dishes.FindAsync(id).AsTask();
        }

    }
}
