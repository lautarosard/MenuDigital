using Application.Models.Request;
using Application.Models.Response.Dish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish
{
    public interface IUpdateDishUseCase
    {
        // Update
        Task<DishResponse> UpdateDish(Guid id, DishUpdateRequest DishUpdateRequest);
    }
}
