using Application.Interfaces.IDish;
using Application.Models.Response;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Querys
{
    public class DishQuery : IDishQuery
    {
        private readonly MenuDigitalDbContext _context;
        public DishQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllAsync(string? name = null, int? categoryId = null, string? priceOrder = null)
        {
            var query = _context.Dishes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(d => d.Name.Contains(name));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(priceOrder))
            {
                var normalized = priceOrder.Trim().ToUpperInvariant();
                if (normalized == "ASC")
                {
                    query = query.OrderBy(d => d.Price);
                }
                else if (normalized == "DESC")
                {
                    query = query.OrderByDescending(d => d.Price);
                }
            }

            return await query.ToListAsync();

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
