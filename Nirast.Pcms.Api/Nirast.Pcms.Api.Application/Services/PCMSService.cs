using Newtonsoft.Json;
using Nirast.Pcms.Ap.Application.Infrastructure;
using Nirast.Pcms.Api.Data.Helpers;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Services;
using Nirast.Pcms.Api.Sdk.UnitOfWork;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;
using static Nirast.Pcms.Api.Sdk.Entities.PublicUserCaretakerBooking;

namespace Nirast.Pcms.Api.Application.Services
{
    public class PCMSService : IPCMSService
    {
        IUnitOfWork _unitOfWork;
        private INotificationService _notificationService;
        public PCMSService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;

        }

        public async Task<int> AddCountry(Countries country)
        {
            return await _unitOfWork.CountryRepository.AddCountry(country);
        }

        public async Task<int> AddState(States state)
        {
            return await _unitOfWork.StateRepository.AddState(state);
        }

        public async Task<int> ForgotPassword(ForgotPasswordViewModel emailId)
        {
            Security security = new Security();
            var credentials = await _unitOfWork.UsersDetailsRepository.RetrievePassword(emailId.Email);
            string decryptedPassword = security.Decrypt(credentials.Password, ConfigurationManager.AppSettings["EncryptPassword"].ToString());
            EmailInput input = new EmailInput();
            input.Name = credentials.LoginName;
            input.UserName = emailId.Email;
            input.EmailId = emailId.Email;
            input.Password = decryptedPassword;
            input.EmailType = EmailType.Registration;
            input.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            var branchDetails = await GetBranchByUserId(1);//need a default from email
            input.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(input.EmailType, branchDetails.BranchId);
            //input.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(input.EmailType);
            input.Subject = "Forgot Password";
            input.Body = SendForgotPasswordEmailBody(emailId, credentials.LoginName, decryptedPassword);
            _notificationService.SendEMail(input);
            return await _unitOfWork.UsersDetailsRepository.ForgotPassword(emailId);
        }

        public async Task<IEnumerable<Countries>> RetrieveCountry(int countryId)
        {
            return await _unitOfWork.CountryRepository.RetrieveCountry(countryId);
        }

        public async Task<Countries> GetDefaultCountry()
        {
            return await _unitOfWork.CountryRepository.GetDefaultCountry();
        }

        public async Task<IEnumerable<States>> RetrieveStates(int stateId)
        {
            return await _unitOfWork.StateRepository.RetrieveStates(stateId);
        }
        public async Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRates(int caretakerId)
        {
            return await _unitOfWork.CareTakerRepository.GetCaretakerPayRiseRates(caretakerId);
        }
        public async Task<IEnumerable<CareTakerServices>> GetCaretakerPayRiseRatesonDateChange(DateTime date, int caretakerId)
        {
            return await _unitOfWork.CareTakerRepository.GetCaretakerPayRiseRatesonDateChange(date, caretakerId);
        }
        public async Task<int> SaveCareTakerPayRise(List<CareTakerServices> careTaker)
        {
            return await _unitOfWork.CareTakerRepository.SaveCareTakerPayRise(careTaker);
        }
        public async Task<int> AddCareTaker(CareTakerRegistrationModel careTakerRegistration)
        {
            int userId = await _unitOfWork.CareTakerRepository.AddCareTaker(careTakerRegistration);
            if (careTakerRegistration.UserId == 0 && userId != 0)
            {
                Task.Factory.StartNew(() => { SendEmailAfterCaretakerReg(careTakerRegistration); SendEmailTOAdminAndOfficeAfterCaretakerReg(careTakerRegistration); });
            }
            return userId;
        }

        public async Task<ClientDetails> AddFranchiseDetails(ClientDetails clientRegistration)
        {
            var clientDetails = await _unitOfWork.clientRepository.AddFranchiseDetails(clientRegistration);
            if (clientRegistration.ClientId == 0 &&
                clientDetails.ClientId != 0)
            {
                Task.Factory.StartNew(() => { SendEmailAfterClientReg(clientRegistration); });
            }
            return clientDetails;
        }

        private async Task SendEmailAfterCaretakerReg(CareTakerRegistrationModel caretaker)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
            inputs.EmailId = caretaker.EmailAddress;
            inputs.EmailType = Enums.EmailType.Registration;
            inputs.Subject = "Tranquil Care Registration Status";
            inputs.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
            var branchDetails = await GetBranchByUserId(caretaker.UserId);
            inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration, branchDetails.BranchId).Result;
            inputs.Body = GetRegistrationEmailBody(caretaker);
            await _notificationService.SendEMail(inputs);
        }

        private string GetRegistrationEmailBody(CareTakerRegistrationModel caretaker)
        {
            try
            {
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                string pswd = StringCipher.Decrypt(caretaker.Password, encryptionPassword);

                string WelcomeMsg = "New Caregiver registered.";
                string MailMsg = "You have been registered successfully.<br/>";
                string Mailcontent = @"<strong>Please find the account details below:</strong>
                                                        <br>
                                                       <table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.1em'>
														    <tr>
														        <td style='width: 150px'> First Name </td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + caretaker.FirstName + @" </td>     
                                                            </tr> 
                                                            <tr>
														        <td style='width: 150px'> Last Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + caretaker.LastName + @" </td>     
                                                            </tr>   
                                                            <tr>     
                                                                <td> Email Address </td>        
                                                                <td>:</td>          
                                                                <td> " + caretaker.EmailAddress + @" </td>             
                                                            </tr>             
                                                            <tr>             
                                                                <td> Password </td>             
                                                                <td>:</td>               
                                                                <td> " + pswd + @" </td>                  
                                                            </tr>                                     
                                                            <tr>                    
                                                                <td> Phone </td>                    
                                                                <td>:</td>                      
                                                                <td> " + caretaker.PrimaryPhoneNo + @" </td>                      
                                                            </tr>                      
                                                      </table>";
                Mailcontent += (caretaker.SendThroughFax == true) ? "<strong>Note : You are opted to send documents through fax.</strong>" : "";
                Mailcontent += "";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = caretaker.SiteURL;
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
                body = string.Format(body, WelcomeMsg, caretaker.FirstName, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private async Task SendEmailTOAdminAndOfficeAfterCaretakerReg(CareTakerRegistrationModel caretaker)
        {
            // var officeStaffMailIds = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
            var branchDetails = await GetBranchByUserId(caretaker.UserId);
            EmailInput inputs = new EmailInput
            {
                EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin(),
                EmailType = Enums.EmailType.Registration,
                Subject = "Tranquil Care Registration Status",
                EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result,
                EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration, branchDetails.BranchId).Result,
            
                Body = GetRegistrationEmailBodyForAdmin(caretaker)
            };
            await _notificationService.SendEMail(inputs);
        }

        private string GetRegistrationEmailBodyForAdmin(CareTakerRegistrationModel caretaker)
        {
            try
            {
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                string pswd = StringCipher.Decrypt(caretaker.Password, encryptionPassword);

                string WelcomeMsg = "New Caregiver registered.";
                string MailMsg = "<br/>";
                string Mailcontent = @"<strong>Please find the account details below:</strong>
                                                        <br>
                                                       <table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.1em'>
														    <tr>
														        <td style='width: 150px'>First Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + caretaker.FirstName + @" </td>     
                                                            </tr> 
                                                            <tr>
														        <td style='width: 150px'>Last Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + caretaker.LastName + @" </td>     
                                                            </tr> 
                                                            <tr>                    
                                                                <td> Phone </td>                    
                                                                <td>:</td>                      
                                                                <td> " + caretaker.PrimaryPhoneNo + @" </td>                      
                                                            </tr> 
                                                            <tr>     
                                                                <td> Email Address </td>        
                                                                <td>:</td>          
                                                                <td> " + caretaker.EmailAddress + @" </td>             
                                                            </tr>                                               
                                                      </table>";
                Mailcontent += (caretaker.SendThroughFax == true) ? "<strong>Note : The caretaker opted to send documents through fax.</strong>" : "";
                Mailcontent += "";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = caretaker.SiteURL;
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
                body = string.Format(body, WelcomeMsg, "Admin", MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                int i = 0;
                return string.Empty;
            }
        }
        public async Task<int> UpdateUserInvoiceNumber(int userid, int invoicenumber)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateUserInvoiceNumber(userid, invoicenumber);
        }

        public async Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsById(string flag, string value)
        {
            return await _unitOfWork.UsersDetailsRepository.GetUsersDetailsById(flag, value);
        }
        public async Task<IEnumerable<PublicUserRegistration>> GetUsersDetailsByLocation(string flag, string value, LocationSearchInputs inputs)
        {
            return await _unitOfWork.UsersDetailsRepository.GetUsersDetailsByLocation(flag, value, inputs);
        }
        public async Task<int> AddPublicUser(PublicUserRegistration usersDetails)
        {
            int userId = await _unitOfWork.UsersDetailsRepository.AddPublicUser(usersDetails);
            if (userId != 0)
            {
                Task.Factory.StartNew(() => { SendEmailAfterInsertUser(usersDetails); });
            }
            return userId;
        }

        private async Task<bool> SendEmailAfterInsertUser(PublicUserRegistration usersDetails)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
            inputs.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
            var branchDetails = await GetBranchByUserId(usersDetails.UserRegnId);
            inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration, branchDetails.BranchId).Result;
            //inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration).Result;
            inputs.EmailId = usersDetails.EmailAddress;
            inputs.Subject = "Tranquil Care Registration Status";
            inputs.Body = GetEmailBody(usersDetails);
            inputs.EmailType = EmailType.Registration;

            return await _notificationService.SendEMail(inputs, ccAddressList);
        }

        private string GetEmailBody(PublicUserRegistration usersDetails)
        {
            try
            {
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
                string pswd = StringCipher.Decrypt(usersDetails.Password, encryptionPassword);
                string WelcomeMsg = "New Public user registered.";
                string MailMsg = "You have been registered successfully.<br/>";
                string Mailcontent = @"<strong>Please find the account details below:</strong>
                                                        <br>
                                                       <table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.1em'>
														    <tr>
														        <td style='width: 150px'> First Name </td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + usersDetails.FirstName + @" </td>     
                                                            </tr> 
														    <tr>
														        <td style='width: 150px'> Last Name </td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + usersDetails.LastName + @" </td>     
                                                            </tr> 
                                                            <tr>     
                                                                <td> Email Address </td>        
                                                                <td>:</td>          
                                                                <td> " + usersDetails.EmailAddress + @" </td>             
                                                            </tr>    
                                                            <tr>             
                                                                <td> Password </td>             
                                                                <td>:</td>               
                                                                <td> " + pswd + @" </td>                  
                                                            </tr>   
                                                            <tr>                  
                                                                <td> Location </td>                  
                                                                <td>:</td>                    
                                                                <td> " + usersDetails.Location + @" </td>                    
                                                            </tr>    
                                                            <tr>                    
                                                                <td> Phone </td>                    
                                                                <td>:</td>                      
                                                                <td> " + usersDetails.PrimaryPhoneNo + @" </td>                      
                                                            </tr>  
                                                      </table>";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = usersDetails.SiteURL;
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
                body = string.Format(body, WelcomeMsg, usersDetails.FirstName, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                //_logger.Error(ex, "Error while generating mail body");
                return string.Empty;
            }
        }

        //private async Task<bool> SendEmailToAdminAndOfficeAfterInsertUser(PublicUserRegistration booking)
        //{
        //    EmailInput inputs = new EmailInput();
        //    List<string> ccAddressList = new List<string>();
        //    //get maild id for admin typeid=4
        //    inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

        //    //get maild id for office staff typeid=5
        //    var officeStaffEmails = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
        //    if (officeStaffEmails.Count > 0)
        //    {
        //        ccAddressList.AddRange(officeStaffEmails);
        //    }
        //    inputs.EmailType = EmailType.Registration;
        //    inputs.Subject = "Tranquil Care Registration Status";
        //    inputs.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
        //    inputs.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration).Result;
        //    string WelcomeMsg = "New Public user registered.";
        //    string MailMsg = "Public User Details.<br/>";
        //    string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
        //	    <tr>
        //	        <td style='width: 150px'>First Name</td>
        //                                   <td style = 'width:10px'>:</td>     
        //                                   <td> " + booking.FirstName + @" </td>     
        //                               </tr> 
        //		   <tr>
        //	        <td style='width: 150px'>Last Name</td>
        //                                   <td style = 'width:10px'>:</td>     
        //                                   <td> " + booking.LastName + @" </td>     
        //                               </tr> 
        //                               <tr>     
        //                                   <td> EmailAddress </td>        
        //                                   <td>:</td>          
        //                                   <td> " + booking.EmailAddress + @" </td>             
        //                               </tr>   
        //                               <tr>     
        //                                   <td> Location </td>        
        //                                   <td>:</td>          
        //                                   <td> " + booking.Location + @" </td>             
        //                               </tr> 
        //                               <tr>     
        //                                   <td> Phone </td>        
        //                                   <td>:</td>          
        //                                   <td> " + booking.PrimaryPhoneNo + @" </td>             
        //                               </tr> 
        //                             </table>";
        //    string ContactNo = "1-800-892-6066";
        //    string RegardsBy = "Tranquil Care Inc.";
        //    string siteUrl = booking.SiteURL;
        //    string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
        //    string CompanyName = "Tranquil Care Inc.";
        //    string body = "";
        //    string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
        //    var sr = new StreamReader(sd);
        //    body = sr.ReadToEnd();
        //    //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
        //    body = string.Format(body, WelcomeMsg, "Admin", MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
        //    inputs.Body = body;
        //    return await _notificationService.SendEMail(inputs, ccAddressList);
        //}

        public async Task<UserCredential> RetrievePassword(string emailId)
        {
            return await _unitOfWork.UsersDetailsRepository.RetrievePassword(emailId);
        }

        public async Task<int> ChangePassword(ChangePassword changePassword)
        {
            return await _unitOfWork.UsersDetailsRepository.ChangePassword(changePassword);
        }
        public async Task<int> EditUserProfile(PublicUserRegistration UsersDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.EditUserProfile(UsersDetails);
        }
        public async Task<int> EditCardDetails(PublicUserRegistration UsersDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.EditCardDetails(UsersDetails);
        }
        public async Task<int> UpdateUserProfilePic(PublicUserRegistration UsersDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateUserProfilePic(UsersDetails);
        }
        public async Task<int> VerifyUserProfile(VerifyUserAccount VerifyUser)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateUserVerification(VerifyUser);
        }
        public async Task<int> AddCity(Cities city)
        {
            return await _unitOfWork.CityRepository.AddCity(city);
        }
        //22-02-2022
        public async Task<IEnumerable<Cities>> RetrieveCities(string flag, string value)
        {
            return await _unitOfWork.CityRepository.RetrieveCities(flag, value);
        }
        public async Task<IEnumerable<Cities>> RetrieveBranches(string flag, string value)
        {
            return await _unitOfWork.CityRepository.RetrieveBranches(flag, value);
        }

        public async Task<IEnumerable<Cities>> RetrieveBranchesById(string flag, string value)
        {
            return await _unitOfWork.CityRepository.RetrieveBranchesById(flag, value);
        }
        public async Task<IEnumerable<Cities>> RetrieveBranchById(int id)
        {
            return await _unitOfWork.CityRepository.RetrieveBranchById(id);
        }
        public async Task<IEnumerable<Cities>> RetrieveCountry(string flag, string value)
        {
            return await _unitOfWork.CityRepository.RetrieveCountry(flag, value);
        }

        public async Task<IEnumerable<Cities>> Retrievestates(string flag, string value)
        {
            return await _unitOfWork.CityRepository.Retrievestates(flag, value);
        }



        public async Task<IEnumerable<Cities>> RetrievecityDetails(string flag, string value)
        {
            return await _unitOfWork.CityRepository.RetrievecityDetails(flag, value);
        }
        public async Task<IEnumerable<Cities>> RetrieveAllBranches()
        {
            return await _unitOfWork.CityRepository.RetrieveAllBranches();
        }
        public async Task<int> DeleteCity(int cityId)
        {
            return await _unitOfWork.CityRepository.DeleteCity(cityId);
        }

        public async Task<IEnumerable<States>> GetStatesById(int countryId)
        {
            return await _unitOfWork.StateRepository.GetStatesById(countryId);
        }

        public async Task<IEnumerable<Cities>> GetCityById(int stateId)
        {
            return await _unitOfWork.CityRepository.GetCityByStateId(stateId);
        }
        public async Task<int> AddCategory(CategoryModel category)
        {
            return await _unitOfWork.CategoryRepository.AddCategory(category);
        }

        public async Task<IEnumerable<CategoryModel>> RetrieveCategory(string flag, string value)
        {
            return await _unitOfWork.CategoryRepository.RetrieveCategory(flag, value);
        }


        public async Task<int> DeleteCategory(int categoryId)
        {
            return await _unitOfWork.CategoryRepository.DeleteCategory(categoryId);
        }


        public async Task<int> AddOrientation(Orientation Orientation)
        {
            return await _unitOfWork.OrientationRepository.AddOrientation(Orientation);
        }

        public List<Orientation> RetrieveOrientation(int OrientationId)
        {
            return _unitOfWork.OrientationRepository.RetrieveOrientation(OrientationId);
        }

        public async Task<int> DeleteOrientation(int OrientationId)
        {
            return await _unitOfWork.OrientationRepository.DeleteOrientation(OrientationId);
        }

        public async Task<int> AddDesignation(DesignationDetails Designation)
        {
            return await _unitOfWork.DesignationRepository.AddDesignation(Designation);
        }

        public async Task<IEnumerable<DesignationDetails>> RetrieveDesignation(int id)
        {
            return await _unitOfWork.DesignationRepository.RetrieveDesignation(id);
        }

        public async Task<int> DeleteDesignation(int DesignationId)
        {
            return await _unitOfWork.DesignationRepository.DeleteDesignation(DesignationId);
        }

        public async Task<int> AddQualification(QualificationDetails Qualification)
        {
            return await _unitOfWork.QualificationRepository.AddQualification(Qualification);
        }

        public async Task<IEnumerable<QualificationDetails>> RetrieveQualification(int QualificationId)
        {
            return await _unitOfWork.QualificationRepository.RetrieveQualification(QualificationId);
        }

        public async Task<int> DeleteQualification(int QualificationId)
        {
            return await _unitOfWork.QualificationRepository.DeleteQualification(QualificationId);
        }

        public async Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakers(int status)
        {
            return await _unitOfWork.CareTakerRepository.SelectRegisteredCaretakers(status);
        }
        
         public async Task<IEnumerable<CareTakerRegistrationModel>> SelectRegisteredCaretakersByLocation(int status,LocationSearchInputs inputs)
        {
            return await _unitOfWork.CareTakerRepository.SelectRegisteredCaretakersByLocation(status,inputs);
        }

        public async Task<IEnumerable<CareTakerServices>> RetrieveCaregiverServices()
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCaregiverServices();
        }


        public async Task<IEnumerable<CareTakerRegistrationModel>> SelectCaretakers(int status)
        {
            return await _unitOfWork.CareTakerRepository.SelectCaretakers(status);
        }

        public async Task<int> RejectCaretakerApplication(RejectCareTaker rejectCareTaker)
        {
            int Id = await _unitOfWork.CareTakerRepository.RejectCaretakerApplication(rejectCareTaker);
            Task.Factory.StartNew(() => { SendEmailAfterCaretakerReject(rejectCareTaker); });
            return Id;
        }

        private async Task<bool> SendEmailAfterCaretakerReject(RejectCareTaker rejectCareTaker)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
            //get maild id for admin
            //string cc = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();
            string cc = "info@tranquilcare.ca";
            ccAddressList.Add(cc);

            //get caretaker details to send
            CareTakerRegistrationModel result = await _unitOfWork.CareTakerRepository.RetrieveCaretakerDetails(rejectCareTaker.Userid);
            inputs.EmailId = result.EmailAddress;

            //get maild id for office staff typeid=5
            //var officeStaffEmails = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
            //if (officeStaffEmails.ToList().Count > 0)
            //{
            //    ccAddressList.AddRange(officeStaffEmails);
            //}
            string serviceNames = string.Empty;
            foreach (var item in result.CareTakerServices)
            {
                serviceNames += item.ServiceName + ",";
            }
            inputs.Subject = "Application Rejected";
            inputs.EmailType = Enums.EmailType.Registration;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            var branchDetails = await GetBranchByUserId(rejectCareTaker.Userid);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);
            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            string welcomeMsg = "Application Rejected.";
            string mailMsg = "Your profile has been rejected.<br/>";
            inputs.Body = GetEmailsBody(welcomeMsg, mailMsg, result.FirstName, result.LastName, serviceNames, result.CategoryName, result.TotalExperience.ToString(), result.PrimaryPhoneNo, result.EmailAddress, rejectCareTaker.SiteURL);
            return await _notificationService.SendEMail(inputs, ccAddressList);
        }

        private string GetEmailsBody(string welcomeMsg, string mailMsg, string firstName, string lastName, string serviceName, string catagory,
                                            string experience, string phno, string email, string siteURL)
        {
            try
            {
                string Mailcontent = @"<strong>Please find the account details below:</strong>
                                                        <br>
                                                       <table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.1em'>
														    <tr>
														        <td style='width: 150px'>First Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + firstName + @" </td>     
                                                            </tr>
                                                            <tr>
														        <td style='width: 150px'>Last Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + lastName + @" </td>     
                                                            </tr> 
                                                            <tr>     
                                                                <td> Service </td>        
                                                                <td>:</td>          
                                                                <td> " + serviceName + @" </td>             
                                                            </tr>             
                                                            <tr>             
                                                                <td> Category </td>             
                                                                <td>:</td>               
                                                                <td> " + catagory + @" </td>                  
                                                            </tr>                  
                                                            <tr>                  
                                                                <td> Experience </td>                  
                                                                <td>:</td>                    
                                                                <td> " + experience + @" </td>                    
                                                            </tr>                    
                                                            <tr>                    
                                                                <td> Phone </td>                    
                                                                <td>:</td>                      
                                                                <td> " + phno + @" </td>                      
                                                            </tr>  
                                                            <tr>                    
                                                                <td> Email </td>                    
                                                                <td>:</td>                      
                                                                <td> " + email + @" </td>                      
                                                            </tr> 
                                                      </table>";
                string ContactNo = "1-800-892-6066";
                string RegardsBy = "Tranquil Care Inc.";
                string siteUrl = siteURL;
                string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
                string CompanyName = "Tranquil Care Inc.";
                string body = "";
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
                body = string.Format(body, welcomeMsg, "", mailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<int> DeleteCaretaker(int userRegnId)
        {
            return await _unitOfWork.CareTakerRepository.DeleteCaretaker(userRegnId);
        }

        public async Task<int> ApproveCaretaker(ApproveCaretaker approveCaretaker)
        {
            int Id = await _unitOfWork.CareTakerRepository.ApproveCaretaker(approveCaretaker);
            await Task.Factory.StartNew(() => { SendEmailAfterApproval(approveCaretaker); });
            return Id;
        }

        private async Task<bool> SendEmailAfterApproval(ApproveCaretaker approveCaretaker)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
            //get maild id for admin
            string cc = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();
            ccAddressList.Add(cc);

            //get caretaker details to send
            CareTakerRegistrationModel result = await _unitOfWork.CareTakerRepository.RetrieveCaretakerDetails(approveCaretaker.CareTakerId);
            inputs.EmailId = result.EmailAddress;

            //get maild id for office staff typeid=5
            //var officeStaffEmails = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
            //if (officeStaffEmails.ToList().Count > 0)
            //{
            //    ccAddressList.AddRange(officeStaffEmails);
            //}
            string serviceNames = string.Empty;
            foreach (var item in result.CareTakerServices)
            {
                serviceNames += item.ServiceName + ",";
            }
            serviceNames = serviceNames.Remove(serviceNames.Length - 1);
            inputs.Subject = "Application Approved";
            string welcomeMsg = "Application Approved.";
            string mailMsg = "Your profile has been approved.<br/>";
            inputs.Body = GetEmailsBody(welcomeMsg, mailMsg, result.FirstName, result.LastName, serviceNames, result.CategoryName, result.TotalExperience.ToString(),
                                        result.PrimaryPhoneNo, result.EmailAddress, approveCaretaker.SiteURL);
            inputs.EmailType = Enums.EmailType.Registration;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            var branchDetails = await GetBranchByUserId(approveCaretaker.CareTakerId);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);

            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            return await _notificationService.SendEMail(inputs, ccAddressList);
        }

        //public List<RegistredCaretakersList> SelectNewAppliedCaretakers()
        //{
        //    return _unitOfWork.CareTakerRepository.SelectNewAppliedCaretakers();
        //}

        public async Task<CareTakerRegistrationModel> RetrieveCaretakerDetails(int caretakerId)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCaretakerDetails(caretakerId);
        }

        public async Task<string> CaretakerProfileId()
        {
            return await _unitOfWork.CareTakerRepository.CaretakerProfileId();
        }

        public async Task<int> SaveCaretakerBooking(CaretakerBookingModel caretakerBooking)
        {
            int id = await _unitOfWork.PatientRepository.SaveCaretakerBooking(caretakerBooking);
            Task.Factory.StartNew(() => { SendEmailAfterBooking(caretakerBooking); });
            return id;
        }

        private async Task<bool> SendEmailAfterBooking(CaretakerBookingModel booking)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
            inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(booking.BookedUserId);

            var result = await _unitOfWork.UsersDetailsRepository.GetUsersDetailsById("UserRegnId", booking.BookedUserId.ToString());

            string cc = "info@tranquilcare.ca";
            ccAddressList.Add(cc);
            //string cc = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(booking.CareTakerId);
            //ccAddressList.Add(cc);

            //get caretaker details to send
            CareTakerRegistrationModel results = await _unitOfWork.CareTakerRepository.RetrieveCaretakerDetails(booking.CareTakerId);

            //get maild id for office staff typeid=5

            //get maild id for admin
            cc = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

            inputs.EmailType = EmailType.Booking;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            var branchDetails = await GetBranchByUserId(booking.CareTakerId);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);
            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            inputs.Subject = "Booking Notification";
            string WelcomeMsg = "Booking Notification";
            string MailMsg = "Following is the booking details.<br/>";
            string Mailcontent = "";
            if (booking.IsFullDay)
            {
                Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
									    <tr>
									        <td style='width: 150px'>Booked by</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + result.First().FirstName + " " + result.First().LastName + @" </td>     
                                       </tr>     
                                       
                                       <tr>     
                                           <td> Booked Date </td>        
                                           <td>:</td>          
                                           <td> " + DateTime.Now.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking From </td>        
                                           <td>:</td>          
                                           <td> " + booking.BookingStartTime.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking To </td>        
                                           <td>:</td>          
                                           <td> " + booking.BookingEndTime.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                     </table>";

            }
            else
            {
                Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
									    <tr>
									        <td style='width: 150px'>Booked by</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + result.First().FirstName + " " + result.First().LastName + @" </td>     
                                       </tr>     
                                       
                                       <tr>     
                                           <td> Booked Date </td>        
                                           <td>:</td>          
                                           <td> " + DateTime.Now.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking From </td>        
                                           <td>:</td>          
                                           <td> " + booking.BookingStartTime.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                       <tr> 
                                           <td> Booking To</td>        
                                           <td>:</td>          
                                           <td> " + booking.BookingEndTime.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                        <tr>  
                                           <td>Shift</td>        
                                           <td>:</td>          
                                           <td> " + booking.BookingStartTime.ToString("h: mm tt") + "-" + booking.BookingEndTime.ToString("h: mm tt") + @" </td>             
                                       </tr> 
                                     </table>";

            }

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

        public async Task<int> AddWorkShift(WorkShifts workShift)
        {
            return await _unitOfWork.WorkShiftRepository.AddWorkShift(workShift);
        }

        public async Task<IEnumerable<WorkShifts>> RetrieveWorkShift(int workShiftId)
        {
            return await _unitOfWork.WorkShiftRepository.RetrieveWorkShift(workShiftId);
        }
        public async Task<int> DeleteWorkShift(int workShiftId)
        {
            return await _unitOfWork.WorkShiftRepository.DeleteWorkShift(workShiftId);
        }

        public async Task<int> AddTimeShift(ClientTimeShifts timeShift)
        {
            return await _unitOfWork.TimeShiftRepository.AddTimeShift(timeShift);
        }

        public async Task<int> DeleteClientShiftDetail(int TimeShiftId)
        {
            return await _unitOfWork.TimeShiftRepository.DeleteClientShiftDetail(TimeShiftId);
        }
        public async Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShift(int timeShiftId)
        {
            return await _unitOfWork.TimeShiftRepository.RetrieveTimeShift(timeShiftId);
        }
        public async Task<IEnumerable<ClientTimeShifts>> RetrieveTimeShiftByClientId(int clientId)
        {
            return await _unitOfWork.TimeShiftRepository.RetrieveTimeShiftByClientId(clientId);
        }

        public async Task<int> AddHoliday(Holidays holiday)
        {
            return await _unitOfWork.HolidayRepository.AddHoliday(holiday);

        }

        public async Task<int> OverrideHoliday()
        {
            return await _unitOfWork.HolidayRepository.OverrideHoliday();

        }

        public async Task<float> RetrieveHolidayPayDetails()
        {
            return await _unitOfWork.HolidayRepository.RetrieveHolidayPayDetails();
        }
        public async Task<float> RetrieveGetIntervalHours()
        {
            return await _unitOfWork.HolidayRepository.RetrieveGetIntervalHours();
        }

        public async Task<int> AddHolidayPay(Holidays holiday)
        {
            return await _unitOfWork.HolidayRepository.AddHolidayPay(holiday);
        }
        public async Task<int> AddIntervalHours(ClientTimeShifts shifts)
        {
            return await _unitOfWork.HolidayRepository.AddIntervalHours(shifts);
        }
        public async Task<int> DeleteHoliday(int id)
        {
            return await _unitOfWork.HolidayRepository.DeleteHoliday(id);
        }

        public async Task<IEnumerable<Holidays>> RetrieveHoliday(Holidays holidaySearchModel)
        {
            return await _unitOfWork.HolidayRepository.RetrieveHoliday(holidaySearchModel);
        }
        public async Task<int> CheckLoginNameExist(string LoginName)
        {
            return await _unitOfWork.UsersDetailsRepository.CheckLoginNameExist(LoginName);
        }
        public async Task<ClientDetails> AddClientDetails(ClientDetails clientRegistration)
        {
            var clientDetails = await _unitOfWork.clientRepository.AddClientDetails(clientRegistration);
            if (clientRegistration.ClientId == 0 &&
                clientDetails.ClientId != 0)
            {
                Task.Factory.StartNew(() => { SendEmailAfterClientReg(clientRegistration); });
            }
            return clientDetails;
        }
       
        private async Task<bool> SendEmailAfterClientReg(ClientDetails clientDetails)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();

            //get maild id for admin
            inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

            //get maild for office staff typeID=5
            //var officeStaffMailIds = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
            //if (officeStaffMailIds.ToList().Count > 0)
            //{
            //    ccAddressList.AddRange(officeStaffMailIds);
            //}
            inputs.EmailType = EmailType.Registration;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            var branchDetails = await GetBranchByUserId(clientDetails.ClientId);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);
            inputs.Subject = "New Client registered";
            string WelcomeMsg = "New Client registered.";
            string MailMsg = "<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
														    <tr>
														        <td style='width: 150px'>Client Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + clientDetails.ClientName + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Client Email </td>        
                                                                <td>:</td>          
                                                                <td> " + clientDetails.EmailId + @" </td>             
                                                            </tr>   
                                                            <tr>     
                                                                <td> Website </td>        
                                                                <td>:</td>          
                                                                <td> " + clientDetails.WebsiteAddress + @" </td>             
                                                            </tr>  
                                                            <tr>     
                                                                <td> Address </td>        
                                                                <td>:</td>          
                                                                <td> " + clientDetails.Address1 + @" </td>             
                                                            </tr>  
                                                        </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = clientDetails.SiteURL;
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

        public async Task<int> AddClientInvoiceDetails(InvoiceSearchInpts invoiceDetails)
        {
            return await _unitOfWork.clientRepository.AddClientInvoiceDetails(invoiceDetails);
        }
        public async Task<int> AddUserInvoiceDetails(InvoiceSearchInpts invoiceDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.AddUserInvoiceDetails(invoiceDetails);
        }

        public async Task<int> AddScheduledDetails(ScheduledData scheduledData)
        {
            if (scheduledData.Id != 0)
            {
                ScheduledData oldScheduledData = await _unitOfWork.clientRepository.GetSchdeuleDetaildById(scheduledData.Id);
                var tempData = MapScheduleData(oldScheduledData);
                scheduledData.OldData = JsonConvert.SerializeObject(tempData);
            }
            string message = string.Empty;
            int scheduleId = await _unitOfWork.clientRepository.AddScheduledDetails(scheduledData, out message);
            scheduledData.SchedulingId = scheduleId;
            var tempNewData = MapScheduleData(scheduledData);
            scheduledData.NewData = JsonConvert.SerializeObject(tempNewData);
            string warning = await _unitOfWork.clientRepository.AddScheduledDetailsAuditLog(scheduledData, message);
            int emailStatus = await _unitOfWork.clientRepository.GetClientEmailStatus(scheduledData.ClientId);
            if (0 != scheduleId)
            {
                Task.Factory.StartNew(() => { SendEmailAfterScheduling(scheduledData, emailStatus); });
            }
            return scheduleId;

        }
        public async Task<int> AddBookingDetails(PublicUserCaretakerBooking bookingData)
        {

            string message = string.Empty;
            int scheduleId = await _unitOfWork.UsersDetailsRepository.AddBookingDetails(bookingData, out message);
            bookingData.SchedulingId = scheduleId;
            if (0 != scheduleId)
            {
                Task.Factory.StartNew(() => { SendEmailAfterBooking(bookingData); });
            }
            return scheduleId;

        }
        private async Task<bool> SendEmailAfterBooking(PublicUserCaretakerBooking bookingData)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();
            inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(bookingData.PublicUserId);

            var result = await _unitOfWork.UsersDetailsRepository.GetUsersDetailsById("UserRegnId", bookingData.PublicUserId.ToString());


            string cc = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(bookingData.CareTaker);
            ccAddressList.Add(cc);

            //get caretaker details to send
            CareTakerRegistrationModel results = await _unitOfWork.CareTakerRepository.RetrieveCaretakerDetails(bookingData.CareTaker);

            //get maild id for office staff typeid=5
            //var officeStaffEmails = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
            //if (officeStaffEmails.Count > 0)
            //{
            //    ccAddressList.AddRange(officeStaffEmails);
            //}

            //get maild id for admin
            cc = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

            inputs.EmailType = EmailType.Booking;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
           // inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            var branchDetails = await GetBranchByUserId(bookingData.PublicUserId);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);
            inputs.Subject = "Booking Notification";


            string WelcomeMsg = "Booking Notification";
            string MailMsg = "Following is the booking details.<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
									    <tr>
									        <td style='width: 150px'>Booked by</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + result.First().FirstName + " " + result.First().LastName + @" </td>     
                                       </tr>     
                                        <tr>
									        <td style='width: 150px'>Caregiver Name</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + bookingData.CareTakerName + @" </td>     
                                       </tr>    
                                        <tr>
									        <td style='width: 150px'>Category</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + bookingData.ServiceTypeName + @" </td>     
                                       </tr>  
                                       
                                       <tr>     
                                           <td> Booked Date </td>        
                                           <td>:</td>          
                                           <td> " + DateTime.Now.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking From Date & Time </td>        
                                           <td>:</td>          
                                           <td> " + bookingData.Start.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking To Date & Time </td>        
                                           <td>:</td>          
                                           <td> " + bookingData.End.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                     </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = bookingData.SiteUrl;
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

        public async Task<IEnumerable<PublicUserCaretakerBooking>> GetAllBookingdetails(CalenderBookingEventInput calenderEventInput)
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllBookingdetails(calenderEventInput);
        }
        private TempScheduledData MapScheduleData(ScheduledData oldScheduledData)
        {


            TempScheduledData tempScheduleData = new TempScheduledData()
            {
                AddressId = oldScheduledData.AddressId,
                Amount = oldScheduledData.Amount,
                AuditLogActionType = oldScheduledData.AuditLogActionType,
                AuditLogType = oldScheduledData.AuditLogType,
                BuildingName = oldScheduledData.BuildingName,
                CareTaker = oldScheduledData.CareTaker,
                CareTakerName = oldScheduledData.CareTakerName,
                CareTakerType = oldScheduledData.CareTakerType,
                CareTakerTypeName = oldScheduledData.CareTakerTypeName,
                CityId = oldScheduledData.CityId,
                CityName = oldScheduledData.CityName,
                ClientId = oldScheduledData.ClientId,
                ClientName = oldScheduledData.ClientName,
                ClientSchedulingDate = oldScheduledData.ClientSchedulingDate,
                ContactPerson = oldScheduledData.ContactPerson,
                CountryId = oldScheduledData.CountryId,
                CountryName = oldScheduledData.CountryName,
                CurrencySymbol = oldScheduledData.CurrencySymbol,
                CustomTiming = oldScheduledData.CustomTiming,
                Description = oldScheduledData.Description,
                End = oldScheduledData.End,
                Enddate = oldScheduledData.Enddate,
                EndDateTime = oldScheduledData.EndDateTime,
                EndTime = oldScheduledData.EndTime,
                FirstName = oldScheduledData.FirstName,
                FromTime = oldScheduledData.FromTime,
                HoildayAmout = oldScheduledData.HoildayAmout,
                HoildayHours = oldScheduledData.HoildayHours,
                Hours = oldScheduledData.Hours,
                HolidayPayValue = oldScheduledData.HolidayPayValue,
                HoursInHoliday = oldScheduledData.HoursInHoliday,
                HST = oldScheduledData.HST,
                Id = oldScheduledData.Id,
                InvoiceNumber = oldScheduledData.InvoiceNumber,
                InvoicePrefix = oldScheduledData.InvoicePrefix,
                IsSeparateInvoice = oldScheduledData.IsSeparateInvoice,
                LastName = oldScheduledData.LastName,
                Location = oldScheduledData.Location,
                LogId = oldScheduledData.LogId,
                MessageContent = oldScheduledData.MessageContent,
                Phone1 = oldScheduledData.Phone1,
                Phone2 = oldScheduledData.Phone2,
                Rate = oldScheduledData.Rate,
                SchedulingId = oldScheduledData.SchedulingId,
                ServiceTypeName = oldScheduledData.ServiceTypeName,
                SiteURL = oldScheduledData.SiteURL,
                Start = oldScheduledData.Start,
                Startdate = oldScheduledData.Startdate,
                StartDateTime = oldScheduledData.StartDateTime,
                StateId = oldScheduledData.StateId,
                StateName = oldScheduledData.StateName,
                ThemeColor = oldScheduledData.ThemeColor,
                TimeIn = oldScheduledData.TimeIn,
                TimeOut = oldScheduledData.TimeOut,
                Total = oldScheduledData.Total,
                TypeRate = oldScheduledData.TypeRate,
                UpdatedDate = oldScheduledData.UpdatedDate,
                UserId = oldScheduledData.UserId,
                WorkMode = oldScheduledData.WorkMode,
                WorkModeName = oldScheduledData.WorkModeName,
                WorkShifDetails = oldScheduledData.WorkShifDetails,
                WorkTime = oldScheduledData.WorkTime,
                WorkTimeDetails = oldScheduledData.WorkTimeDetails,
                WorkTimeName = oldScheduledData.WorkTimeName,
                Zipcode = oldScheduledData.Zipcode

            };
            return tempScheduleData;
        }
        private async Task<bool> SendEmailAfterScheduling(ScheduledData scheduledData, int emailStatus)
        {
            EmailInput inputs = new EmailInput();
            List<string> emailIdList = new List<string>();
            List<string> ccAddressList = new List<string>();
            string ccEmail = "scheduling@tranquilcare.ca";
            //get maild id for admin typeid=4
            // inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();

            inputs.EmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(scheduledData.CareTaker);
            emailIdList.Add(inputs.EmailId);

            if (emailStatus == 1)
            {
                string emailIdClient = await _unitOfWork.clientRepository.GetEmailIdForClient(scheduledData.ClientId);
                //string emailIdCaregiver = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(scheduledData.CareTaker);
                //emailIdList.Add(emailIdCaregiver);
                emailIdList.Add(emailIdClient);
            }

            //get maild id for caretaker
            //string ccEmail = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(scheduledData.CareTaker);
            //  ccAddressList.Add(ccEmail);

            //As per new requirement email has not to be sent to the office staffs

            //var officeStaffMailIds = await _unitOfWork.CareTakerRepository.GetEmailIdForOfficeStaff();
            //if (officeStaffMailIds.Count > 0)
            //{
            //    ccAddressList.AddRange(officeStaffMailIds);
            //}

            //get maild id for scheduled client
            // string ccEmail = await _unitOfWork.clientRepository.GetEmailIdForClient(scheduledData.ClientId);
            if (ccEmail != "")
            {
                ccAddressList.Add(ccEmail);
            }

            inputs.EmailType = EmailType.Scheduling;
            inputs.Subject = "New Schedule Created";
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            var branchDetails = await GetBranchByUserId(scheduledData.ClientId);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Scheduling, branchDetails.BranchId);

            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Scheduling);
            string WelcomeMsg = "New Schedule Created";
            string MailMsg = "Schedule Details.<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
														    <tr>
														        <td style='width: 150px'>Caregiver Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + scheduledData.CareTakerName + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Client Name </td>        
                                                                <td>:</td>          
                                                                <td> " + scheduledData.ClientName + @" </td>             
                                                            </tr>   
                                                            <tr>     
                                                                <td> Start Date & Time </td>        
                                                                <td>:</td>          
                                                                <td> " + scheduledData.Start.ToLongDateString() + " - " + scheduledData.Start.ToShortTimeString() + @" </td>             
                                                            </tr>  
                                                            <tr>     
                                                                <td> End Date & Time </td>        
                                                                <td>:</td>          
                                                                <td> " + scheduledData.End.ToLongDateString() + " - " + scheduledData.End.ToShortTimeString() + @" </td>             
                                                            </tr>  
                                                            <tr>     
                                                                <td> WorkMode </td>        
                                                                <td>:</td>          
                                                                <td> " + scheduledData.WorkModeName + " - " + scheduledData.Description + @" </td>             
                                                            </tr>    
                                                            <tr>     
                                                                <td> ServiceType </td>        
                                                                <td>:</td>          
                                                                <td> " + scheduledData.ServiceTypeName + @" </td>             
                                                            </tr>  
                                                        </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = scheduledData.SiteURL;
            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string CompanyName = "Tranquil Care Inc.";
            string body = "";
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, WelcomeMsg, "", MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
            inputs.Body = body;
            return await _notificationService.SendEMail(inputs, ccAddressList, emailIdList);
        }

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategory(int CategoryId)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListByCategory(CategoryId);
        }
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndLocation(int CategoryId, LocationSearchInputs inputs)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListByCategoryAndLocation(CategoryId,inputs);
        }
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByService(int ServiceId)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListByService(ServiceId);
        }

        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndClientId(int CategoryId, int ClientId)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListByCategoryAndClientId(CategoryId, ClientId);
        }
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListByCategoryAndDate(int CategoryId, string DateTime, int hours, int clientId)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListByCategoryAndDate(CategoryId, DateTime, hours, clientId);
        }
        public async Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListByCategoryAndDate(int CategoryId, string DateTime, int hours, int clientId, int Workshift)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveAvailableCareTakerListByCategoryAndDate(CategoryId, DateTime, hours, clientId, Workshift);
        }
        public async Task<IEnumerable<CareTakers>> RetrieveAvailableCareTakerListForPublicUser(int CategoryId, string DateTime, int hours, int Workshift)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveAvailableCareTakerListForPublicUser(CategoryId, DateTime, hours, Workshift);
        }

        public async Task<IEnumerable<CaretakerAvailableReport>> RetrieveAvailableCareTakerListReport(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveAvailableCareTakerListReport(inputs);
        }

        public async Task<IEnumerable<CaretakerAvailableReport>> RetrieveCommissionReport(PaymentAdvancedSearch inputs)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCommissionReport(inputs);
        }
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdl()
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListForDdl();
        }
        public async Task<IEnumerable<CareTakers>> RetrieveCareTakerListForDdlByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.CareTakerRepository.RetrieveCareTakerListForDdlByLocation(inputs);
        }
        public async Task<IEnumerable<ClientScheduling>> GetSchedulingDetails(int schedulingId)
        {
            return await _unitOfWork.CareTakerRepository.GetSchedulingDetails(schedulingId);
        }
        public async Task<IEnumerable<UserBooking>> GetUserBookingDetails(CaretakerScheduleListSearch caretakerBookingListSearch)
        {
            return await _unitOfWork.CareTakerRepository.GetUserBookingDetails(caretakerBookingListSearch);
        }
        public async Task<IEnumerable<Notification>> GetNotification(int caretakerId)
        {
            return await _unitOfWork.CareTakerRepository.GetNotification(caretakerId);
        }
        public async Task<NotificationDetails> GetNotificationDetailsById(int bookingId)
        {
            return await _unitOfWork.CareTakerRepository.GetNotificationDetailsById(bookingId);
        }
        public async Task<UpcomingAppointment> GetUpcomingNotifications(int caretakerId)
        {
            return await _unitOfWork.CareTakerRepository.GetUpcomingNotifications(caretakerId);
        }
        public async Task<int> ConfirmAppointments(UpcomingAppointment upcomingAppointment)
        {
            int Id = await _unitOfWork.CareTakerRepository.ConfirmAppointments(upcomingAppointment);
            Task.Factory.StartNew(() => { SendEmailAfterAppointment(upcomingAppointment); });
            return Id;
        }

        private async Task<bool> SendEmailAfterAppointment(UpcomingAppointment upcomingAppointment)
        {
            EmailInput inputs = new EmailInput();
            List<string> ccAddressList = new List<string>();

            var result = await _unitOfWork.CareTakerRepository.GetAppointmentDetails(upcomingAppointment);
            inputs.EmailId = result.AdminEmail;
            ccAddressList.Add(inputs.EmailId);

            inputs.Subject = "Confirmed AppointmentsNotification";
            inputs.EmailType = Enums.EmailType.Booking;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            var branchDetails = await GetBranchByUserId(1);//need user id
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);
            string welcomeMsg = "Appointment Details.";
            string mailMsg = "Following is the Confirmed Appointment Details:<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
														    <tr>
														        <td style='width: 150px'>Booked Date</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + result.AppointmentDate + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Booking Time </td>        
                                                                <td>:</td>          
                                                                <td> " + result.AppointmentTime + @" </td>             
                                                            </tr>             
                                                            <tr>             
                                                                <td> Caretaker </td>             
                                                                <td>:</td>               
                                                                <td> " + result.CareTaker + @" </td>                  
                                                            </tr>                  
                                                            <tr>                  
                                                                <td> User </td>                  
                                                                <td>:</td>                    
                                                                <td> " + result.User + @" </td>                    
                                                            </tr>       
                                                      </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = upcomingAppointment.SiteURL;
            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string CompanyName = "Tranquil Care Inc.";
            string body = "";
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, welcomeMsg, "", mailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
            inputs.Body = body;
            return await _notificationService.SendEMail(inputs, ccAddressList);
        }

        public async Task<List<LoggedInUser>> CheckLoginCredentials(UserCredential userCredential)
        {
            return await _unitOfWork.loggedInUserRepository.CheckLoginCredentialsCheckLoginCredentials(userCredential);
        }

        public async Task<List<LoggedInUser>> CheckLoginCredentialsDouble(UserCredential userCredential)
        {
            return await _unitOfWork.loggedInUserRepository.CheckLoginCredentialsCheckLoginCredentialsDouble(userCredential);
        }


        public async Task<IEnumerable<Cities>> GetBranchesByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.CityRepository.GetBranchesByLocation(inputs);
        }
        public async Task<Cities> GetBranchByUserId(int Id)
        {
            return await _unitOfWork.CityRepository.GetBranchByUserId(Id);
        }
        public async Task<List<LoggedInUser>> CheckLoginAdmin(LoggedInUser userCredential)
        {
            return await _unitOfWork.loggedInUserRepository.CheckLoginAdminType(userCredential);
        }


        public async Task<LoggedInUser> GetLoggedInUserDetails(int userId)
        {
            return await _unitOfWork.loggedInUserRepository.GetLoggedInUserDetails(userId);
        }
        public async Task<bool> AddLoginLog(int userId)
        {
            return await _unitOfWork.loggedInUserRepository.AddLoginLog(userId);
        }

        public async Task<int> DeleteCountry(int countryId)
        {
            return await _unitOfWork.CountryRepository.DeleteCountry(countryId);
        }

        public async Task<int> DeleteState(int stateId)
        {
            return await _unitOfWork.StateRepository.DeleteState(stateId);
        }

        public async Task<int> DeleteService(int serviceId)
        {
            return await _unitOfWork.ServiceReposoitory.DeleteService(serviceId);
        }
        public async Task<IEnumerable<QuestionareModel>> RetrieveQuestions(int id)
        {
            return await _unitOfWork.QuestionareRepository.RetrieveQuestions(id);
        }

        public async Task<int> AddQuestions(QuestionareModel questions)
        {
            return await _unitOfWork.QuestionareRepository.AddQuestions(questions);
        }

        public async Task<int> DeleteQuestions(int questionId)
        {
            return await _unitOfWork.QuestionareRepository.DeleteQuestions(questionId);
        }
        public async Task<int> ChangeUserStatus(int userId, int status)
        {
            return await _unitOfWork.UsersDetailsRepository.ChangeUserStatus(userId, status);
        }
        public async Task<int> VerifyEmail(int userId)
        {
            return await _unitOfWork.UsersDetailsRepository.VerifyEmail(userId);
        }

        public async Task<int> AddRoles(Roles roles)
        {
            return await _unitOfWork.roleRepository.AddRoles(roles);
        }

        public async Task<IEnumerable<Roles>> RetrieveRolesById(int roleId)
        {
            return await _unitOfWork.roleRepository.RetrieveRoles(roleId);
        }

        public async Task<IEnumerable<Privileges>> SelectRolePrivileges(int RoleId)
        {
            return await _unitOfWork.roleRepository.SelectRolePrivileges(RoleId);
        }

        public async Task<int> DeleteRoles(int id)
        {
            return await _unitOfWork.roleRepository.DeleteRoles(id);
        }

        public async Task<int> SaveRolePrivileges(SaveRolePrivileges saveRolePrivileges)
        {
            return await _unitOfWork.roleRepository.SaveRolePrivileges(saveRolePrivileges);
        }
        public async Task<IEnumerable<UserBooking>> GetPublicUserBookingDetails(int publicUserId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetPublicUserBookingDetails(publicUserId);
        }
        public async Task<UserBooking> GetPublicUserBookingDetailsById(int bookingId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetPublicUserBookingDetailsById(bookingId);
        }
        public async Task<IEnumerable<UserBooking>> GetCaretakerBookingDetails(CalenderEventInput calenderEventInput)
        {
            return await _unitOfWork.UsersDetailsRepository.GetCaretakerBookingDetails(calenderEventInput);
        }
        public async Task<IEnumerable<PublicUserPaymentHistory>> GetPublicUserPaymentHistory(int publicUserId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetPublicUserPaymentHistory(publicUserId);
        }
        public async Task<IEnumerable<AdminBookingNotification>> GetPublicUserNotification(int publicUserId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetPublicUserNotification(publicUserId);
        }
        public async Task<PublicUserNotificationDetails> GetUserNotificationDetailsById(int bookingId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetUserNotificationDetailsById(bookingId);
        }
        public async Task<int> AddInvoiceDetails(PublicUserPaymentInvoiceInfo userInvoiceDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.AddInvoiceDetails(userInvoiceDetails);
        }

        public async Task<IEnumerable<UsersDetails>> GetUserInvoiceDetails()
        {
            return await _unitOfWork.UsersDetailsRepository.GetUserInvoiceDetails();
        }

        public async Task<int> ChangeBookigStatus(int userId, int status, string siteURL, string reason)
        {
            int result = await _unitOfWork.UsersDetailsRepository.ChangeBookigStatus(userId, status);
            PublicUserNotificationDetails notificationDetails = await _unitOfWork.UsersDetailsRepository.GetUserNotificationDetailsById(userId);
            notificationDetails.SiteURL = siteURL;
            notificationDetails.Reason = reason;
            string userEmailId = await _unitOfWork.CareTakerRepository.GetEmailIdForUser(notificationDetails.PublicUserId);
            Task.Factory.StartNew(() => { SendEmailToPublicUserAsync(notificationDetails, status, userEmailId); });
            return result;
        }
        private async Task<bool> SendEmailToPublicUserAsync(PublicUserNotificationDetails notificationDetails, int status, string userEmailId)
        {
            List<string> ccAddressList = new List<string>();
            //string cc = await _unitOfWork.CareTakerRepository.GetEmailIdForAdmin();
            string cc = "info@tranquilcare.ca";
            ccAddressList.Add(cc);

            EmailInput inputs = new EmailInput();
            inputs.EmailType = EmailType.Booking;
            inputs.EmailId = userEmailId;
            inputs.EmailConfig = await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
            //inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType);
            var branchDetails = await GetBranchByUserId(notificationDetails.PublicUserId);
            inputs.EmailIdConfig = await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(inputs.EmailType, branchDetails.BranchId);

            string WelcomeMsg = "";
            string Mailcontent = "";
            if (status == 3)
            {
                inputs.Subject = "Booking Rejected";
                WelcomeMsg = "Booking Rejected";
                Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
									    <tr>
									        <td style='width: 150px'>Booked by</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + notificationDetails.CareRecipient + @" </td>     
                                       </tr>     
                                       
                                       <tr>     
                                           <td> Booked Date </td>        
                                           <td>:</td>          
                                           <td> " + DateTime.Now.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking From Date & Time </td>        
                                           <td>:</td>          
                                           <td> " + notificationDetails.FromDateTime.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking To Date & Time </td>        
                                           <td>:</td>          
                                           <td> " + notificationDetails.ToDateTime.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                        <tr>     
                                           <td> Booking Rejected Reason </td>        
                                           <td>:</td>          
                                           <td> " + notificationDetails.Reason + @" </td>             
                                       </tr> 
                                     </table>";
            }
            else if (status == 4)
            {
                inputs.Subject = "Booking Cancelled";
                WelcomeMsg = "Booking Cancelled :Your Refund will be Initiated";
                Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
									    <tr>
									        <td style='width: 150px'>Booked by</td>
                                           <td style = 'width:10px'>:</td>     
                                           <td> " + notificationDetails.CareRecipient + @" </td>     
                                       </tr>     
                                       
                                       <tr>     
                                           <td> Booked Date </td>        
                                           <td>:</td>          
                                           <td> " + DateTime.Now.ToString("dd-MMM-yyyy") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking From Date & Time </td>        
                                           <td>:</td>          
                                           <td> " + notificationDetails.FromDateTime.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                       <tr>     
                                           <td> Booking To Date & Time </td>        
                                           <td>:</td>          
                                           <td> " + notificationDetails.ToDateTime.ToString("dd-MMM-yyyy h: mm tt") + @" </td>             
                                       </tr> 
                                        <tr>     
                                           <td> Booking Cancelled Reason </td>        
                                           <td>:</td>          
                                           <td> " + notificationDetails.Reason + @" </td>             
                                       </tr> 
                                     </table>";
            }

            string MailMsg = "Following is the booking details.<br/>";

            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";

            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string CompanyName = "Tranquil Care Inc.";
            string body = "";
            string siteUrl = notificationDetails.SiteURL;
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, WelcomeMsg, "", MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);

            inputs.Body = body;

            return await _notificationService.SendEMail(inputs, ccAddressList);
        }

        public async Task<IEnumerable<CaretakerScheduleList>> GetCaretakerScheduleList(CaretakerScheduleListSearch caretakerScheduleListSearch)
        {
            return await _unitOfWork.CareTakerRepository.GetCaretakerScheduleList(caretakerScheduleListSearch);
        }
        public async Task<int> SendContactForm(ContactModel contactModel)
        {
            var emailId = await _unitOfWork.UsersDetailsRepository.GetAdminEmailId();
            Task.Factory.StartNew(() => { SendEmailToAdmin(contactModel, emailId); });
            return 1;
        }
        private int SendEmailToAdmin(ContactModel contactModel, string emailId)
        {
            EmailInput input = new EmailInput();
            input.EmailId = emailId;
            input.EmailConfig = _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration().Result;
            //input.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration).Result;
            var branchDetails = GetBranchByUserId(1);//need user
            input.EmailIdConfig = _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(EmailType.Registration, branchDetails.Result.BranchId).Result;

            input.Subject = "Contact Form";
            input.EmailType = EmailType.Registration;
            input.Body = SendContactEmailBody(contactModel);
            _notificationService.SendEMail(input);
            return 1;
        }
        private string SendContactEmailBody(ContactModel contactModel)
        {
            string WelcomeMsg = "Contact Form";
            string MailMsg = "Contact Details are as follows.<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
														    <tr>
														        <td style='width: 150px'>Name</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + contactModel.Name + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Phone </td>        
                                                                <td>:</td>          
                                                                <td> " + contactModel.Phone + @" </td>             
                                                            </tr>   
                                                            <tr>     
                                                                <td> Email </td>        
                                                                <td>:</td>          
                                                                <td> " + contactModel.Email + @" </td>             
                                                            </tr>  
                                                            <tr>     
                                                                <td> Description </td>        
                                                                <td>:</td>          
                                                                <td> " + contactModel.Description + @" </td>             
                                                            </tr>  
                                                        </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = contactModel.SiteURL;
            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string CompanyName = "Tranquil Care Inc.";
            string body = "";
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, WelcomeMsg, "", MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
            return body;
        }
        public async Task<UsersDetails> GetAdminProfile(int id)
        {
            return await _unitOfWork.UsersDetailsRepository.GetAdminProfile(id);
        }
        public async Task<int> UpdateAdminProfile(UsersDetails adminDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateAdminProfile(adminDetails);
        }
        public async Task<int> UpdateUserEmail(int id, string emailId)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateUserEmail(id, emailId);
        }

        public async Task<int> InsertUpdateCompanyDetails(CompanyProfile companyProfile)
        {
            return await _unitOfWork.UsersDetailsRepository.InsertUpdateCompanyDetails(companyProfile);
        }

        public async Task<CompanyProfile> GetCompanyProfiles(int CompanyId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetCompanyProfiles(CompanyId);
        }
        public async Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryList(InvoiceHistory invoiceHistory)
        {
            return await _unitOfWork.clientRepository.GetInvoiceHistoryList(invoiceHistory);
        }
        public async Task<IEnumerable<InvoiceSearchInpts>> GetInvoiceHistoryById(int id)
        {
            return await _unitOfWork.clientRepository.GetInvoiceHistoryById(id);
        }
        public async Task<string> LoadPhoneCodeByCountryId(int countryId)
        {
            return await _unitOfWork.UsersDetailsRepository.LoadPhoneCodeByCountryId(countryId);
        }

        public async Task<IEnumerable<CaretakerType>> GetCaretakerType()
        {
            return await _unitOfWork.CareTakerRepository.GetCaretakerType();
        }
        public async Task<int> SaveResidentDetails(Resident residentDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.SaveResidentDetails(residentDetails);
        }
        public async Task<IEnumerable<Resident>> RetrieveResidentDetails()
        {
            return await _unitOfWork.UsersDetailsRepository.RetrieveResidentDetails();
        }
        public async Task<IEnumerable<Resident>> RetrieveResidentDetailsById(int clientId)
        {
            return await _unitOfWork.UsersDetailsRepository.RetrieveResidentDetailsById(clientId);
        }
        public async Task<int> DeleteResident(int residentId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeleteResident(residentId);
        }

        public async Task<int> InsertUpdatePaypalSettings(PayPalAccount payPal)
        {
            return await _unitOfWork.UsersDetailsRepository.InsertUpdatePaypalSettings(payPal);
        }

        public async Task<PayPalAccount> GetPayPalAccount(int paypalId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetPayPalAccount(paypalId);
        }

        public async Task<int> InsertUpdateTestimonials(Testimonial testimonial)
        {
            return await _unitOfWork.UsersDetailsRepository.InsertUpdateTestimonials(testimonial);
        }

        public async Task<IEnumerable<Testimonial>> RetrieveTestimonials(int testimonialId)
        {
            return await _unitOfWork.UsersDetailsRepository.RetrieveTestimonials(testimonialId);
        }

        public async Task<int> DeleteTestimonial(int testimonialId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeleteTestimonial(testimonialId);
        }
        public async Task<int> AddEmailConfiguration(EmailConfiguration emailConfiguration)
        {
            return await _unitOfWork.UsersDetailsRepository.AddEmailConfiguration(emailConfiguration);
        }

        public async Task<IEnumerable<EmailConfiguration>> GetConfigList()
        {
            return await _unitOfWork.UsersDetailsRepository.GetConfigList();
        }
        public async Task<int> SetConfig(int configId)
        {
            return await _unitOfWork.UsersDetailsRepository.SetConfig(configId);
        }
        public async Task<int> DeleteConfigDetails(int configId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeleteConfigDetails(configId);
        }
        private string SendForgotPasswordEmailBody(ForgotPasswordViewModel passwordViewModel, string name, string password)
        {
            string WelcomeMsg = "Forgot Password";
            string MailMsg = "Following is your account details.<br/>";
            string Mailcontent = @"<table cellpadding=5 style = 'font-family:Roboto,Tahoma,Arial,Helvetica,sans-serif;font-weight:300;color:#6b6b6b;margin:7px 0px 7px 0px;font-size:1.0em'>
														    <tr>
														        <td style='width: 150px'>Username</td>
                                                                <td style = 'width:10px'>:</td>     
                                                                <td> " + passwordViewModel.Email + @" </td>     
                                                            </tr>     
                                                            <tr>     
                                                                <td> Password </td>        
                                                                <td>:</td>          
                                                                <td> " + password + @" </td>             
                                                            </tr>                                                             
                                                        </table>";
            string ContactNo = "1-800-892-6066";
            string RegardsBy = "Tranquil Care Inc.";
            string siteUrl = passwordViewModel.SiteURL;
            string CompanyName_TagLine = "Tranquil Care - Delivering Care Excellence.";
            string CompanyName = "Tranquil Care Inc.";
            string body = "";
            string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
            var sr = new StreamReader(sd);
            body = sr.ReadToEnd();
            //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
            body = string.Format(body, WelcomeMsg, name, MailMsg, Mailcontent, ContactNo, RegardsBy, siteUrl, CompanyName_TagLine, CompanyName);
            return body;
        }

        public async Task<int> AddEmailTypeConfiguration(EmailTypeConfiguration emailTypeConfiguration)
        {
            return await _unitOfWork.UsersDetailsRepository.AddEmailTypeConfiguration(emailTypeConfiguration);
        }

        public async Task<IEnumerable<EmailTypeConfiguration>> GetEmailTypeConfig()
        {
            return await _unitOfWork.UsersDetailsRepository.GetEmailTypeConfig();
        }

        public async Task<EmailTypeConfiguration> GetEmailIdConfigByType(EmailType emailType, int branchId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetEmailIdConfigByType(emailType,branchId);
        }

        public async Task<EmailConfiguration> GetDefaultConfiguration()
        {
            return await _unitOfWork.UsersDetailsRepository.GetDefaultConfiguration();
        }

        public async Task<int> DeleteEmailTypeConfig(int configId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeleteEmailTypeConfig(configId);
        }

        public async Task<IEnumerable<BookingSchedulingData>> GetAllBookingSchedulingData(CalenderEventInput calenderEventInput)
        {
            List<BookingSchedulingData> bookingSchedulingList = new List<BookingSchedulingData>();
            var bookingList = await _unitOfWork.UsersDetailsRepository.GetCaretakerBookingDetails(calenderEventInput);
            foreach (var booking in bookingList)
            {
                BookingSchedulingData bookingData = new BookingSchedulingData
                {
                    Service = booking.Service,
                    BookedUser = booking.BookedUser,
                    BookingDate = booking.BookingDate,
                    StartDateTime = booking.FromDateTime,
                    EndDateTime = booking.ToDateTime,
                    Description = booking.CareRecipient,
                    Start = booking.Start,
                    End = booking.EndDate,
                    Type = "Booking",
                    CaretakerId = calenderEventInput.CaretakerId
                };
                bookingSchedulingList.Add(bookingData);
            }
            var schedulingList = await _unitOfWork.clientRepository.GetAllScheduledetails(calenderEventInput);
            foreach (var schedule in schedulingList)
            {
                BookingSchedulingData scheduleData = new BookingSchedulingData
                {
                    ClientName = schedule.ClientName,
                    CareTakerName = schedule.CareTakerName,
                    CareTakerTypeName = schedule.CareTakerTypeName,
                    WorkModeName = schedule.WorkModeName,
                    WorkTimeName = schedule.WorkTimeName,
                    Description = schedule.Description,
                    Start = schedule.Start,
                    End = schedule.End,
                    StartDateTime = schedule.StartDateTime,
                    EndDateTime = schedule.EndDateTime,
                    FromTime = schedule.FromTime,
                    EndTime = schedule.EndTime,
                    Type = "Scheduling",
                    CaretakerId = schedule.CareTaker,
                    ClientLocation = schedule.Location
                };
                bookingSchedulingList.Add(scheduleData);
            }
            return bookingSchedulingList;
        }
        public async Task<IEnumerable<BookingPayriseModel>> GetBookingPayriseList(BookingPayriseModel bookingPayrise)
        {
            return await _unitOfWork.UsersDetailsRepository.GetBookingPayriseList(bookingPayrise);
        }
        public async Task<IEnumerable<InvoicePayriseModel>> GetInvoicePayriseList(InvoicePayriseModel invoicePayrise)
        {
            return await _unitOfWork.UsersDetailsRepository.GetInvoicePayriseList(invoicePayrise);
        }
        public async Task<IEnumerable<PayrollPayriseModel>> GetPayrollPayriseList(PayrollPayriseModel payrollPayrise)
        {
            return await _unitOfWork.UsersDetailsRepository.GetPayrollPayriseList(payrollPayrise);
        }
        public async Task<int> DeleteBookingPayrise(int bookingPayriseId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeleteBookingPayrise(bookingPayriseId);
        }
        public async Task<int> DeleteInvoicePayrise(int invoicePayriseId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeleteInvoicePayrise(invoicePayriseId);
        }
        public async Task<int> DeletePayrollPayrise(int payrollPayriseId)
        {
            return await _unitOfWork.UsersDetailsRepository.DeletePayrollPayrise(payrollPayriseId);
        }
        public async Task<IEnumerable<BookingPayriseModel>> GetAllBookingPayriseDetails()
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllBookingPayriseDetails();
        }
        public async Task<IEnumerable<InvoicePayriseModel>> GetAllInvoicePayriseDetails()
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllInvoicePayriseDetails();
        }
        public async Task<IEnumerable<PayrollPayriseModel>> GetAllPayrollPayriseDetails()
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllPayrollPayriseDetails();
        }

        public async Task<IEnumerable<UsersDetails>> GetAllUserDetails()
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllUserDetails();
        }
        
        public async Task<IEnumerable<UsersDetails>> GetAllUserDetailsByLocation(LocationSearchInputs inputs)
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllUserDetailsByLocation(inputs);
        }
        public async Task<int> DeleteSchedule(ScheduleDeleteData deleteData)
        {

            return await _unitOfWork.UsersDetailsRepository.DeleteSchedule(deleteData);
        }
        public async Task<int> UpdateUserDetails(UsersDetails usersDetails)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateUserDetails(usersDetails);
        }

        public async Task<IEnumerable<DocumentsList>> GetAllCaretakerDocuments()
        {
            return await _unitOfWork.UsersDetailsRepository.GetAllCaretakerDocuments();
        }

        public async Task<int> UpdateCaretakerDocuments(DocumentsList doc)
        {
            return await _unitOfWork.UsersDetailsRepository.UpdateCaretakerDocuments(doc);
        }
        public async Task<UserRoleDetailsModel> GetUserRoleDetails(int userId)
        {
            return await _unitOfWork.UsersDetailsRepository.GetUserRoleDetails(userId);
        }
    }
}
