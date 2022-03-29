using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
   public interface ICityRepository  : IGenericRepository<Cities>
    {
        Task<int> AddCity(Cities city);

        Task<IEnumerable<Cities>> RetrieveCities(string flag,string value);
        Task<IEnumerable<Cities>> RetrieveBranches(string flag, string value);
        Task<IEnumerable<Cities>> RetrieveBranchesById(string flag, string value);
        Task<IEnumerable<Cities>> RetrieveAllBranches();
        Task<Cities> GetBranchByUserId(int id);
        Task<IEnumerable<Cities>> RetrieveCountry(string flag, string value);
        Task<IEnumerable<Cities>> Retrievestates(string flag, string value);
        Task<IEnumerable<Cities>> RetrievecityDetails(string flag, string value);
        Task<IEnumerable<Cities>> GetBranchesByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<Cities>> RetrieveBranchById(int id);
        Task<int> DeleteCity(int id);

        /// <summary>
        /// To get city details by state id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        Task<IEnumerable<Cities>> GetCityByStateId(int stateId);
    }
}
