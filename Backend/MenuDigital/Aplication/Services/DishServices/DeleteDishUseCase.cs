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
    public class DeleteDishUseCase : IDeleteDishUseCase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDishRepository _dishRepository;
        
        public DeleteDishUseCase(ICategoryRepository categoryRepository, IDishRepository dishRepository)
        {
            _categoryRepository = categoryRepository;
            _dishRepository = dishRepository;
        }

        public async Task<DishResponse?> DeleteDish(Guid id)
        {
            var dish = await _dishRepository.GetDishById(id);
            if (dish == null)
            {
                return null;
            }
            await _dishRepository.RemoveDish(dish);
            return new DishResponse
            {
                Id = id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = new GenericResponse { Id = dish.Category, Name = dish.CategoryEnt.Name },
                isActive = dish.Available,
                ImageUrl = dish.ImageUrl,
                createdAt = dish.CreateDate,
                updateAt = dish.UpdateDate

            };
        }
    }
}
