using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish.Repository;
using Application.Interfaces.UnitOfWork;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MenuDigitalDbContext _context;

        public IDishRepository DishesRepository { get; }
        public ICategoryRepository CategoryRepository { get; }

        public UnitOfWork(MenuDigitalDbContext context,
                          IDishRepository dishRepository,
                          ICategoryRepository categoryRepository)
        {
            _context = context;
            DishesRepository = dishRepository;
            CategoryRepository = categoryRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
