using Application.Exceptions;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.Repository;
using Application.Models.Request;
using Application.Models.Response;
using Application.Models.Response.Dish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DishServices
{
    public class UpdateDishUseCase : IUpdateDishUseCase
    {
        private readonly IDishCommand _dishCommand;
        private readonly IDishQuery _dishQuery;
        private readonly ICategoryQuery _categoryQuery;
        private readonly ICategoryCommand _categoryCommand;
        public UpdateDishUseCase(
            ICategoryQuery categoryQuery,
            ICategoryCommand categoryCommand,
            IDishCommand dishCommand,
            IDishQuery dishQuery
            )
        {
            _categoryQuery = categoryQuery;
            _categoryCommand = categoryCommand;
            _dishCommand = dishCommand;
            _dishQuery = dishQuery;
        }
        public async Task<DishResponse> UpdateDish(Guid id, DishUpdateRequest DishUpdateRequest)
        {
            var existingDish = await _dishQuery.GetDishById(id);

            if (existingDish == null)
            {//que retorne null si no encuantre
                throw new NotFoundException($"Dish with ID {id} not found.");
            }
            var categoryExists = await _categoryQuery.CategoryExistAsync(DishUpdateRequest.Category);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {DishUpdateRequest.Category} not found.");
            }
            var alreadyExist = await _dishQuery.DishExists(DishUpdateRequest.Name, id);
            if (alreadyExist)
            {//buscar tirar la exception al controller
                throw new ConflictException($"dish {DishUpdateRequest.Name} already exists");
            }
            var category = await _categoryQuery.GetCategoryById(DishUpdateRequest.Category);

            existingDish.Name = DishUpdateRequest.Name;
            existingDish.Description = DishUpdateRequest.Description;
            existingDish.Price = DishUpdateRequest.Price;
            existingDish.Available = DishUpdateRequest.IsActive;
            existingDish.Category = DishUpdateRequest.Category;
            existingDish.ImageUrl = DishUpdateRequest.Image;
            existingDish.UpdateDate = DateTime.UtcNow;

            await _dishCommand.UpdateDish(existingDish);

            return new DishResponse
            {
                Id = existingDish.DishId,
                Name = existingDish.Name,
                Description = existingDish.Description,
                Price = existingDish.Price,
                Category = new GenericResponse { Id = category.Id, Name = category.Name},
                isActive = existingDish.Available,
                ImageUrl = existingDish.ImageUrl,
                createdAt = existingDish.CreateDate,
                updateAt = existingDish.UpdateDate                
            };
        }
    }
}
