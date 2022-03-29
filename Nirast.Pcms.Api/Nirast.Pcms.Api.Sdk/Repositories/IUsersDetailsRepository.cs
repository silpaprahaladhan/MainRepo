using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.PublicUserCaretakerBooking;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IUsersDetailsRepository : IGenericRepository<PublicUserRegistration>
    {
        Task<int> DeleteSchedule(ScheduleDeleteData deleteData);
        Task<int> AddUserInvoiceDetails(InvoiceSearchInpts invoiceDetails);
        Task<int> UpdateUserInvoiceNumber(int userid,int invoiceNumber);
        Task<int> InsertUpdateCompanyDetails(CompanyProfile companyProfile);
        Task<EmailTypeConfiguration> GetEmailIdConfigByType(Enums.EmailType emailType, int branchId);
        Task<CompanyProfile> GetCompanyProfiles(int CompanyId);
        Task<int> AddPublicUser(PublicUserRegistration UsersDetails);
        Task<int> ForgotPassword(ForgotPasswordViewModel emailId);
        Task<int> DeleteUserDetails(string userStatus);
        Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsById(string Flag, string Value);
        Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsByLocation(string Flag, string Value,LocationSearchInputs inputs);
        
        Task<int> EditUserProfile(PublicUserRegistration UsersDetails);
        Task<int> EditCardDetails(PublicUserRegistration UsersDetails);
        Task<int> UpdateUserProfilePic(PublicUserRegistration UsersDetails);
        Task<int> UpdateUserVerification(VerifyUserAccount VerifyUser);
        Task<int> CheckLoginNameExist(string LoginName);
        Task<int> ChangeUserStatus(int userId, int status);
        Task<IEnumerable<UserBooking>> GetPublicUserBookingDetails(int publicUserId);
        Task<IEnumerable<UserBooking>> GetCaretakerBookingDetails(CalenderEventInput calenderEventInput);
        Task<IEnumerable<PublicUserPaymentHistory>> GetPublicUserPaymentHistory(int publicUserId);
        Task<IEnumerable<AdminBookingNotification>> GetPublicUserNotification(int publicUserId);
        Task<PublicUserNotificationDetails> GetUserNotificationDetailsById(int bookingId);
        Task<UserCredential> RetrievePassword(string emailId);
        Task<int> ChangePassword(ChangePassword changePasswordInputs);
        Task<int> ChangeBookigStatus(int userId, int status);
        Task<string> GetAdminEmailId();
        Task<UsersDetails> GetAdminProfile(int id);
        Task<int> UpdateAdminProfile(UsersDetails adminDetails);
        Task<int> UpdateUserEmail(int id, string emailId);
        Task<string> LoadPhoneCodeByCountryId(int countryId);
        Task<int> SaveResidentDetails(Resident residentDetails);
        Task<IEnumerable<Resident>> RetrieveResidentDetails();
        Task<int> DeleteResident(int residentId);
        Task<int> InsertUpdatePaypalSettings(PayPalAccount payPal);
        Task<PayPalAccount> GetPayPalAccount(int paypalId);
        Task<int> InsertUpdateTestimonials(Testimonial testimonial);
        Task<IEnumerable<Testimonial>> RetrieveTestimonials(int testimonialId);
        Task<int> DeleteTestimonial(int testimonialId);
        Task<int> AddEmailConfiguration(EmailConfiguration emailConfiguration);
        Task<IEnumerable<EmailConfiguration>> GetConfigList();
        Task<int> SetConfig(int configId);
        Task<int> DeleteConfigDetails(int configId);
        Task<EmailConfiguration> GetDefaultConfiguration();
        Task<int> AddEmailTypeConfiguration(EmailTypeConfiguration emailTypeConfiguration);
        Task<IEnumerable<EmailTypeConfiguration>> GetEmailTypeConfig();
        Task<int> DeleteEmailTypeConfig(int configId);
        //Task<EmailTypeConfiguration> GetEmailIdConfigByType(Enums.EmailType emailType);
        //Task<EmailTypeConfiguration> GetEmailIdConfigByType(Enums.EmailType emailType, int branchId);
        Task<IEnumerable<Resident>> RetrieveResidentDetailsById(int clientId);
        Task<IEnumerable<BookingPayriseModel>> GetBookingPayriseList(BookingPayriseModel bookingPayriseModel);
        Task<IEnumerable<InvoicePayriseModel>> GetInvoicePayriseList(InvoicePayriseModel invoicePayriseModel);
        Task<IEnumerable<PayrollPayriseModel>> GetPayrollPayriseList(PayrollPayriseModel payrollPayrise);
        Task<int> DeleteBookingPayrise(int bookingPayriseId);
        Task<int> DeleteInvoicePayrise(int invoicePayriseId);
        Task<int> DeletePayrollPayrise(int payrollPayriseId);
        Task<IEnumerable<BookingPayriseModel>> GetAllBookingPayriseDetails();
        Task<IEnumerable<InvoicePayriseModel>> GetAllInvoicePayriseDetails();
        Task<IEnumerable<PayrollPayriseModel>> GetAllPayrollPayriseDetails();

        /// <summary>
        /// DB Migration purpose only
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UsersDetails>> GetAllUserDetails();
        Task<IEnumerable<UsersDetails>> GetAllUserDetailsByLocation(LocationSearchInputs inputs);
        Task<int> UpdateUserDetails(UsersDetails usersDetails);
        Task<IEnumerable<DocumentsList>> GetAllCaretakerDocuments();
        Task<int> UpdateCaretakerDocuments(DocumentsList doc);
        Task<UserRoleDetailsModel> GetUserRoleDetails(int userId);
        Task<int> VerifyEmail(int userId);
        Task<int> AddInvoiceDetails(PublicUserPaymentInvoiceInfo InvoiceDetails);
        Task<IEnumerable<UsersDetails>> GetUserInvoiceDetails();
        Task<UserBooking> GetPublicUserBookingDetailsById(int bookingId);
        Task<int> AddBookingDetails(PublicUserCaretakerBooking data, out string message);
        Task<IEnumerable<PublicUserCaretakerBooking>> GetAllBookingdetails(CalenderBookingEventInput calenderEventInput);
    }
}
