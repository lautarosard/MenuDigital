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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        public DeleteDishUseCase(ICategoryRepository categoryRepository, IDishRepository dishRepository, IOrderItemRepository orderItemRepository)
        {
            _categoryRepository = categoryRepository;
            _dishRepository = dishRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<DishResponse?> DeleteDish(Guid id)
        {
            var dish = await _dishRepository.GetDishById(id);
            if (dish == null)
            {
                throw new NotFoundException($"Dish with ID {id} not found.");
            }
            bool usedInOrders = await _orderItemRepository.ExistsByDishId(id);
            if (usedInOrders)
            {
                throw new ConflictException($"Dish with ID {id} cannot be deleted because it is used in existing orders.");
            }
            dish.Available = false; // Set the dish as inactive before deletion
            await _dishRepository.UpdateDish(dish);
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
