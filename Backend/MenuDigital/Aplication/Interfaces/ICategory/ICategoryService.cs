using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Models.Request;
using Application.Models.Response;

namespace Application.Interfaces.ICategory
{
    public interface ICategoryService
    {
        // Methods
        // List
        Task<List<CategoryResponse>> GetAllCategories();

        // Create
        Task<CategoryResponse> CreateCategory(CategoryRequest categoria);

        // Update
        Task<CategoryResponse> UpdateCategory(int id, CategoryRequest categoria);

        // Delete
        Task<CategoryResponse> DeleteCategory(int id);

        // Queries
        Task<CategoryResponse> GetCategoryById(int id);
    }
}
