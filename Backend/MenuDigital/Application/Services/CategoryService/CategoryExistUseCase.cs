using Application.Interfaces.ICategory;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CategoryService
{
    public class CategoryExistUseCase : ICategoryExistUseCase
    {
        private readonly ICategoryQuery _categoryQuery;
        private readonly ICategoryCommand _categoryCommand;
        public CategoryExistUseCase(ICategoryQuery categoryQuery, ICategoryCommand categoryCommand)
        {
            _categoryQuery = categoryQuery;
            _categoryCommand = categoryCommand;
        }
        public async Task<bool> CategoryExist(int id)
        {
            var category = await _categoryQuery.CategoryExistAsync(id);
            return category;
        }
    }
}
