using Newtonsoft.Json;
using Nirast.Pcms.Api.Helpers;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Models;
using Nirast.Pcms.Ap.Application.Infrastructure;
using System.IO;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;
using static Nirast.Pcms.Api.Sdk.Entities.PublicUserCaretakerBooking;

namespace Nirast.Pcms.Api.Controllers
{
    public class PublicUserController : ApiController
    {
        // GET: PublicUser
        private IPCMSService _pcmsService;
        private IPCMSLogger _logger;
        private INotificationService _notifactionService;
        public PublicUserController() { }
        public PublicUserController(IPCMSService pcmsService, INotificationService notifactionService, IPCMSLogger logger)
        {
            _pcmsService = pcmsService;
            _notifactionService = notifactionService;
            _logger = logger;
        }

        /// <summary>
        /// To retrieve required UsersDetails details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/UsersDetails/5
        [Route("api/PublicUser/GetAllUsersDetails")]
        public async Task<HttpResponseMessage> GetAllUsersDetails(string flag, string value)
        {
            PublicUserRegistration UsersDetails = new PublicUserRegistration();
            try
            {
                var lstUsersDetails = await _pcmsService.GetUsersDetailsById(flag, value);
                string result = JsonConvert.SerializeObject(lstUsersDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get user details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required UsersDetails details by location
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PublicUser/GetAllUsersDetailsByLocation")]
        public async Task<HttpResponseMessage> GetAllUsersDetailsByLocation(string flag, string value, LocationSearchInputs inputs)
        {
            PublicUserRegistration UsersDetails = new PublicUserRegistration();
            try
            {
                if (inputs == null)
                {
                    inputs = new LocationSearchInputs();
                    inputs.CountryId = null;
                    inputs.StateId = null;
                    inputs.CityId = null;
                }
                if (inputs.CountryId == 0)
                {
                    inputs.CountryId = null;
                }
                if (inputs.StateId == 0)
                {
                    inputs.StateId = null;
                }
                if (inputs.CityId == 0)
                {
                    inputs.CityId = null;
                }
                var lstUsersDetails = await _pcmsService.GetUsersDetailsByLocation(flag, value,inputs);
                string result = JsonConvert.SerializeObject(lstUsersDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get user details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/User/UpdateUserInvoiceNumber/{userid}/{invoiceNumber}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> UpdateUserInvoiceNumber(int userid,int invoicenumber)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _pcmsService.UpdateUserInvoiceNumber(userid,invoicenumber);
                string result = JsonConvert.SerializeObject(clientDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No client details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update InvoiceNumber");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/User/SaveUserInvoiceDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveUserInvoiceDetails(InvoiceSearchInpts invoiceDetails)
        {
            try
            {
                int userDetailId = await _pcmsService.AddUserInvoiceDetails(invoiceDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (userDetailId == 0)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Save Client invoice Details failed.");
                }
                response.Content = new StringContent(userDetailId.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Save Client invoice Details failed.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        /// <summary>
        ///To add UsersDetails
        /// </summary>
        /// <param name="UsersDetails"></param>
        /// <returns></returns>
        // POST: api/UsersDetails
        [HttpPost]
        [Route("api/PublicUser/AddPublicUser")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> AddPublicUser(PublicUserRegistration usersDetails)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.AddPublicUser(usersDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User registration failed.");
                }
                response.Content = new StringContent(UserDetailId.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to register user.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/PublicUser/ForgotPassword")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> ForgotPassword(ForgotPasswordViewModel emailId)
        {
            try
            {
                string result = "success";

                var UserDetailId = await _pcmsService.ForgotPassword(emailId);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User registration failed.");
                }
                response.Content = new StringContent(UserDetailId.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to register user.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
       
        [HttpPost]
        [Route("api/PublicUser/UpdateUserDetails")]
        //[BasicAuthentication("Public User")]
        public async Task<HttpResponseMessage> UpdateUserDetails(PublicUserRegistration usersDetails)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Update user failed");
                }
                int UserDetailId = await _pcmsService.EditUserProfile(usersDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update user details. User Id: {0}", usersDetails.UserRegnId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/PublicUser/UpdateCardDetails")]
        //[BasicAuthentication("Public User")]
        public async Task<HttpResponseMessage> UpdateCardDetails(PublicUserRegistration usersDetails)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User updation creation failed.");
                }
                int cardId = await _pcmsService.EditCardDetails(usersDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update card details. User Id: {0}", usersDetails.UserRegnId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/PublicUser/VerifyUserProfile")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> UpdateUserVerification(VerifyUserAccount verifyUsers)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User verification failed");
                }
                int UserDetailId = await _pcmsService.VerifyUserProfile(verifyUsers);
                var response = new HttpResponseMessage();
                if (UserDetailId == 1)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                } else if(UserDetailId == 2)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);

                }
                else if (UserDetailId == 3)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);

                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to verify user id: {0}", verifyUsers.UserId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/PublicUser/UploadUserProfilePic")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> UploadUserProfilePic(PublicUserRegistration usersDetails)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User profile picture upload failed.");
                }
                int cardId = await _pcmsService.UpdateUserProfilePic(usersDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update user pic. User Id: {0}", usersDetails.UserRegnId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/PublicUser/SaveCaretakerBooking")]
        //[BasicAuthentication("Public User")]
        public async Task<HttpResponseMessage> SaveCaretakerBooking(CaretakerBookingModel careTakerBooking)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Caregiver booking creation failed.");
                }
                int bookingId = await _pcmsService.SaveCaretakerBooking(careTakerBooking);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (bookingId == 999999)
                {
                    response = Request.CreateResponse(HttpStatusCode.Found);
                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update booking details. User Id: {0}", careTakerBooking.BookingId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to send otp to email
        /// </summary>
        /// <param name="input"></param>
        //[ApiAuthorize("blogger_user")]
        [HttpPost]
        [Route("api/Users/SendVerificationEmail")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SendVerificationEmail(VerifyEmail verifyEmail)
        {
            try
            {
                Random generator = new Random();
                String otp = generator.Next(0, 999999).ToString("D6");
                EmailInput inputs = new EmailInput();
                inputs.EmailId = verifyEmail.Email;
                inputs.EmailType = Enums.EmailType.Registration;
                inputs.EmailConfig = await _pcmsService.GetDefaultConfiguration();
                var branchDetails = _pcmsService.GetBranchByUserId(verifyEmail.UserId);
                inputs.EmailIdConfig = await _pcmsService.GetEmailIdConfigByType(inputs.EmailType, branchDetails.Result.BranchId);

               // inputs.EmailIdConfig = await _pcmsService.GetEmailIdConfigByType(inputs.EmailType);
                //UserPaymentInvoiceModel mailContent = new UserPaymentInvoiceModel()
                //{
                //    FirstName = verifyEmail.FirstName,
                //};
                inputs.Subject = verifyEmail.Subject;
                inputs.Body = GetVerificationLinkEmailBody(verifyEmail);
                
                //inputs.Body = "Dear  " + verifyEmail.FirstName + ",<br/>";
                //inputs.Body += "<br/>Please click on the following link to activate your account.<br/>";
                //inputs.Body += verifyEmail.VerificationLink;
                //inputs.Body += "<br/><br/>Thanks";

                bool result = await _notifactionService.SendEMail(inputs);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send otp to email.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        private string GetVerificationLinkEmailBody(VerifyEmail verifyEmail)
        {
            try
            {
                Security security = new Security();
                string body = "";
                string verifybody = "";
                string vt = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/VerifyTemplete.html";
                var rdr = new StreamReader(vt);
                verifybody = rdr.ReadToEnd();
                verifybody = string.Format(verifybody, verifyEmail.Mailcontent);
                string sd = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates/EmailCommon.html";
                var sr = new StreamReader(sd);
                body = sr.ReadToEnd();
                //body = string.Format(body, siteUrl, invoiceDetails.UserName, invoiceDetails.BookingDate, invoiceDetails.Amount, invoiceDetails.TaxAmount, invoiceDetails.TotalAmount, invoiceDetails.InvoiceNumber.ToString());
                body = string.Format(body,verifyEmail.WelcomeMsg,verifyEmail.FirstName,verifyEmail.MailMsg, verifybody, verifyEmail.ContactNo,verifyEmail.RegardsBy,verifyEmail.siteUrl,verifyEmail.CompanyName_TagLine,verifyEmail.CompanyName);
                return body;
            }
            catch (Exception ex)
            {
                int i = 0;
            }
            return string.Empty;
        }
        [Route("api/User/GetCheckLoginNameAlreadyExist/{LoginName}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCheckLoginNameAlreadyExist(string LoginName)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Checking failed");
                }
                int Output = await _pcmsService.CheckLoginNameExist(LoginName);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(Output.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Fetch", LoginName);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve PublicUser Booking Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/GetPublicUserBookingDetails/{publicUserId}")]
     
        public async Task<HttpResponseMessage> GetPublicUserBookingDetails(int publicUserId)
        {
            List<UserBooking> userBookingDetails = new List<UserBooking>();
            try
            {
                var bookingDetails = await _pcmsService.GetPublicUserBookingDetails(publicUserId);
                string result = JsonConvert.SerializeObject(bookingDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No  details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get scheduling Details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }
        [HttpGet]
        [Route("api/User/GetPublicUserBookingDetailsById/{bookingId}")]

        public async Task<HttpResponseMessage> GetPublicUserBookingDetailsById(int bookingId)
        {
            List<UserBooking> userBookingDetails = new List<UserBooking>();
            try
            {
                var bookingDetails = await _pcmsService.GetPublicUserBookingDetailsById(bookingId);
                string result = JsonConvert.SerializeObject(bookingDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No  details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get scheduling Details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }


    

        [HttpGet]
        [Route("api/User/GetCaretakerBookingDetails")]

        public async Task<HttpResponseMessage> GetCaretakerBookingDetails(CalenderEventInput calenderEventInput)
        {
            List<UserBooking> userBookingDetails = new List<UserBooking>();
            try
            {
                var bookingDetails = await _pcmsService.GetCaretakerBookingDetails(calenderEventInput);
                string result = JsonConvert.SerializeObject(bookingDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No  details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get GetCaretakerBookingDetails");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }


        /// <summary>
        /// method to retrieve PublicUser Payment History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/GetPublicUserPaymentHistory/{publicUserId}")]

        public async Task<HttpResponseMessage> GetPublicUserPaymentHistory(int publicUserId)
        {
            List<PublicUserPaymentHistory> userBookingDetails = new List<PublicUserPaymentHistory>();
            try
            {
                var bookingDetails = await _pcmsService.GetPublicUserPaymentHistory(publicUserId);
                string result = JsonConvert.SerializeObject(bookingDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No  details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get scheduling Details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }


        /// <summary>
        /// method to retrieve PublicUser Notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/GetPublicUserNotification/{publicUserId}")]

        public async Task<HttpResponseMessage> GetPublicUserNotification(int publicUserId)
        {
            List<AdminBookingNotification> userNotificationDetails = new List<AdminBookingNotification>();
            var response = new HttpResponseMessage();
            try
            {
                var notificationDetails = await _pcmsService.GetPublicUserNotification(publicUserId);
                string result = JsonConvert.SerializeObject(notificationDetails);
                if (result != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No  details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get scheduling Details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }

        /// <summary>
        /// method to retrieve notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        [Route("api/User/GetUserNotificationDetailsById/{bookingId}")]
        public async Task<HttpResponseMessage> GetUserNotificationDetailsById(int bookingId)
        {
            try
            {
                var notification = await _pcmsService.GetUserNotificationDetailsById(bookingId);
                string result = JsonConvert.SerializeObject(notification);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No  details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get notification Details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }

        [HttpPost]
        [Route("api/User/AddInvoiceDetails")]

        public async Task<HttpResponseMessage> AddInvoiceDetails(PublicUserPaymentInvoiceInfo userInvoiceDetails)
        {

            try
            {
                var userDetails = await _pcmsService.AddInvoiceDetails(userInvoiceDetails);
                string result = JsonConvert.SerializeObject(userDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No client details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get client details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("api/User/GetPublicInvoiceDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetPublicInvoiceDetails()
        {
            List<UsersDetails> clientInvoiceDetails = new List<UsersDetails>();
            try
            {
                var clientDetails = await _pcmsService.GetUserInvoiceDetails();
                string result = JsonConvert.SerializeObject(clientDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No client details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get client details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/User/SaveBookingDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveBookingDetails(PublicUserCaretakerBooking scheduledData)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.AddBookingDetails(scheduledData);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client registration failed.");
                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to register Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/User/GetAllBookingdetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllBookingdetails(CalenderBookingEventInput calenderEventInput)
        {
            try
            {
                var scheduledDetails = await _pcmsService.GetAllBookingdetails(calenderEventInput);
                string result = JsonConvert.SerializeObject(scheduledDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get scheduled details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Client/GetAllUserDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllUserDetails()
        {
            List<UsersDetails> officeStaffRegistration = new List<UsersDetails>();
            try
            {
                var userDetails = await _pcmsService.GetAllUserDetails();
                string result = JsonConvert.SerializeObject(userDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No user details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get client details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/User/DeleteSchedule")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteSchedule(ScheduleDeleteData deleteData)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Category delete failed.");
                }
                int id = await _pcmsService.DeleteSchedule(deleteData);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete schedules");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
    }
}