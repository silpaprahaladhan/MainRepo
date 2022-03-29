using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IDesignationRepository : IGenericRepository<DesignationDetails>
    {
        Task<int> AddDesignation(DesignationDetails Designation);

        Task<IEnumerable<DesignationDetails>> RetrieveDesignation(int DesignationId);
        Task<DesignationDetails> RetrieveDesignationById(int DesignationId);
        Task<int> DeleteDesignation(int id);
    }
}
