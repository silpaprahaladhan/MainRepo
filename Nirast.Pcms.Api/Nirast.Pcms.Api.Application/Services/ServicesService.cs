using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Services;
using Nirast.Pcms.Api.Sdk.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Application.Services
{
    public class ServicesService : IServicesService
    {
        IUnitOfWork _unitOfWork;
        public ServicesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        public async Task<int> AddServices(Api.Sdk.Entities.Services service)
        {
            return await _unitOfWork.ServiceReposoitory.AddService(service);
        }

        public async Task<IEnumerable<Api.Sdk.Entities.Services>> RetrieveServices(int serviceId)
        {
            return await _unitOfWork.ServiceReposoitory.RetrieveServices(serviceId);
        }


     
    }
}
