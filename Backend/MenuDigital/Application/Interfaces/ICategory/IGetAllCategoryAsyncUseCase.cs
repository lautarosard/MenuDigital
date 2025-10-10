using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Response;
using Domain.Entities;
namespace Application.Interfaces.ICategory
{
    public interface IGetAllCategoryAsyncUseCase
    {
        Task<List<CategoryResponse>> GetAllAsync();
    }
}
