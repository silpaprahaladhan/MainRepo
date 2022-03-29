using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface ICareTakerRepository : IGenericRepository<CareTakerRegistrationModel>
    {
        Task<IEnumerable<CareTakerServices>> RetrieveCaregiverServices();
        Task<int> AddCareTaker(CareTakerRegistrationModel careTaker);
       
        Task<int> SaveCareTakerPayRise(List<CareTakerServices> careTaker);
        Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRates(int caretakerId);
        Task<CareTakerRegistrationModel> RetrieveCaretakerDetails(int caretakerUserId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategory(int CategoryId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndLocation(int CategoryId, LocationSearchInputs inputs);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByService(int ServiceId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndClientId(int CategoryId,int clientId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndDate(int CategoryId,string DateTime,int hours,int clientId);
        Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListByCategoryAndDate(int CategoryId,string DateTime, int hours, int clientId, int Workshift);
        Task<IEnumerable<CaretakerAvailableReport>> RetrieveAvailableCareTakerListReport(PaymentAdvancedSearch inputs);
        Task<IEnumerable<CaretakerAvailableReport>> RetrieveCommissionReport(PaymentAdvancedSearch inputs);
        Task<string> CaretakerProfileId();
        Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakers(int status);
        Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakersByLocation(int status, LocationSearchInputs inputs);
        Task<IEnumerable<CareTakerRegistrationModel>> SelectCaretakers(int status);
        Task<int> ChangeUserStatus(int UserRegnId, int AccountStatus);
        Task<int> ApproveCaretaker(ApproveCaretaker approveCaretaker);
        Task<int> RejectCaretakerApplication(RejectCareTaker rejectCareTaker);
        Task<int> DeleteCaretaker(int UserRegnId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdl();
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdlByLocation(LocationSearchInputs inputs);
        
        Task<IEnumerable<ClientScheduling>> GetSchedulingDetails(int caretakerId);
        Task<IEnumerable<UserBooking>> GetUserBookingDetails(CaretakerScheduleListSearch caretakerBookingListSearch);
        Task<IEnumerable<Notification>> GetNotification(int caretakerId);
        Task<NotificationDetails> GetNotificationDetailsById(int bookingId);
        Task<UpcomingAppointment> GetUpcomingNotifications(int caretakerId);
        Task<int> ConfirmAppointments(UpcomingAppointment upcomingAppointment);
        Task<IEnumerable<CaretakerScheduleList>> GetCaretakerScheduleList(CaretakerScheduleListSearch caretakerScheduleListSearch);
        Task<IEnumerable<CaretakerType>> GetCaretakerType();
        Task<string> GetEmailIdForAdmin();
        Task<List<string>> GetEmailIdForOfficeStaff();
        Task<string> GetEmailIdForUser(int userId);
        Task<UpcomingAppointment> GetAppointmentDetails(UpcomingAppointment upcomingAppointment);
        Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRatesonDateChange(DateTime date,int caretakerId);
        Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListForPublicUser(int categoryId, string startDateTime, int hours, int Workshift);
    }
}
