using Nirast.Pcms.Ap.Application.Infrastructure;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Services;
using Nirast.Pcms.Api.Sdk.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private Sdk.Logger.IPCMSLogger _logger;
        IUnitOfWork _unitOfWork;
        private INotificationService _notificationService;
        private IPCMSService _pcmsService;
        public InvoiceService(IUnitOfWork unitOfWork, INotificationService notificationService, IPCMSService pCMSService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _pcmsService = pCMSService;
        }
        /// <summary>
        /// method to add user payment transaction details
        /// </summary>
        /// <param name="paymentTransactionModel"></param>
        /// <returns></returns>
        public async Task<int> AddPaymentTransactionDetails(UserPaymentTransactionModel paymentTransactionModel)
        {
            UserPaymentInvoiceModel userModel = await _unitOfWork.invoiceRepository.AddPaymentTransactionDetails(paymentTransactionModel);
            if (paymentTransactionModel.Status == true)
            {
                //Task.Factory.StartNew(() => { SendEmailToUser(paymentTransactionModel.SiteURL,paymentTransactionModel.TransactionNumber, userModel); });
                if (userModel != null)
                {
                    return 1;
                }
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private int SendEmailToUser(string siteURL,string txnId, UserPaymentInvoiceModel userModel)
        {
            _logger.Info("SendEmailToUser");
            EmailInput input = new EmailInput();            
            input.EmailId = userModel.EmailId;
            input.UserName = userModel.UserName;
            input.Subject = "Transaction Status";
            input.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
            var branchDetails = _pcmsService.GetBranchByUserId(userModel.UserId);
            input.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration, branchDetails.Result.BranchId).Result;
           // input.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration).Result;
            input.Body = GetTransactionEmailBody(siteURL, userModel, txnId);
            input.EmailType = EmailType.UserPayment;
            _notificationService.SendEMail(input);
            if (userModel != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// method to impliment get invoice details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public async Task<UserPaymentInvoiceModel> GetUserPaymentInvoiceDetails(string invoiceNumber)
        {
            //To do decription
            int invoice = Convert.ToInt32(invoiceNumber);
            return await _unitOfWork.invoiceRepository.GetUserPaymentInvoiceDetails(invoice);
        }
        /// <summary>
        /// Gets the transaction email body.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="txnId">The TXN identifier.</param>
        /// <returns></returns>
        private string GetTransactionEmailBody(string siteURL, UserPaymentInvoiceModel userPayment, string txnId)
        {
            string WelcomeMsg = "Booking Payment Status";
            string CompanyName = "Tranquil Care Inc.";
            string MailMsg = "Your PayPal transaction with <strong>" + CompanyName + @"</strong> is successful.<br/>";
            string Mailcontent = @"<table style='font - family: Roboto, Tahoma, Arial, Helvetica, sans - serif; font - weight: 300; color: #6b6b6b; margin: 7px auto; font-size: 1em;' cellpadding='5'>
                                                            <tr>     
                                                                <td> Invoice Number </td>        
                                                                <td>:</td>          
                                                                <td> " + userPayment.InvoiceNumber + @" </td>             
                                                            </tr>
                                                            <tr>     
                                                                <td> Transaction Number </td>        
                                                                <td>:</td>          
                                                                <td> " + txnId + @" </td>             
                                                            </tr>
														    <tr>
														        <td style='width: 150px'>Amount</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + userPayment.CurrencySymbol + " " + userPayment.Amount.ToString("0.00") + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Tax </td>        
                                                                <td>:</td>          
                                                                <td> " + userPayment.CurrencySymbol + " " + userPayment.TaxAmount.ToString("0.00") + @" </td>             
                                                            </tr>   
                                                            <tr>     
                                                                <td> Total Amount </td>        
                                                                <td>:</td>          
                                                                <td> " + userPayment.CurrencySymbol + " " + userPayment.TotalAmount.ToString("0.00") + @" </td>             
                                                            </tr>
                                                            </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = siteURL;
            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string body = "";
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, WelcomeMsg, userPayment.FirstName+" "+userPayment.LastName, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
            return body;
        }
        /// <summary>
        /// method to impliment get invoice details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentHistory>> GetPaymentDetails()
        {
          
            return await _unitOfWork.invoiceRepository.GetPaymentDetails();
        }


        public async Task<IEnumerable<AdminBookingList>> GetAdminBookingListByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.invoiceRepository.GetAdminBookingListByLocation(inputs);
        }
        public async Task<IEnumerable<AdminBookingNotification>> GetAdminNotificationByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.invoiceRepository.GetAdminNotificationByLocation(inputs);
        }

        /// <summary>
        /// method to impliment get invoice details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentHistory>> SearchPaymentDetails(PaymentAdvancedSearch inputs)
        {

            return await _unitOfWork.invoiceRepository.SearchPaymentDetails(inputs);
        }
        public async Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReport(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.SearchClientInvoiceReport(inputs);           
        }
        public async Task<IEnumerable<InvoiceReportData>> SearchClientInvoiceReportSummary(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.SearchClientInvoiceReportSummary(inputs);
        }

        public async Task<IEnumerable<ScheduledData>> GetClientScheduledDetailsByBranchWise(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.GetClientScheduledDetailsByBranchWise(inputs);
        }
        public async Task<IEnumerable<ScheduledData>> GetClientScheduledDetails(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.GetClientScheduledDetails(inputs);
        }
        public async Task<IEnumerable<ScheduledData>> GetInvoiceGenerationDetails(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.GetInvoiceGenerationDetails(inputs);
        }
        public async Task<IEnumerable<UserBookingInvoiceReport>> GetUserInvoiceGenerationDetails(BookingHistorySearch inputs)
        {
            return await _unitOfWork.invoiceRepository.GetUserInvoiceGenerationDetails(inputs);
        }
        
        public async Task<IEnumerable<ScheduledData>> SearchClientScheduleReoprt(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.SearchClientScheduleReoprt(inputs);
        }
        public async Task<IEnumerable<ScheduledData>> SearchCaretakerPayHoursReoprt(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.invoiceRepository.SearchCaretakerPayHoursReoprt(inputs);
        }
        public async Task<IEnumerable<PaymentReportDetails>> SearchUserPaymentReport(PaymentReport inputs)
        {
            return await _unitOfWork.invoiceRepository.SearchUserPaymentReport(inputs);
        }
        public async Task<IEnumerable<AdminBookingNotification>> GetAdminNotification()
        {
            return await _unitOfWork.invoiceRepository.GetAdminNotification();
        }
        public async Task<IEnumerable<AdminBookingList>> GetAdminBookingList()
        {
            return await _unitOfWork.invoiceRepository.GetAdminBookingList();
        }
        public async Task<IEnumerable<AdminSchedulingList>> GetAdminSchedulingList()
        {
            return await _unitOfWork.invoiceRepository.GetAdminSchedulingList();
        }
        public async Task<AdminBookingDetails> GetBookingDetailsById(int bookingId)
        {
            return await _unitOfWork.invoiceRepository.GetBookingDetailsById(bookingId);
        }
        public async Task<AdminSchedulingDetails> GetSchedulingDetailsById(int schedulingId)
        {
            return await _unitOfWork.invoiceRepository.GetSchedulingDetailsById(schedulingId);
        }
        public async Task<int> GenerateInvoice(InvoiceMail invoiceMail)
        {
            int invoiceNo =  await _unitOfWork.invoiceRepository.GenerateInvoice(invoiceMail);

            //if (invoiceNo > 0)
            //{
            //    Task.Factory.StartNew(() => { SendPaymentInvoiceToUser(invoiceMail , invoiceNo); });
               
            //}
            return invoiceNo;
        }
        public async Task<int> AddPaymentInvoiceDetails(InvoiceSearchInpts invoiceMail)
        {
            int invoiceNo = await _unitOfWork.invoiceRepository.AddPaymentInvoiceDetails(invoiceMail);

            //if (invoiceNo > 0)
            //{
            //    Task.Factory.StartNew(() => { SendPaymentInvoiceToUser(invoiceMail , invoiceNo); });

            //}
            return invoiceNo;
        }
        public Task SendPaymentInvoiceToUser(InvoiceMail invoiceMail)
        {
            SendPaymentInvoice(invoiceMail);
            return null;
        }
        public async Task<string> SavePublicUserPaymentInvoice(PublicUserPaymentInvoiceInfo invoiceData)
        {
            return await _unitOfWork.invoiceRepository.SavePublicUserPaymentInvoice(invoiceData);
        }
        private async Task SendPaymentInvoice(InvoiceMail invoiceMail)
        {

            UserPaymentInvoiceModel invoiceDetails = _unitOfWork.invoiceRepository.GetUserPaymentInvoiceDetails(invoiceMail.InvoiceNo).Result;
            string name = invoiceDetails.UserName + "_" + invoiceDetails.InvoiceNumber;
            var branchDetails = await _pcmsService.GetBranchByUserId(invoiceDetails.UserId);
            EmailInput input = new EmailInput
            {
                EmailId = invoiceMail.EmailId,
                UserName = invoiceDetails.UserName,
                Subject = "Tranquil care - Payment link",
                EmailType = EmailType.Invoice,
                Body = GetPaymentLinkEmailBody(invoiceDetails, invoiceMail.SiteUrl),
                Attachment= new Attachment(new MemoryStream(invoiceMail.Attachment), name+".pdf"),
                EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration(),
                EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Invoice, branchDetails.BranchId)
            };

          //  input.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
          //  input.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Invoice).Result;
            await _notificationService.SendEMail(input);           
        }
        private string GetPaymentLinkEmailBody(UserPaymentInvoiceModel invoiceDetails, string siteUrl)
        {
            try
            {
                string WelcomeMsg = "Booking Payment";
                string MailMsg = @"Thank you for the order and your overwhelmed support!<br><br>
                                                        Please click the 'Procced to Pay' button to make the payment for your booking on " + invoiceDetails.BookingDate + ".";
                string Mailcontent = @" <table style='font - family: Roboto, Tahoma, Arial, Helvetica, sans - serif; font - weight: 300; color: #6b6b6b; margin: 7px auto; font-size: 1em;' cellpadding='5'>
														    <tr>
														        <td style='width: 150px'>Amount</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + invoiceDetails.CurrencySymbol + " " + invoiceDetails.Amount.ToString("0.00") + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Tax </td>        
                                                                <td>:</td>          
                                                                <td> " + invoiceDetails.CurrencySymbol + " " + invoiceDetails.TaxAmount.ToString("0.00") + @" </td>             
                                                            </tr>   
                                                            <tr>     
                                                                <td> Total Amount </td>        
                                                                <td>:</td>          
                                                                <td> " + invoiceDetails.CurrencySymbol + " " + invoiceDetails.TotalAmount.ToString("0.00") + @" </td>             
                                                            </tr>  
                                                        </table>
                                                    <div></div>
                                                 <div style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif; text-align: center;'>
                                                 <a href = '" + siteUrl + @"publicuser/invoicepayments?invoice=" + invoiceDetails.InvoiceNumber.ToString() + @"' style = 'display:inline-block;background-color:#37abc8;color:#ffffff;font-size:1.2em;font-weight:300;text-decoration:none;padding:13px 25px 13px 25px;border-radius:10px' target = '_blank'> Proceed to Pay<a></div>";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
                body = string.Format(body, WelcomeMsg, invoiceDetails.UserName, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch(Exception ex)
            {
                int i = 0;
            }
            return string.Empty;
        }
    }
}
