using Nirast.Pcms.Api.Sdk.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IWorkShiftRepository : IGenericRepository<WorkShifts>
    {
        Task<int> AddWorkShift(WorkShifts workShift);

        Task<IEnumerable<WorkShifts>> RetrieveWorkShift(int ShiftId);
        Task<int> DeleteWorkShift(int workShiftId);
    }
}
