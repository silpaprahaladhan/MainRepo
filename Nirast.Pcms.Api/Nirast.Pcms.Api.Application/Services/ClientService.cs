using Newtonsoft.Json;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Services;
using Nirast.Pcms.Api.Sdk.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Application.Services
{
    public class ClientService : IClientService
    {
        IUnitOfWork _unitOfWork;
        private INotificationService _notificationService;
        private IPCMSService _pcmsService;
        public ClientService(IUnitOfWork unitOfWork, INotificationService notificationService, IPCMSService pCMSService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _pcmsService = pCMSService;
        }
        /// <summary>
        /// To get client details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ClientDetails>> GetAllClientDetails()
        {
            return await _unitOfWork.clientRepository.GetClientDetails();
        }
        public async Task<IEnumerable<ClientDetails>> GetAllClientDetailsByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.clientRepository.GetClientDetailsByLocation(inputs);
        }


        public async Task<IEnumerable<ClientDetails>> GetAllFranchiseDetailsByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.clientRepository.GetFranchiseDetailsByLocation(inputs);
        }

        public async Task<IEnumerable<UsersDetails>> GetUserTypeId()
        {
            return await _unitOfWork.clientRepository.GetUserTypeId();
        }
        /// <summary>
        /// To get client details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduledData>> GetAllScheduleLogDetails()
        {
            return await _unitOfWork.clientRepository.GetAllScheduleLogDetails();
        }
        public async Task<IEnumerable<LoginLog>> GetLoginLogDetailsByTypeId(int typeId)
        {
            return await _unitOfWork.clientRepository.GetLoginLogDetailsByTypeId(typeId);
        }
        public async Task<IEnumerable<RejectedCaretaker>> GetAllScheduleRejectedCaretaker(BookingHistorySearch bookingHistorySearch)
        {
            return await _unitOfWork.clientRepository.GetAllScheduleRejectedCaretaker(bookingHistorySearch);
        }

        public async Task<ClientDetails> GetClientDetailsByID(int clientId)
        {
            return await _unitOfWork.clientRepository.GetClientDetailsByID(clientId);
        }
        public async Task<ClientDetails> GetFranchiseDetailsByID(int clientId)
        {
            return await _unitOfWork.clientRepository.GetFranchiseDetailsByID(clientId);
        }
        public async Task<IEnumerable<ScheduledData>> GetAllScheduledetails(CalenderEventInput calenderEventInput)
        {
            return await _unitOfWork.clientRepository.GetAllScheduledetails(calenderEventInput);
        }

        public async Task<int> DeleteSchedule(ScheduleDeleteData deleteData)
        {
            ScheduledData scheduledData = await _unitOfWork.clientRepository.GetSchdeuleDetaildById(deleteData.ScheduleId);
            deleteData.CareTakerName = scheduledData.CareTakerName;
            deleteData.OldData = JsonConvert.SerializeObject(scheduledData);
            scheduledData.SiteURL = deleteData.SiteURL;
            int Id = await _unitOfWork.clientRepository.DeleteSchedule(deleteData);

            Task.Factory.StartNew(() => { SendEmailAfterDeleteSchedule(scheduledData); });
            return Id;
        }

        private async Task<bool> SendEmailAfterDeleteSchedule(ScheduledData data)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();

            //get maild id for admin typeid=4

            inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

            //get maild id for caretaker
            string cc = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(data.CareTaker);
            ccAddressList.Add(cc);

            //get maild id for scheduled client
            cc = await _unitOfWork.clientRepository.GetEmailIdForClient(data.ClientId);
            if (cc != "")
            {
                ccAddressList.Add(cc);
            }

            inputs.EmailType = EmailType.Scheduling;
            inputs.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
            //inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Scheduling).Result;

            var branchDetails = await _pcmsService.GetBranchByUserId(data.ClientId);
            inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Scheduling, branchDetails.BranchId).Result;

            //inputs.Subject = " Schedule Deleted on " + data.Startdate + " for Client " + data.ClientName;
            inputs.Subject = "Schedule Deleted";
            string WelcomeMsg = "Schedule Deleted.";
            string MailMsg = "Schedule Details:<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>   
                                                 <tr>
											  <td style='width: 150px'>Caregiver Name</td>
                                                     <td style = 'width:10px'>:</td>     
                                                     <td> " + data.CareTakerName + @" </td>     
                                                 </tr>     
                                                 <tr>     
                                                     <td> Client Name </td>        
                                                     <td>:</td>          
                                                     <td> " + data.ClientName + @" </td>             
                                                 </tr>   
                                                 <tr>     
                                                     <td> Start Date & Time </td>        
                                                     <td>:</td>          
                                                     <td> " + data.Startdate + @" </td>             
                                                 </tr>  
                                                 <tr>     
                                                     <td> End Date & Time </td>        
                                                     <td>:</td>          
                                                     <td> " + data.Enddate + @" </td>             
                                                 </tr>  
                                                 <tr>     
                                                     <td> WorkMode </td>        
                                                     <td>:</td>          
                                                     <td> " + data.WorkModeName + " - " + data.Description + @" </td>             
                                                 </tr>                                                  
                                             </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = data.SiteURL;
            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string CompanyName = "Tranquil Care Inc.";
            string body = "";
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, WelcomeMsg, "", MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
            inputs.Body = body;
            return await _notificationService.SendEMail(inputs, ccAddressList);
        }
        public async Task<int> ModifyClientStatusById(int id, int status)
        {
            return await _unitOfWork.clientRepository.ModifyClientStatusById(id, status);
        }


        public async Task<int> ModifyFranchiseStatusById(int id, int status)
        {
            return await _unitOfWork.clientRepository.ModifyFranchiseStatusById(id, status);
        }
        public async Task<int> ChangeClientEmailStatus(int id, int emailstatus)
        {
            return await _unitOfWork.clientRepository.ChangeClientEmailStatus(id, emailstatus);
        }
        public async Task<int> UpdateClientInvoiceNumber(int clientId, int InvoiceNumber)
        {
            return await _unitOfWork.clientRepository.UpdateClientInvoiceNumber(clientId, InvoiceNumber);
        }

        public async Task<int> SaveClientCareTakerMapping(WorkShiftPayRates shiftPayRates)
        {
            return await _unitOfWork.clientRepository.SaveClientCareTakerMapping(shiftPayRates);
        }
        public async Task<int> SaveClientCareTakerPayRise(List<WorkShiftRates> workShiftRates)
        {
            return await _unitOfWork.clientRepository.SaveClientCareTakerPayRise(workShiftRates);
        }
        public async Task<int> SaveClientcategoryCareTakerPayRise(List<ClientCategoryRate> categoryRates)
        {
            return await _unitOfWork.clientRepository.SaveClientcategoryCareTakerPayRise(categoryRates);
        }
        public async Task<int> DeleteClientCareTakerMapping(ClientCaretakers clientcaretaker)
        {
            return await _unitOfWork.clientRepository.DeleteClientCareTakerMapping(clientcaretaker);
        }
        public async Task<int> SaveScheduleRejectedCareTaker(RejectedCaretaker careTaker)
        {
            return await _unitOfWork.clientRepository.SaveScheduleRejectedCareTaker(careTaker);
        }
        /// <summary>
        /// method to search client details
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ClientDetails>> SearchClient(ClientSearchInputs inputs)
        {
            return await _unitOfWork.clientRepository.ClientSearch(inputs);
        }
        public async Task<IEnumerable<ClientDetails>> GetClientInvoiceDetails()
        {
            return await _unitOfWork.clientRepository.GetClientInvoiceDetails();
        }
        public async Task<int> AddInvoiceDetails(ClientDetails clientInvoiceDetails)
        {
            return await _unitOfWork.clientRepository.AddInvoiceDetails(clientInvoiceDetails);
        }
        public async Task<int> GetClientFromUserId(int id)
        {
            return await _unitOfWork.clientRepository.GetClientFromUserId(id);
        }

        public async Task<IEnumerable<WorkShiftRates>> GetMappedCaretakerRates(int clientId, int caretakerId)
        {
            return await _unitOfWork.clientRepository.GetMappedCaretakerRates(clientId, caretakerId);
        }
        public async Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersLatestPayRiseRates(int clientId, int caretakerId)
        {
            return await _unitOfWork.clientRepository.GetMappedCaretakersLatestPayRiseRates(clientId, caretakerId);
        }
        public async Task<IEnumerable<WorkShiftRates>> GetMappedCaretakersPayRiseRatesByDate(PayriseData payriseData)
        {
            return await _unitOfWork.clientRepository.GetMappedCaretakersPayRiseRatesByDate(payriseData);
        }
        public async Task<IEnumerable<ClientCategoryRate>> GetCategoryClientPayRiseRates(int clientId)
        {
            return await _unitOfWork.clientRepository.GetCategoryClientPayRiseRates(clientId);
        }
        public async Task<IEnumerable<ClientCategoryRate>> GetClientInvoicePayRiseRatesonDateChange(int clientId, DateTime date)
        {
            return await _unitOfWork.clientRepository.GetClientInvoicePayRiseRatesonDateChange(clientId, date);
        }
        public async Task<ScheduledData> GetSchedulingLogDetailsById(int logId)
        {
            return await _unitOfWork.clientRepository.GetSchedulingLogDetailsById(logId);
        }
       

        public async Task<int> UpdateClientInvoice(InvoiceSearchInpts searchInpts)
        {
            return await _unitOfWork.clientRepository.UpdateClientInvoice(searchInpts);
        }
    }
}
