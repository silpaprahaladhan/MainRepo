using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<UserPaymentInvoiceModel>
    {
        Task<int> AddPaymentInvoiceDetails(InvoiceSearchInpts invoiceMail);
        Task<IEnumerable<UserBookingInvoiceReport>> GetUserInvoiceGenerationDetails(BookingHistorySearch inputs);

        /// <summary>
        /// method to get user payment invoice details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        Task<UserPaymentInvoiceModel> GetUserPaymentInvoiceDetails(int invoiceNumber);

        /// <summary>
        /// method to add user payment transaction details
        /// </summary>
        /// <param name="paymentTransactionModel"></param>
        /// <returns></returns>
        Task<UserPaymentInvoiceModel> AddPaymentTransactionDetails(UserPaymentTransactionModel paymentTransactionModel);

        /// <summary>
        /// method to get payment details
        /// </summary>
        /// 
        /// <returns></returns>
        Task<IEnumerable<PaymentHistory>> GetPaymentDetails();

        /// <summary>
        /// method to search payment details
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<PaymentHistory>> SearchPaymentDetails(PaymentAdvancedSearch inputs);
        Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReport(PaymentAdvancedSearch inputs);
        Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReportSummary(PaymentAdvancedSearch inputs);
        Task<IEnumerable<ScheduledData>> SearchClientScheduleReoprt(PaymentAdvancedSearch inputs);
        Task<IEnumerable<ScheduledData>> SearchCaretakerPayHoursReoprt(PaymentAdvancedSearch inputs);
        Task<IEnumerable<PaymentReportDetails>> SearchUserPaymentReport(PaymentReport inputs);
        Task<IEnumerable<AdminBookingNotification>> GetAdminNotification();
        Task<IEnumerable<AdminBookingList>> GetAdminBookingList();
        Task<IEnumerable<AdminBookingList>> GetAdminBookingListByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<AdminBookingNotification>> GetAdminNotificationByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<AdminSchedulingList>> GetAdminSchedulingList();
        Task<AdminBookingDetails> GetBookingDetailsById(int bookingId);
        Task<AdminSchedulingDetails> GetSchedulingDetailsById(int bookingId);
        Task<int> GenerateInvoice(InvoiceMail invoiceMail);
        Task<string> SavePublicUserPaymentInvoice(PublicUserPaymentInvoiceInfo invoiceData);
        Task<IEnumerable<ScheduledData>> GetInvoiceGenerationDetails(PaymentAdvancedSearch inputs);
        Task<IEnumerable<ScheduledData>> GetClientScheduledDetails(PaymentAdvancedSearch inputs);


        Task<IEnumerable<ScheduledData>> GetClientScheduledDetailsByBranchWise(PaymentAdvancedSearch inputs);
    }
}
