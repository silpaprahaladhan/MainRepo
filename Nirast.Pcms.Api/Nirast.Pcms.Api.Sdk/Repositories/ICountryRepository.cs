using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface ICountryRepository : IGenericRepository<Countries>
    {
        Task<int> AddCountry(Countries country);

        Task<IEnumerable<Countries>> RetrieveCountry(int CountryId);

        Task<Countries> GetDefaultCountry();

        Task<int> DeleteCountry(int CountryId);
    }
}
