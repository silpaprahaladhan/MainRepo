using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IStateRepository : IGenericRepository<States>
    {
        Task<int> AddState(States state);

        Task<IEnumerable<States>> RetrieveStates(int stateId );
        /// <summary>
        /// To get States by country id
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<States>> GetStatesById(int countryId);

        Task<int> DeleteState(int StateId);
    }
}
