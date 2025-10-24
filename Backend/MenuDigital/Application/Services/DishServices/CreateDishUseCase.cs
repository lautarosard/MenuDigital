using Application.Exceptions;
using Application.Interfaces.ICategory;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.Repository;
using Application.Models.Request;
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
    public class CreateDishUseCase : ICreateDishUseCase
    {
        private readonly ICategoryQuery _categoryQuery;
        private readonly ICategoryCommand _categoryCommand;
        private readonly IDishCommand _dishCommand;
        private readonly IDishQuery _dishQuery;
        private readonly ICategoryExistUseCase _categoryExist;

        public CreateDishUseCase(
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
        public async Task<DishResponse?> CreateDish(DishRequest dishRequest)
        {
            //validaciones
            var existingDish = await _dishQuery.DishExists(dishRequest.Name,null);
            // if already exist a dish with that name, throw a 409 Conflict 
            if (existingDish)
            {
                throw new ConflictException($"A dish with this name {dishRequest.Name} already exists.");
            }
            var categoryExists = await _categoryQuery.CategoryExistAsync(dishRequest.Category);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {dishRequest.Category} not found.");
            }
            var category = await _categoryQuery.GetCategoryById(dishRequest.Category);
            var dish = new Dish
            {
                DishId = Guid.NewGuid(),
                Name = dishRequest.Name,
                Description = dishRequest.Description,
                Price = dishRequest.Price,
                Available = true,
                ImageUrl = dishRequest.Image,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Category = dishRequest.Category
            };
            await _dishCommand.InsertDish(dish);
            return new DishResponse
            {
                Id = dish.DishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = new GenericResponse { Id = category.Id, Name = category.Name },
                isActive = dish.Available,
                ImageUrl = dish.ImageUrl,
                createdAt = dish.CreateDate,
                updateAt = dish.UpdateDate
            };
        }
    }
}
