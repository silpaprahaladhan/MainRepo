using System.Collections.Generic;
using Nirast.Pcms.Api.Sdk.Entities;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IHolidayRepository : IGenericRepository<Holidays>
    {
        Task<int> AddHoliday(Holidays holiday);
        Task<int> OverrideHoliday();

        Task<IEnumerable<Holidays>> RetrieveHoliday(Holidays holidaySearchModel);
        Task<int> DeleteHoliday(int id);

        Task<int> AddHolidayPay(Holidays holiday);
        Task<int> AddIntervalHours(ClientTimeShifts shifts);

        Task <float> RetrieveHolidayPayDetails();
        Task <float> RetrieveGetIntervalHours();

    }
}
