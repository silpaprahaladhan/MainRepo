using Newtonsoft.Json;
using Nirast.Pcms.Ap.Application.Infrastructure;
using Nirast.Pcms.Api.Helpers;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Nirast.Pcms.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : ApiController
    {
        private IHomeService _homeService;
        private IPCMSLogger _logger;
        private static IPCMSService _pcmsService;

        public HomeController(IHomeService homeService, IPCMSLogger logger, IPCMSService pcmsService)
        {
            _homeService = homeService;
            _logger = logger;
            _pcmsService = pcmsService;
        }

        [HttpPost]
        [Route("api/Home/CheckLoginCredential")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> CheckLoginCredential(UserCredential userCredential)
        {
            UserCredential newUserCredential = new UserCredential()
            {
                LoginName = userCredential.LoginName,

            };
            try
            {
                var loggedInUserDetails = await _pcmsService.CheckLoginCredentials(newUserCredential);
                var userDetails = loggedInUserDetails.Where(x => x.EmployeeNumber == userCredential.LoginName).FirstOrDefault();

                //loggedInUserDetails.ForEach(x => x.Password = security.Decrypt(x.Password, encryptionPassword));
                //var userDetails = loggedInUserDetails.FirstOrDefault(x => x.Password.Equals(userCredential.Password));
                if (userDetails != null)
                {
                    if ((userDetails.UserStatus != 1) || (userDetails.IsVerified == false))
                    {
                        var result = JsonConvert.SerializeObject(loggedInUserDetails);
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                        return response;
                    }
                    else
                    {
                        LoggedInUser user = await _pcmsService.GetLoggedInUserDetails(userDetails.UserId);
                        if (user != null)
                        {
                            var result = JsonConvert.SerializeObject(user);
                            if (result != null)
                            {
                                bool logResult = await _pcmsService.AddLoginLog(user.UserId);
                                var response = Request.CreateResponse(HttpStatusCode.OK);
                                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                                return response;
                            }
                        }
                        else
                        {
                            return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Registered User staff details found");
                        }
                    }
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Registered User staff details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered User details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Home/CheckLoginCredentialDouble")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> CheckLoginCredentialDouble(UserCredential userCredential)
        {
            UserCredential newUserCredential = new UserCredential()
            {
                LoginName = userCredential.LoginName,

            };
            try
            {
                var loggedInUserDetails = await _pcmsService.CheckLoginCredentialsDouble(newUserCredential);

                string result = JsonConvert.SerializeObject(loggedInUserDetails);
               
                    var response = Request.CreateResponse(HttpStatusCode.OK, loggedInUserDetails);
                   
                
                return response;
                //var userDetails = loggedInUserDetails.Where(x => x.EmailAddress == userCredential.LoginName).ToList();
                //var result = JsonConvert.SerializeObject(userDetails);
                //var response = Request.CreateResponse(HttpStatusCode.OK);
                //response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                //return response;


            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered User details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #region Created by silpa on 13-01-2022
        [HttpPost]
        [Route("api/Home/CheckAdmin")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> CheckAdmin(LoggedInUser userCredential)
        {
            LoggedInUser newUserCredential = new LoggedInUser()
            {
                UserId = userCredential.UserId,
                UserType=userCredential.UserType

            };
            try
            {
                var loggedInUserDetails = await _pcmsService.CheckLoginAdmin(newUserCredential);




                var userDetails = loggedInUserDetails.Where(x => x.UserId == userCredential.UserId).FirstOrDefault();

                //loggedInUserDetails.ForEach(x => x.Password = security.Decrypt(x.Password, encryptionPassword));
                //var userDetails = loggedInUserDetails.FirstOrDefault(x => x.Password.Equals(userCredential.Password));
                if (userDetails != null)
                {
                  
                        var result = JsonConvert.SerializeObject(userDetails);
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                        return response;
                   
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Registered User staff details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered User details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion





        /// <summary>
        /// Method to retrieve caretaker details using advanced search
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Home/SearchCareTaker")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SearchCareTaker(AdvancedSearchInputModel inputs)
        {

            try
            {
                var careTakerDetails = await _homeService.RetrievecareTakerDetails(inputs);
                string result = JsonConvert.SerializeObject(careTakerDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No office staff details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }       

        [HttpPost]
        [Route("api/Home/KeywordCareTakerSearchDetail")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> KeywordCareTakerSearchDetail()
        {
            try
            {
                string keyword = Request.Content.ReadAsStringAsync().Result;
                var careTakerDetails = await _homeService.KeywordCareTakerSearchDetail(keyword);
                string result = JsonConvert.SerializeObject(careTakerDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No office staff details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
               
        /// <summary>
        /// Method to get approved rate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Home/GetApprovedRate")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetApprovedRate()
        {
            try
            {
                var rateResult = await _homeService.RetrievedApprovedRate();
                string result = JsonConvert.SerializeObject(rateResult);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No approved rate found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get approved rate");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Home/RetrievePassword")]
        //[BasicAuthentication("Caretaker", "Public User", "Office Staff")]
        public async Task<HttpResponseMessage> RetrievePassword(ForgotPasswordViewModel Email)
        {
            try
            {
                var rateResult = await _pcmsService.RetrievePassword(Email.Email);
                string result = JsonConvert.SerializeObject(rateResult);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No user found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get current password");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Home/ChangePassword")]
        //[BasicAuthentication("Caretaker", "Public User", "Office Staff", "Administrator")]
        public async Task<HttpResponseMessage> ChangePassword(ChangePassword changePassword)
        {
            try
            {
                var Result = await _pcmsService.ChangePassword(changePassword);
                string result = JsonConvert.SerializeObject(Result);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No user found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to set new password");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///Contact Form
        /// </summary>
        /// <param name="UsersDetails"></param>
        /// <returns></returns>
       
        [HttpPost]
        [Route("api/Home/SendContactForm")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SendContactForm(ContactModel contactModel)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.SendContactForm(contactModel);

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
    }
}