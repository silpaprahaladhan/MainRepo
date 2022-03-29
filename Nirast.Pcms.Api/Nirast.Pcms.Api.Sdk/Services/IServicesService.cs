using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Services
{
   public interface IServicesService
    {
        Task<int> AddServices(Entities.Services service);

        Task<IEnumerable<Entities.Services>> RetrieveServices(int serviceId);

       
    }
}
