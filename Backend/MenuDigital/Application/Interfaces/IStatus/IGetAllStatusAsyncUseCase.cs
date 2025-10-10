using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IStatus
{
    public interface IGetAllStatusAsyncUseCase
    {
        Task<List<StatusResponse>> GetAllStatuses();
    }
}
