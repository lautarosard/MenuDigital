using Application.Interfaces.ICategory;
using Application.Interfaces.ICategory.Repository;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CategoryService
{
    public class GetAllCategoryAsyncUseCase : IGetAllCategoryAsyncUseCase
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetAllCategoryAsyncUseCase(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllCategories();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();
        }
    }
}
