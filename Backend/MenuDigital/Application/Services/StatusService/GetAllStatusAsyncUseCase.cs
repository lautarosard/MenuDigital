using Application.Interfaces.IStatus;
using Application.Interfaces.IStatus.Repository;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.StatusService
{
    public class GetAllStatusAsyncUseCase : IGetAllStatusAsyncUseCase
    {
        private readonly IStatusQuery _statusQuery;
        public GetAllStatusAsyncUseCase(IStatusQuery statusQuery)
        {
            _statusQuery = statusQuery;
        }
        public async Task<List<StatusResponse>> GetAllStatuses()
        {
            var statuses = await _statusQuery.GetAllStatuses();
            return statuses.Select(s => new StatusResponse
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }
    }
}
