using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.ICategory;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Command
{
    public class CategoryCommand : ICategoryCommand
    {
        private readonly MenuDigitalDbContext _context;
        public CategoryCommand(MenuDigitalDbContext context)
        {
            _context = context;
        }
        public async Task InsertCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
