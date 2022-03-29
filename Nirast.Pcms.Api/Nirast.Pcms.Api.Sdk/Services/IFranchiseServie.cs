using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Services
{
   public interface IFranchiseService
    {
        /// <summary>
        /// method to get all client details
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ClientDetails>> GetAllClientDetails();
        Task<IEnumerable<ClientDetails>> GetAllClientDetailsByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<ScheduledData>> GetAllScheduleLogDetails();
        Task<IEnumerable<LoginLog>> GetLoginLogDetailsByTypeId(int typeId);
        Task<IEnumerable<UsersDetails>> GetUserTypeId();
        Task<IEnumerable<RejectedCaretaker>> GetAllScheduleRejectedCaretaker(BookingHistorySearch bookingHistorySearch);

        Task<ClientDetails> GetClientDetailsByID(int clientId);

        Task<IEnumerable<ScheduledData>> GetAllScheduledetails(CalenderEventInput calenderEventInput);

        Task<int> DeleteSchedule(ScheduleDeleteData deleteData);

        Task<int> ModifyClientStatusById(int id, int status);
        Task<int> UpdateClientInvoiceNumber(int clientId, int InvoiceNumber);
        Task<int> SaveClientCareTakerMapping(WorkShiftPayRates shiftPayRates);
        Task<int> SaveClientCareTakerPayRise(List<WorkShiftRates> workShiftRates);
        Task<int> SaveClientcategoryCareTakerPayRise(List<ClientCategoryRate> clientcaretaker);
        Task<int> DeleteClientCareTakerMapping(ClientCaretakers clientcaretaker);
        Task<int> SaveScheduleRejectedCareTaker(RejectedCaretaker careTaker);
        /// <summary>
        /// method to search client details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<ClientDetails>> SearchClient(ClientSearchInputs inputs);
        Task<IEnumerable<ClientDetails>> GetClientInvoiceDetails();
        Task<int> AddInvoiceDetails(ClientDetails clientInvoiceDetails);
        Task<int> GetClientFromUserId(int id);
        Task<IEnumerable<WorkShiftRates>> GetMappedCaretakerRates(int clientId, int caretakerId);
        Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersLatestPayRiseRates(int clientId, int caretakerId);
        Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersPayRiseRatesByDate(PayriseData payriseData);
        Task<IEnumerable<ClientCategoryRate>> GetCategoryClientPayRiseRates(int clientId);
        Task<ScheduledData> GetSchedulingLogDetailsById(int logId);
       
        Task<IEnumerable<ClientCategoryRate>> GetClientInvoicePayRiseRatesonDateChange(int clientId, DateTime date);
       
        Task<int> ChangeClientEmailStatus(int id, int emailstatus);
        Task<int> UpdateClientInvoice(InvoiceSearchInpts searchInpts);


    }
}
