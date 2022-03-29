using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;
using static Nirast.Pcms.Api.Sdk.Entities.PublicUserCaretakerBooking;

namespace Nirast.Pcms.Api.Sdk.Services
{
    public interface IPCMSService
    {
        Task<int> ChangeBookigStatus(int userId, int status, string siteURL, string reason);
        Task<int> DeleteSchedule(ScheduleDeleteData deleteData);
        Task<int> AddUserInvoiceDetails(InvoiceSearchInpts invoiceDetails);
        Task<int> UpdateUserInvoiceNumber(int userid,int invoicenumber);
        Task<IEnumerable<CareTakerServices>> RetrieveCaregiverServices();
        Task<int> AddCountry(Countries country);
        Task<int> InsertUpdateCompanyDetails(CompanyProfile companyProfile);
        Task<CompanyProfile> GetCompanyProfiles(int CompanyId);

        Task<IEnumerable<Cities>> GetBranchesByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<Countries>> RetrieveCountry(int countryId);
        Task<Countries> GetDefaultCountry();

        Task<int> AddState(States state);

        Task<IEnumerable<States>> RetrieveStates(int stateId);

        Task<int> ForgotPassword(ForgotPasswordViewModel emailId);
        Task<int> AddCareTaker(CareTakerRegistrationModel careTaker);
        Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRates(int caretakerId);
        Task<int> SaveCareTakerPayRise(List<CareTakerServices> caretaker);
        Task<CareTakerRegistrationModel> RetrieveCaretakerDetails(int CaretakerId);
        Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakers(int status);
        Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakersByLocation(int status, LocationSearchInputs inputs);
        Task<IEnumerable<CareTakerRegistrationModel>> SelectCaretakers(int status);
        Task<int> RejectCaretakerApplication(RejectCareTaker rejectCareTaker);
        Task<int> DeleteCaretaker(int UserRegnId);

        Task<IEnumerable<Privileges>> SelectRolePrivileges(int RoleId);
        Task<int> SaveRolePrivileges(SaveRolePrivileges saveRolePrivileges);

        Task<int> ApproveCaretaker(ApproveCaretaker approveCaretaker);
        Task<int> AddPublicUser(PublicUserRegistration UsersDetails);
        Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsById(string flag, string value);
        Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsByLocation(string flag, string value,LocationSearchInputs inputs);
        Task<int> EditUserProfile(PublicUserRegistration UsersDetails);

        Task<int> VerifyUserProfile(VerifyUserAccount VerifyUser);
        Task<int> EditCardDetails(PublicUserRegistration UsersDetails);
        Task<int> UpdateUserProfilePic(PublicUserRegistration UsersDetails);
        Task<int> AddCity(Cities City);
        Task<IEnumerable<Cities>> RetrieveCities(string flag, string value);

        Task<IEnumerable<Cities>> RetrieveCountry(string flag, string value);

        Task<IEnumerable<Cities>> Retrievestates(string flag, string value);

        Task<IEnumerable<Cities>> RetrievecityDetails(string flag, string value);
        Task<IEnumerable<Cities>> RetrieveBranches(string flag, string value);
        Task<IEnumerable<Cities>> RetrieveBranchesById(string flag, string value);
        Task<IEnumerable<Cities>> RetrieveAllBranches();
        Task<IEnumerable<Cities>> RetrieveBranchById(int id);

        Task<int> DeleteCity(int cityId);
        Task<int> DeleteCountry(int countryId);
        Task<int> DeleteState(int stateId);
        Task<int> DeleteService(int serviceId);
        Task<int> AddCategory(CategoryModel City);
        Task<IEnumerable<CategoryModel>> RetrieveCategory(string flag, string value);
        Task<int> DeleteCategory(int cityId);

        Task<int> AddDesignation(DesignationDetails Designation);
        Task<IEnumerable<DesignationDetails>> RetrieveDesignation(int DesignationId);
        Task<int> DeleteDesignation(int DesignationId);
        Task<int> AddQualification(QualificationDetails Qualification);
        Task<IEnumerable<QualificationDetails>> RetrieveQualification(int QualificationId);
        Task<int> DeleteQualification(int QualificationId);
        Task<IEnumerable<Cities>> GetCityById(int stateId);
        Task<UserCredential> RetrievePassword(string emailId);
        Task<int> ChangePassword(ChangePassword changePasswordInputs);

        /// <summary>
        /// method to get states bu country id
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<States>> GetStatesById(int countryId);


        Task<string> CaretakerProfileId();

        Task<int> SaveCaretakerBooking(CaretakerBookingModel caretakerBooking);

        Task<int> AddWorkShift(WorkShifts workShift);

        Task<IEnumerable<WorkShifts>> RetrieveWorkShift(int shiftId);
        Task<int> DeleteWorkShift(int workShiftId);

        Task<int> AddTimeShift(ClientTimeShifts timeShift);

        Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryList(InvoiceHistory invoiceHistory);
        Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryById(int id);
        Task<int> DeleteClientShiftDetail(int TimeShiftId);
        Task<IEnumerable<BookingSchedulingData>> GetAllBookingSchedulingData(CalenderEventInput calenderEventInput);
        Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShift(int timeShiftId);
        Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShiftByClientId(int ClientId);

        Task<int> AddHoliday(Holidays holiday);
        Task<int> OverrideHoliday();

        Task<IEnumerable<Holidays>> RetrieveHoliday(Holidays holidaySearchModel);
        Task<float> RetrieveHolidayPayDetails();
        Task<float> RetrieveGetIntervalHours();
        Task<int> AddHolidayPay(Holidays holiday);
        Task<int> AddIntervalHours(ClientTimeShifts shifts);

        Task<int> DeleteHoliday(int id);

        Task<int> CheckLoginNameExist(string LoginName);

        Task<ClientDetails> AddClientDetails(ClientDetails clientRegistration);
      

        Task<int> AddClientInvoiceDetails(InvoiceSearchInpts invoiceDetails);


        Task<int> AddScheduledDetails(ScheduledData scheduledData);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategory(int CategoryId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndLocation(int CategoryId ,LocationSearchInputs inputs);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByService(int ServiceId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndClientId(int CategoryId, int ClientId);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndDate(int CategoryId, string DateTime, int hours, int clientId);
        Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListByCategoryAndDate(int CategoryId, string DateTime, int hours, int clientId, int Workshift);

        Task<IEnumerable<CaretakerAvailableReport>> RetrieveAvailableCareTakerListReport(PaymentAdvancedSearch inputs);


        Task<IEnumerable<CaretakerAvailableReport>> RetrieveCommissionReport(PaymentAdvancedSearch inputs);
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdl();
        Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdlByLocation(LocationSearchInputs inputs);
        Task<int> InsertUpdatePaypalSettings(PayPalAccount payPal);
        Task<List<LoggedInUser>> CheckLoginCredentials(UserCredential userCredential);

        Task<List<LoggedInUser>> CheckLoginCredentialsDouble(UserCredential userCredential);

        #region created by silpa on Admintype
        Task<List<LoggedInUser>> CheckLoginAdmin(LoggedInUser userCredential);
        #endregion

        Task<ClientDetails> AddFranchiseDetails(ClientDetails clientRegistration);
        Task<LoggedInUser> GetLoggedInUserDetails(int userId);
        Task<bool> AddLoginLog(int userId);
        Task<int> AddQuestions(QuestionareModel questions);
        Task<IEnumerable<QuestionareModel>> RetrieveQuestions(int questionId);
        Task<int> DeleteQuestions(int questionId);
        Task<int> ChangeUserStatus(int userId, int status);
        Task<int> AddRoles(Roles roles);
        Task<IEnumerable<Roles>> RetrieveRolesById(int roleId);
        Task<int> DeleteRoles(int id);
        Task<IEnumerable<ClientScheduling>> GetSchedulingDetails(int caretakerId);
        Task<IEnumerable<UserBooking>> GetUserBookingDetails(CaretakerScheduleListSearch caretakerBookingListSearch);
        Task<IEnumerable<Notification>> GetNotification(int caretakerId);
        Task<NotificationDetails> GetNotificationDetailsById(int bookingId);
        Task<UpcomingAppointment> GetUpcomingNotifications(int caretakerId);
        Task<int> ConfirmAppointments(UpcomingAppointment upcomingAppointment);

        Task<IEnumerable<UserBooking>> GetPublicUserBookingDetails(int publicUserId);
        Task<IEnumerable<UserBooking>> GetCaretakerBookingDetails(CalenderEventInput calenderEventInput);
        Task<IEnumerable<PublicUserPaymentHistory>> GetPublicUserPaymentHistory(int publicUserId);
        Task<IEnumerable<AdminBookingNotification>> GetPublicUserNotification(int publicUserId);
        Task<PublicUserNotificationDetails> GetUserNotificationDetailsById(int bookingId);
       
        Task<IEnumerable<CaretakerScheduleList>> GetCaretakerScheduleList(CaretakerScheduleListSearch caretakerScheduleListSearch);
        Task<int> SendContactForm(ContactModel contactModel);
        Task<UsersDetails> GetAdminProfile(int id);
        Task<int> UpdateAdminProfile(UsersDetails adminDetails);
        Task<int> UpdateUserEmail(int id, string emailId);
        Task<string> LoadPhoneCodeByCountryId(int countryId);
        Task<IEnumerable<CaretakerType>> GetCaretakerType();
        Task<int> SaveResidentDetails(Resident residentDetails);
        Task<IEnumerable<Resident>> RetrieveResidentDetails();
        Task<IEnumerable<Resident>> RetrieveResidentDetailsById(int clientId);
        Task<int> DeleteResident(int residentId);
        Task<PayPalAccount> GetPayPalAccount(int paypalId);
        Task<int> InsertUpdateTestimonials(Testimonial testimonial);
        Task<IEnumerable<Testimonial>> RetrieveTestimonials(int testimonialId);
        Task<int> DeleteTestimonial(int testimonialId);
        Task<int> AddEmailConfiguration(EmailConfiguration emailConfiguration);
        Task<IEnumerable<EmailTypeConfiguration>> GetEmailTypeConfig();
        Task<int> SetConfig(int configId);
        Task<int> DeleteConfigDetails(int configId);
        Task<int> AddEmailTypeConfiguration(EmailTypeConfiguration emailTypeConfiguration);
        Task<int> DeleteEmailTypeConfig(int configId);
        Task<Cities> GetBranchByUserId(int Id);
        Task<IEnumerable<EmailConfiguration>> GetConfigList();
        Task<EmailTypeConfiguration> GetEmailIdConfigByType(EmailType emailType, int branchId);
        Task<EmailConfiguration> GetDefaultConfiguration();
        Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRatesonDateChange(DateTime date,int caretakerId);
        Task<IEnumerable<BookingPayriseModel>> GetBookingPayriseList(BookingPayriseModel bookingPayrise);
        Task<IEnumerable<InvoicePayriseModel>> GetInvoicePayriseList(InvoicePayriseModel invoicePayrise);
        Task<IEnumerable<PayrollPayriseModel>> GetPayrollPayriseList(PayrollPayriseModel payrollPayrise);
        Task<int> DeleteBookingPayrise(int bookingPayriseId);
        Task<int> DeleteInvoicePayrise(int invoicePayriseId);
        Task<int> DeletePayrollPayrise(int payrollPayriseId);
        Task<IEnumerable<BookingPayriseModel>> GetAllBookingPayriseDetails();
        Task<IEnumerable<InvoicePayriseModel>> GetAllInvoicePayriseDetails();
        Task<IEnumerable<PayrollPayriseModel>> GetAllPayrollPayriseDetails();

        /// <summary>
        /// For DB Migration purpose only
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UsersDetails>> GetAllUserDetails();


        /// <summary>
        /// For DB Migration purpose only
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UsersDetails>> GetAllUserDetailsByLocation(LocationSearchInputs inputs);

        /// <summary>
        ///  For DB Migration purpose only
        /// </summary>
        /// <param name="usersDetails"></param>
        /// <returns></returns>
        Task<int> UpdateUserDetails(UsersDetails usersDetails);

        /// <summary>
        /// For DB Migration purpose only
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DocumentsList>> GetAllCaretakerDocuments();
        Task<int> UpdateCaretakerDocuments(DocumentsList doc);
        Task<UserRoleDetailsModel> GetUserRoleDetails(int userId);
        Task<int> VerifyEmail(int userId);
        Task<int> AddInvoiceDetails(PublicUserPaymentInvoiceInfo userInvoiceDetails);
        Task<IEnumerable<UsersDetails>> GetUserInvoiceDetails();
        Task<UserBooking> GetPublicUserBookingDetailsById(int bookingId);
        Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListForPublicUser(int CategoryId, string DateTime, int hours, int Workshift);
        Task<int> AddBookingDetails(PublicUserCaretakerBooking bookingData);
        Task<IEnumerable<PublicUserCaretakerBooking>> GetAllBookingdetails(CalenderBookingEventInput calenderEventInput);
    }
}
