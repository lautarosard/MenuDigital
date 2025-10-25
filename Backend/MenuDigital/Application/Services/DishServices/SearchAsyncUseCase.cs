using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.Repository;
using Application.Models.Response;
using Application.Models.Response.Dish;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DishServices
{
    public class SearchAsyncUseCase : ISearchAsyncUseCase
    {
        private readonly IDishCommand _dishCommand;
        private readonly IDishQuery _dishQuery;
        private readonly ICategoryQuery _categoryQuery;
        private readonly ICategoryCommand _categoryCommand;
        public SearchAsyncUseCase(
            ICategoryCommand categoryCommand,
            ICategoryQuery categoryQuery,
            IDishCommand dishCommand,
            IDishQuery dishQuery
            )
        {
            _dishCommand = dishCommand;
            _dishQuery = dishQuery;
            _categoryQuery = categoryQuery;
            _categoryCommand = categoryCommand;
        }

        public async Task<IEnumerable<DishResponse?>> SearchAsync(string? name, int? categoryId, OrderPrice? priceOrder = OrderPrice.ASC, bool? onlyActive = null)
        {
            if (categoryId != 0 && categoryId != null)
            {
                var categoryExists = await _categoryQuery.CategoryExistAsync(categoryId.Value);
                if (!categoryExists)
                {
                    throw new NotFoundException($"Category with ID {categoryId} not found.");
                }
            }

            var list = await _dishQuery.GetAllAsync(name, categoryId, priceOrder, onlyActive);

            return list.Select(dishes => new DishResponse
            {
                Id = dishes.DishId,
                Name = dishes.Name,
                Description = dishes.Description,
                Price = dishes.Price,
                Category = new GenericResponse {Id = dishes.Category, Name = dishes.CategoryEnt?.Name },
                isActive = dishes.Available,
                ImageUrl = dishes.ImageUrl,
                createdAt = dishes.CreateDate,
                updateAt = dishes.UpdateDate
            }).ToList();
        }
    }
}
