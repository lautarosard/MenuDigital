using Application.Exceptions;
using Application.Interfaces.ICategory.Repository;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.Repository;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IOrderItem.Repository;
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
        private readonly ICategoryQuery _categoryQuery;
        private readonly ICategoryCommand _categoryCommand;
        private readonly IDishCommand _dishCommand;
        private readonly IDishQuery _dishQuery;
        private readonly IOrderItemCommand _orderItemCommand;
        private readonly IOrderItemQuery _orderItemQuery;

        public DeleteDishUseCase(
            ICategoryQuery categoryQuery,
            ICategoryCommand categoryCommand,
            IDishCommand dishCommand,
            IDishQuery dishQuery,
            IOrderItemCommand orderItemCommand,
            IOrderItemQuery orderItemQuery
            )
        {
            _categoryQuery = categoryQuery;
            _categoryCommand = categoryCommand;
            _dishCommand = dishCommand;
            _dishQuery = dishQuery;
            _orderItemCommand = orderItemCommand;
            _orderItemQuery = orderItemQuery;
        }

        public async Task<DishResponse?> DeleteDish(Guid id)
        {
            var dish = await _dishQuery.GetDishById(id);
            if (dish == null)
            {
                throw new NotFoundException($"Dish with ID {id} not found.");
            }
            bool usedInOrders = await _orderItemQuery.ExistsByDishId(id);
            if (usedInOrders)
            {
                throw new ConflictException($"Dish with ID {id} cannot be deleted because it is used in existing orders.");
            }
            dish.Available = false; // Set the dish as inactive before deletion
            await _dishCommand.UpdateDish(dish);
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
