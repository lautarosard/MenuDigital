using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.ICategory.Repository;

namespace Infrastructure.Querys
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly MenuDigitalDbContext _context;
        public CategoryQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExistAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _context.Categories.FindAsync(id).AsTask();
        }
    }
}
