using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IDish;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Command
{
    public class DishCommand : IDishCommand
    {
        private readonly MenuDigitalDbContext _context;
        public DishCommand(MenuDigitalDbContext context)
        {
            _context = context;
        }

        public async Task InsertDish(Dish dish)
        {
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDish(Dish dish)
        {
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDish(Dish dish)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
        }
    }
}
