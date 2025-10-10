using Application.Enums;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.Repository;
using Application.Models.Response;
using Application.Models.Response.Dish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DishServices
{
    public class SearchAsyncUseCase : ISearchAsyncUseCase
    {
        private readonly IDishRepository _dishRepository;
        public SearchAsyncUseCase(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public async Task<IEnumerable<DishResponse?>> SearchAsync(string? name, int? categoryId, OrderPrice? priceOrder = OrderPrice.ASC, bool? onlyActive = null)
        {

            var list = await _dishRepository.GetAllAsync(name, categoryId, priceOrder, onlyActive);



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
