using System.Collections.Generic;
using Nirast.Pcms.Api.Sdk.Entities;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface ITimeShiftRepository : IGenericRepository<ClientTimeShifts>
    {
        Task<int> AddTimeShift(ClientTimeShifts timeShift);

        Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShift(int TimeShiftId);
        Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShiftByClientId(int ClientId);

        Task<int> DeleteClientShiftDetail(int TimeShiftId);
    }
}
