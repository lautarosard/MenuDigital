using Application.Enums;
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

        public async Task<IEnumerable<Dish>> GetAllAsync(string? name = null, int? categoryId = null, OrderPrice? priceOrder = OrderPrice.ASC, bool? onlyActive = null)
        {
            var query = _context.Dishes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(d => d.Name.Contains(name));
            }

            if (categoryId >= 1 && categoryId <= 10)
            {
                query = query.Where(d => d.Category == categoryId.Value);
            }
            
            switch(priceOrder)
            {
                case OrderPrice.ASC:
                    query = query.OrderBy(d => d.Price);
                    break;
                case OrderPrice.DESC:
                    query = query.OrderByDescending(d => d.Price);
                    break;
                default:
                    throw new InvalidOperationException("Valor de ordenamiento inválido");
                    
            }
            
            switch(onlyActive)
            {
                case true:
                    query = query.Where(d => d.Available == true);
                    break;
                default:
                    break;
            }
            
            return await query
            .Include(d => d.CategoryEnt)
            .ToListAsync();

        }

        public async Task<List<Dish>> GetAllDishes()
        {
            return await _context.Dishes.ToListAsync();
        }
        public async Task<Dish?> GetDishById(Guid id)
        {
            return await _context.Dishes.FindAsync(id).AsTask();
        }

        public async Task<bool> DishExists(string name, Guid? id)
        {
            var query = _context.Dishes.AsQueryable();

            if (id.HasValue)
            {
                // Si estamos actualizando, excluimos el ID actual de la búsqueda
                query = query.Where(d => d.DishId != id.Value);
            }

            // Ahora la búsqueda de conflicto solo se hará en los OTROS platos
            return await query.AnyAsync(d => d.Name == name);

        }
    }
}
