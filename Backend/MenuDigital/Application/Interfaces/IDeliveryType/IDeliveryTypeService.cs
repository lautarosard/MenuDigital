using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Models.Request;
using Application.Models.Response;

namespace Application.Interfaces.IDeliveryType
{
    public interface IDeliveryTypeService
    {
        // List
        Task<List<DeliveryTypeResponse>> GetAllDeliveryTypes();

        // Create
        Task<DeliveryTypeResponse> CreateDeliveryType(DeliveryTypeRequest deliveryTypeRequest);

        // Update
        Task<DeliveryTypeResponse> UpdateDeliveryType(int id, DeliveryTypeRequest deliveryTypeRequest);

        // Delete
        Task<DeliveryTypeResponse> DeleteDeliveryType(int id);

        // Queries
        Task<DeliveryTypeResponse> GetDeliveryTypeById(int id);
    }
}
