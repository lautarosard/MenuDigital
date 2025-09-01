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
        public DishService(IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
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

        public async Task<DishResponse?> GetDishById(Guid id)
        {
            var dish = await _query.GetDishById(id);
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

        public async Task<UpdateDishResult> UpdateDish(Guid id, DishUpdateRequest DishUpdateRequest)
        {
            var existingDish = await _query.GetDishById(id);

            if (existingDish == null)
            {//que retorne null si no encuantre
                return new UpdateDishResult { NotFound = true };
            }
            var alreadyExist = _query.DishExists(DishUpdateRequest.Name);
            if (alreadyExist == null)
            {//buscar tirar la exception al controller
                return new UpdateDishResult { NameConflict = true };
            }

            existingDish.Name = DishUpdateRequest.Name;
            existingDish.Description = DishUpdateRequest.Description;
            existingDish.Price = DishUpdateRequest.Price;
            existingDish.Available = DishUpdateRequest.IsActive;
            existingDish.ImageUrl = DishUpdateRequest.Image;
            existingDish.UpdateDate = DateTime.UtcNow;

            await _command.UpdateDish(existingDish);

            return new UpdateDishResult
            { 
                Success = true,
                UpdatedDish = existingDish
            };
        }
    }
}
