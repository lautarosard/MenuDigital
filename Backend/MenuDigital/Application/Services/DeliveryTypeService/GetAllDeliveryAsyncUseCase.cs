using Application.Interfaces.ICategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Response;
using Application.Interfaces.IDeliveryType.Repository;
using Application.Interfaces.IDeliveryType;

namespace Application.Services.DeliveryTypeService
{
    public class GetAllDeliveryAsyncUseCase : IGetAllDeliveryAsyncUseCase
    {
        private readonly IDeliveryTypeQuery _deliveryTypeQuery;
        public GetAllDeliveryAsyncUseCase(IDeliveryTypeQuery deliveryTypeQuery)
        {
            _deliveryTypeQuery = deliveryTypeQuery;
        }
        public async Task<List<DeliveryTypeResponse>> GetAllAsync()
        {
            var deliveryTypes = await _deliveryTypeQuery.GetAllDeliveryTypes();
            return deliveryTypes.Select(dt => new DeliveryTypeResponse
            {
                Id = dt.Id,
                Name = dt.Name
            }).ToList();
        }
    }
}
