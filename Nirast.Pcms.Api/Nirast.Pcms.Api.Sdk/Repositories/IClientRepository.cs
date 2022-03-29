using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
   public interface IClientRepository: IGenericRepository<ClientDetails>
    {
        Task<ClientDetails> AddClientDetails(ClientDetails client);
        Task<ClientDetails> AddFranchiseDetails(ClientDetails client);

        Task<int> AddClientInvoiceDetails(InvoiceSearchInpts invoiceDetails);
        

            /// <summary>
            /// method to get all client details
            /// </summary>
            /// <returns></returns>
       Task<IEnumerable<ClientDetails>> GetClientDetails();
       Task<IEnumerable<ClientDetails>> GetClientDetailsByLocation(LocationSearchInputs inputs);

        Task<IEnumerable<ClientDetails>> GetFranchiseDetailsByLocation(LocationSearchInputs inputs);

        Task<IEnumerable<ScheduledData>> GetAllScheduleLogDetails();
       Task<IEnumerable<LoginLog>> GetLoginLogDetailsByTypeId(int typeId);
        Task<IEnumerable<RejectedCaretaker>> GetAllScheduleRejectedCaretaker(BookingHistorySearch bookingHistorySearch);

        Task<ClientDetails> GetClientDetailsByID(int clientId);
        Task<ClientDetails> GetFranchiseDetailsByID(int clientId);
        Task<IEnumerable<ScheduledData>> GetAllScheduledetails(CalenderEventInput calenderEventInput);

        Task<int> DeleteSchedule(ScheduleDeleteData deleteData);
        Task<int> ModifyClientStatusById(int id, int status);
        Task<int> ModifyFranchiseStatusById(int id, int status);

        Task<IEnumerable<UsersDetails>> GetUserTypeId();
        Task<int> UpdateClientInvoiceNumber(int clientId, int InvoiceNumber);
        Task<int> SaveClientCareTakerMapping(WorkShiftPayRates clientcaretaker);
        Task<int> SaveClientCareTakerPayRise(List<WorkShiftRates> workShiftRates);
        Task<int> SaveClientcategoryCareTakerPayRise(List<ClientCategoryRate> clientcaretaker);
        Task<int> DeleteClientCareTakerMapping(ClientCaretakers clientcaretaker);
        Task<int> SaveScheduleRejectedCareTaker(RejectedCaretaker careTaker);
        /// <summary>
        /// method to search client details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<ClientDetails>> ClientSearch(ClientSearchInputs inputs);

        Task<int> AddScheduledDetails(ScheduledData scheduledData,out string message);
        Task<IEnumerable<ClientDetails>> GetClientInvoiceDetails();
        Task<int> AddInvoiceDetails(ClientDetails clientInvoiceDetails);

        Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryList(InvoiceHistory invoiceHistory);
        Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryById(int id);
        Task<int> GetClientFromUserId(int id);
        Task<IEnumerable<WorkShiftRates>> GetMappedCaretakerRates(int clientId, int caretakerId);
        Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersLatestPayRiseRates(int clientId, int caretakerId);
        Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersPayRiseRatesByDate(PayriseData payriseData);
        Task<IEnumerable<ClientCategoryRate>> GetCategoryClientPayRiseRates(int clientId);
        Task<ScheduledData> GetSchdeuleDetaildById(int scheduleId);
        Task<string> GetEmailIdForClient(int userId);
        Task<ScheduledData> GetSchedulingLogDetailsById(int logId);
       
        Task<string> AddScheduledDetailsAuditLog(ScheduledData data, string message);
        Task<IEnumerable<ClientCategoryRate>> GetClientInvoicePayRiseRatesonDateChange(int clientId, DateTime date);
        Task<int> ChangeClientEmailStatus(int id, int emailstatus);
        Task<int> UpdateClientInvoice(InvoiceSearchInpts searchInpts);
        Task<int> GetClientEmailStatus(int clientId);

    }
}
