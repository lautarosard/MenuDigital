using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DishService : IDishService
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;
        public DishService(IDishCommand command, IDishQuery query)
        {
            _command = command;
            _query = query;
        }
        public async Task<DishResponse?> CreateDish(DishRequest dishRequest)
        {
            //validaciones
            var existingDish = await _query.DishExists(dishRequest.Name);
            
            if (existingDish)
            {
                return null;
            }
            var category = await _categoryQuery.GetCategoryById(dishRequest.CategoryId);
            var dish = new Dish
            {
                DishId = Guid.NewGuid(),
                Name = dishRequest.Name,
                Description = dishRequest.Description,
                Price = dishRequest.Price,
                Available = dishRequest.Available,
                ImageUrl = dishRequest.ImageUrl,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                CategoryId = dishRequest.CategoryId
            };
            await _command.InsertDish(dish);
            return new DishResponse
            {
                Id = dish.DishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = new GenericResponse { Id = category.Id, Name = category.Name },
                Available = dish.Available,
                ImageUrl = dish.ImageUrl,
                CreateDate = dish.CreateDate,
                UpdateDate = dish.UpdateDate
            };
        }

        public Task<DishResponse> DeleteDish(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DishResponse>> GetAllDishesAsync()
        {
            var dishes = await _query.GetAllDishes();

            return dishes.Select(dishes => new DishResponse
            {

                Id = dishes.DishId,
                Name = dishes.Name,
                Description = dishes.Description,
                Price = dishes.Price,
                Category = new GenericResponse { Id = dishes.CategoryId, Name = dishes.Category?.Name},
                Available = dishes.Available,
                ImageUrl = dishes.ImageUrl,
                CreateDate = dishes.CreateDate,
                UpdateDate = dishes.UpdateDate
            }).ToList();
        }

        public async Task<DishResponse> GetDishById(Guid id)
        {
            var dish = await _query.GetDishById(id);
            if (dish == null)
            {
                throw new Exception("Dish not found");
            }

            return new DishResponse
            {
                Id = dish.DishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = new GenericResponse { Id = dish.CategoryId, Name = dish.Category?.Name },
                Available = dish.Available,
                ImageUrl = dish.ImageUrl,
                CreateDate = dish.CreateDate,
                UpdateDate = dish.UpdateDate
            };
        }

        public async Task<IEnumerable<DishResponse?>> SearchAsync(string? name, int? categoryId, string? priceOrder)
        {
            
            var list = await _query.GetAllAsync(name, categoryId, priceOrder);
            return list.Select(dishes => new DishResponse
            {
                Id = dishes.DishId,
                Name = dishes.Name,
                Description = dishes.Description,
                Price = dishes.Price,
                Category = new GenericResponse { Id = dishes.CategoryId, Name = dishes.Category?.Name },
                Available = dishes.Available,
                ImageUrl = dishes.ImageUrl,
                CreateDate = dishes.CreateDate,
                UpdateDate = dishes.UpdateDate
            }).ToList();
        }

        public async Task<DishResponse> UpdateDish(Guid id, DishRequest dishRequest)
        {
            var existingDish = await _query.GetDishById(id);

            if (existingDish == null)
            {//que retorne null si no encuantre
                throw new Exception("Dish not found");
            }
            var alreadyExist = _query.DishExists(dishRequest.Name);
            if (alreadyExist == null)
            {//buscar tirar la exception al controller
                throw new Exception();
            }

            existingDish.Name = dishRequest.Name;
            existingDish.Description = dishRequest.Description;
            existingDish.Price = dishRequest.Price;
            existingDish.Available = dishRequest.Available;
            existingDish.ImageUrl = dishRequest.ImageUrl;
            existingDish.UpdateDate = DateTime.UtcNow;

            await _command.UpdateDish(existingDish);

            return new DishResponse
            {
                Id = existingDish.DishId,
                Name = existingDish.Name,
                Description = existingDish.Description,
                Price = existingDish.Price,
                Available = existingDish.Available,
                ImageUrl = existingDish.ImageUrl,
                CreateDate = existingDish.CreateDate,
                UpdateDate = existingDish.UpdateDate
            };
        }
    }
}
