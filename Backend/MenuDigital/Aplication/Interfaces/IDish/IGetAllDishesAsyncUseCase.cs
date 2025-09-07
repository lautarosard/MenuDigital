using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish
{
    public interface IGetAllDishesAsyncUseCase
    {
        // List
        Task<List<DishResponse>> GetAllDishesAsync();
    }
}
