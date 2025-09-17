using Application.Models.Response.Dish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish
{
    public interface IDeleteDishUseCase
    {
        // Delete
        Task<DishResponse> DeleteDish(Guid id);
    }
}
