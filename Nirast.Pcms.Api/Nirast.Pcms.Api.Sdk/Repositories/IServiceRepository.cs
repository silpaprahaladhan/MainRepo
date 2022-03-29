using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IServiceRepository : IGenericRepository<Entities.Services>
    {
        Task<int> AddService(Entities.Services service);

        Task<IEnumerable<Entities.Services>> RetrieveServices(int serviceId );
        Task<int> DeleteService(int ServiceId);
    }
}
