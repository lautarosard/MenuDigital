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
        private readonly ICategoryCommand _command;
        private readonly ICategoryQuery _query;
        public GetAllCategoryAsyncUseCase(ICategoryCommand command, ICategoryQuery query)
        {
            _command = command;
            _query = query;
        }
        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _query.GetAllCategories();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();
        }
    }
}
