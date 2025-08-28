using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Request;
using Application.Models.Response;

namespace Application.Interfaces.IStatus
{
    public interface IStatusService
    {
        // List
        Task<List<StatusResponse>> GetAllStatuses();

        // Create
        Task<StatusResponse> CreateStatus(string status);

        // Update
        Task<StatusResponse> UpdateStatus(int id, string status);

        // Delete
        Task<StatusResponse> DeleteStatus(int id);

        // Queries
        Task<StatusResponse> GetStatusById(int id);
    }
}
