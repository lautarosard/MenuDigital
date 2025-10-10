using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDishRepository DishesRepository { get; }
        
        ICategoryRepository CategoryRepository { get; }
        Task SaveChangesAsync();

    }
}
