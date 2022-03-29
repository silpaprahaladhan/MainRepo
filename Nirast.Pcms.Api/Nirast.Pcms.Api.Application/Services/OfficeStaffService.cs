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
    public class OfficeStaffService : IOfficeStaffService
    {
        IUnitOfWork _unitOfWork;
        private INotificationService _notificationService;
        private IPCMSService _pcmsService;
        public OfficeStaffService(IUnitOfWork unitOfWork, INotificationService notificationService, IPCMSService pCMSService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _pcmsService = pCMSService;
        }
        /// <summary>
        /// method to add office staff details
        /// </summary>
        /// <param name="officeStaff"></param>
        /// <returns></returns>
        public async Task<int> AddOfficeStaff(OfficeStaffRegistration officeStaff)
        {
            int UserId = officeStaff.UserId;
            int userId= await _unitOfWork.OfficeStaffReposoitory.AddOfficeStaff(officeStaff);
            if (UserId == 0 && userId != 0)
            {
                await Task.Factory.StartNew(() => { SendEmailAfterofficeStaffReg(officeStaff); });
            }
            return userId;
        }

        private async Task<bool> SendEmailAfterofficeStaffReg(OfficeStaffRegistration booking)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
           
            inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

            var result = await _unitOfWork.DesignationRepository.RetrieveDesignationById(booking.DesignationId??0);

            inputs.Subject = "Tranquil Care Registration Status";
            inputs.EmailType = EmailType.Registration;
            inputs.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
            var branchDetails = await _pcmsService.GetBranchByUserId(booking.UserId);
            inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration, branchDetails.BranchId).Result;
            //inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration).Result;
            string WelcomeMsg = "Office Staff registered";
            string MailMsg = "New office staff registered.<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
									    <tr>
									        <td style='width: 150px'>Name</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + booking.FirstName + ' ' + booking.LastName + @" </td>     
                                       </tr>     
                                       <tr>     
                                           <td> Designation </td>        
                                           <td>:</td>          
                                           <td> " + result.Designation + @" </td>             
                                       </tr>   
                                                        </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = booking.SiteURL;
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

        /// <summary>
        /// method to delete office staff details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> DeleteOfficeStaffProfile(int id)
        {
            return await _unitOfWork.OfficeStaffReposoitory.DeleteOfficeStaffDetails(id);
        }

        /// <summary>
        /// method to get all office staff details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OfficeStaffRegistration>> GetAllOfficeStaffDetails()
        {
            return await _unitOfWork.OfficeStaffReposoitory.GetOfficeStaffDetails();
        }

        /// <summary>
        /// method to get all office staff details
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OfficeStaffRegistration>> GetAllOfficeStaffDetailsByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.OfficeStaffReposoitory.GetOfficeStaffDetailsByLocation(inputs);
        }

        /// <summary>
        /// method to get office staff details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OfficeStaffRegistration> GetOfficeStaffProfile(int id)
        {
            return await _unitOfWork.OfficeStaffReposoitory.GetOfficeStaffDetails(id);
        }
    }
}
