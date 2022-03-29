using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Services
{
   public interface IInvoiceService
    {

        /// <summary>
        /// to get invoice details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        Task<IEnumerable<UserBookingInvoiceReport>> GetUserInvoiceGenerationDetails(BookingHistorySearch inputs);
        Task<UserPaymentInvoiceModel> GetUserPaymentInvoiceDetails(string invoiceNumber);

        /// <summary>
        /// method to add user payment transaction details
        /// </summary>
        /// <param name="paymentTransactionModel"></param>
        /// <returns></returns>
        Task<int> AddPaymentTransactionDetails(UserPaymentTransactionModel paymentTransactionModel);

        /// <summary>
        /// to get payment details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        Task<IEnumerable<PaymentHistory>> GetPaymentDetails();

        /// <summary>
        /// to search payment details
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Task<IEnumerable<PaymentHistory>> SearchPaymentDetails(PaymentAdvancedSearch inputs);
        Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReport(PaymentAdvancedSearch inputs);
        Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReportSummary(PaymentAdvancedSearch inputs);
        Task<IEnumerable<ScheduledData>> SearchClientScheduleReoprt(PaymentAdvancedSearch inputs);
        Task<IEnumerable<ScheduledData>> SearchCaretakerPayHoursReoprt(PaymentAdvancedSearch inputs);
        Task<IEnumerable<PaymentReportDetails>> SearchUserPaymentReport(PaymentReport inputs);
        Task<IEnumerable<AdminBookingNotification>> GetAdminNotification();
        Task<IEnumerable<AdminBookingList>> GetAdminBookingList();
        Task<IEnumerable<AdminSchedulingList>> GetAdminSchedulingList();
        Task<AdminBookingDetails> GetBookingDetailsById(int bookingId);
        Task<AdminSchedulingDetails> GetSchedulingDetailsById(int schedulingId);
        Task<int> GenerateInvoice(InvoiceMail invoiceMail);
        Task SendPaymentInvoiceToUser(InvoiceMail invoiceMail);
        Task<string> SavePublicUserPaymentInvoice(PublicUserPaymentInvoiceInfo invoiceData);
        Task<IEnumerable<ScheduledData>> GetInvoiceGenerationDetails(PaymentAdvancedSearch inputs);
        Task<IEnumerable<ScheduledData>> GetClientScheduledDetails(PaymentAdvancedSearch inputs);
        Task<int> AddPaymentInvoiceDetails(InvoiceSearchInpts invoiceMail);
        Task<IEnumerable<AdminBookingList>> GetAdminBookingListByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<AdminBookingNotification>> GetAdminNotificationByLocation(LocationSearchInputs inputs);
        Task<IEnumerable<ScheduledData>> GetClientScheduledDetailsByBranchWise(PaymentAdvancedSearch inputs);
    }
}
