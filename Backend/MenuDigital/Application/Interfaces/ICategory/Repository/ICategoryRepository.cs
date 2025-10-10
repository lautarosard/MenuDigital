using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ICategory.Repository
{
    public interface ICategoryRepository
    {
        //Queries
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
        Task<bool> CategoryExistAsync(int id);
        
    }
}
