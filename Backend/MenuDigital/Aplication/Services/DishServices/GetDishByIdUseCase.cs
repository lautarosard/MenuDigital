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
    public class GetDishByIdUseCase : IGetDishByIdUseCase
    {
        private readonly IDishRepository _dishRepository;
        public GetDishByIdUseCase(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }
        public async Task<DishResponse?> GetDishById(Guid id)
        {
            var dish = await _dishRepository.GetDishById(id);
            if (dish == null)
            {
                return null;
            }

            return new DishResponse
            {
                Id = dish.DishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = new GenericResponse { Id = dish.Category, Name = dish.CategoryEnt?.Name },
                isActive = dish.Available,
                ImageUrl = dish.ImageUrl,
                createdAt = dish.CreateDate,
                updateAt = dish.UpdateDate
            };
        }
    }
}
