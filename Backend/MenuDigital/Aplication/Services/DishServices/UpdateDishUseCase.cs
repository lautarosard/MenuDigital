using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DishServices
{
    public class UpdateDishUseCase : IUpdateDishUseCase
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;
        public UpdateDishUseCase(IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
        }
        public async Task<UpdateDishResult> UpdateDish(Guid id, DishUpdateRequest DishUpdateRequest)
        {
            var existingDish = await _query.GetDishById(id);

            if (existingDish == null)
            {//que retorne null si no encuantre
                return new UpdateDishResult { NotFound = true };
            }
            var alreadyExist = await _query.DishExists(DishUpdateRequest.Name);
            if (alreadyExist)
            {//buscar tirar la exception al controller
                return new UpdateDishResult { NameConflict = true };
            }
            var category = await _categoryQuery.GetCategoryById(DishUpdateRequest.Category);

            existingDish.Name = DishUpdateRequest.Name;
            existingDish.Description = DishUpdateRequest.Description;
            existingDish.Price = DishUpdateRequest.Price;
            existingDish.Available = DishUpdateRequest.IsActive;
            existingDish.Category = DishUpdateRequest.Category;
            existingDish.ImageUrl = DishUpdateRequest.Image;
            existingDish.UpdateDate = DateTime.UtcNow;

            await _command.UpdateDish(existingDish);

            return new UpdateDishResult
            {
                Success = true,
                UpdatedDish = new DishResponse
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
                }
            };
        }
    }
}
