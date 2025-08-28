using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Domain.Entities;
using Application.Interfaces.ICategory;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly MenuDigitalDbContext _context;
        public CategoryQuery(MenuDigitalDbContext context)
        {
            _context = context;
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
