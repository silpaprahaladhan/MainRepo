using Newtonsoft.Json;
using Nirast.Pcms.Api.Helpers;
using Nirast.Pcms.Api.Models;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminController : ApiController
    {
        private IOfficeStaffService _officeStaffService;
        private IServicesService _servicesService;
        private IPCMSService _pcmsService;
        private IPCMSLogger _logger;
        private IInvoiceService _invoiceService;
        private INotificationService _notifactionService;
        private IAdminServices _adminService;
        public AdminController(IOfficeStaffService officeStaffService, IServicesService servicesService, IPCMSService pcmsService, IPCMSLogger logger, IInvoiceService invoiceService, INotificationService notifactionService, IAdminServices adminServices)
        {
            _officeStaffService = officeStaffService;
            _servicesService = servicesService;
            _pcmsService = pcmsService;
            _invoiceService = invoiceService;
            _notifactionService = notifactionService;
            _logger = logger;
            _adminService = adminServices;
        }
        #region public methods

        /// <summary>
        /// To retrieve country details
        /// </summary>
        /// <returns></returns>
        // GET: api/Country
        [HttpGet]
        [Route("api/Admin/GetCountryDetails/{countryId}")]
        //[AllowAnonymous]
        public async Task<HttpResponseMessage> GetCountryDetails(int countryId)

        {
            try
            {
                var lstCountry = await _pcmsService.RetrieveCountry(countryId);
                string result = JsonConvert.SerializeObject(lstCountry);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstCountry);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No country found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Country details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Method to retrieve caretaker details using advanced search
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetDefaultCountry")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetDefaultCountry()
        {

            try
            {
                var lstCountry = await _pcmsService.GetDefaultCountry();
                string result = JsonConvert.SerializeObject(lstCountry);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstCountry);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No country found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to GetDefaultCountry");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To Send Mail
        /// </summary>       
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/SendEmailNotification")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SendEmailNotification(EmailInput data)
        {
            string result = "success";
            try
            {
                EmailType emailType = new EmailType();
                var branchDetails = await _pcmsService.GetBranchByUserId(data.UserId);
                data.EmailIdConfig = await _pcmsService.GetEmailIdConfigByType(data.EmailType, branchDetails.BranchId);
                data.EmailConfig = await _pcmsService.GetDefaultConfiguration();

                if (await _notifactionService.SendEMail(data))
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }

                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Sending Email Failed");

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to send email notification.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        [HttpPost]
        [Route("api/Admin/SendPaymentInvoiceToUser")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SendPaymentInvoiceToUser(InvoiceMail invoiceMail)
        {
            string result = "success";
            try
            {
                await _invoiceService.SendPaymentInvoiceToUser(invoiceMail);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to send payment notification.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        /// <summary>
        ///To save invoice path
        /// </summary>
        /// <param name="Invoice Number"></param>
        /// <param name="Invoice Path"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/SavePublicUserPaymentInvoice/")]
        public async Task<HttpResponseMessage> SavePublicUserPaymentInvoice(PublicUserPaymentInvoiceInfo invoiceData)
        {
            string result = "success";
            try
            {
                await _invoiceService.SavePublicUserPaymentInvoice(invoiceData);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to save invoice path.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [Route("api/Admin/InsertUpdatePaypalSettings")]
        public async Task<HttpResponseMessage> InsertUpdatePaypalSettings(PayPalAccount payPal)
        {
            string result = "success";
            try
            {
                int paypalId = await _pcmsService.InsertUpdatePaypalSettings(payPal);
                var response = new HttpResponseMessage();
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save Paypal details");
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Unable to paypal data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/Admin/InsertUpdateCompany")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> InsertUpdateCompany(CompanyProfile companyProfile)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Country creation failed.");
                }
                int countryId = await _pcmsService.InsertUpdateCompanyDetails(companyProfile);
                var response = new HttpResponseMessage();
                if (countryId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save Company details");
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Unable to company data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                return response;
            }
        }

        [HttpGet]
        [Route("api/Admin/GetCompanyProfiles/{companyId}")]
        //[AllowAnonymous]
        public async Task<HttpResponseMessage> GetCompanyProfiles(int companyId)

        {
            try
            {
                var lstCompany = await _pcmsService.GetCompanyProfiles(companyId);
                string result = JsonConvert.SerializeObject(lstCompany);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Company found.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Company details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("api/Admin/GetPayPalAccount/{paypalId}")]
        //[AllowAnonymous]
        public async Task<HttpResponseMessage> GetPayPalAccount(int paypalId)

        {
            try
            {
                var lstCompany = await _pcmsService.GetPayPalAccount(paypalId);
                string result = JsonConvert.SerializeObject(lstCompany);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No PayPal Account found.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get PayPal Account details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add country
        /// </summary>
        /// <param name="Country"></param>
        /// <returns></returns>
        // POST: api/Country
        [Route("api/Admin/AddCountry")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> AddCountry(Countries Country)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Country creation failed.");
                }
                int countryId = await _pcmsService.AddCountry(Country);
                var response = new HttpResponseMessage();
                if (countryId== 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save Country details");
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
        /// To delete country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        // DELETE: api/Country/5
        [HttpPost]
        [Route("api/Admin/DeleteCountry/{countryId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteCountry(int countryId)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Country Deletion failed.");
                }
                int country = await _pcmsService.DeleteCountry(countryId);
                var response = new HttpResponseMessage();
                if (country == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete Country details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/DeleteState/{StateId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteState(int StateId)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "State Deletion failed.");
                }
                int stateId = await _pcmsService.DeleteState(StateId);
                var response = new HttpResponseMessage();
                if (stateId == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete State details. State Id: {0}", StateId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/DeleteService/{ServiceId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteService(int ServiceId)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Service Deletion failed.");
                }
                int serviceId = await _pcmsService.DeleteService(ServiceId);
                var response = new HttpResponseMessage();
                if (serviceId == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete Service details. Service Id: {0}", ServiceId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// Add a state details
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        // POST: api/Country
        [Route("api/Admin/AddState")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> AddState(States state)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "State creation failed.");
                }
                int countryId = await _pcmsService.AddState(state);

                var response = new HttpResponseMessage();
                if (countryId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add State details");
                if (ex.InnerException.Message.Contains("UNIQUE KEY") || ex.InnerException.Message.Contains("duplicate key row"))
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
        [Route("api/Admin/GetStateDetails/{stateId}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetStateDetails(int stateId)

        {
            try
            {
                var lstStates = await _pcmsService.RetrieveStates(stateId);
                string result = JsonConvert.SerializeObject(lstStates);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstStates);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No State found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get State details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To get sates by countryid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetStatesByCountryId/{countryId}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetStatesByCountryId(int countryId)
        {
            List<States> states = new List<States>();
            try
            {
                var statesList = await _pcmsService.GetStatesById(countryId);
                string result = JsonConvert.SerializeObject(statesList);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No states found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Countries");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        #region OfficeStffDetails

        /// <summary>
        /// method to add office staff details
        /// </summary>
        /// <param name="officeStaffRegistration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/SaveOfficeStaffDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        // POST: api/Admin/SaveOfficeStaffDetails   
        public async Task<HttpResponseMessage> SaveOfficeStaffDetails(OfficeStaffRegistration officeStaffRegistration)
        {
            try
            {
                string result = "Office staff registration success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save office staff registration");
                }

                int userId = await _officeStaffService.AddOfficeStaff(officeStaffRegistration);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(userId.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to post office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve required office staff profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetOfficeStaffProfile/{id}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetOfficeStaffProfile(int id)
        {
            OfficeStaffRegistration officeStaffRegistration = new OfficeStaffRegistration();
            try
            {
                var officeStaffDetails = await _officeStaffService.GetOfficeStaffProfile(id);
                if (officeStaffDetails != null)
                {
                    string result = JsonConvert.SerializeObject(officeStaffDetails);
                    if (result != null)
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                        return response;
                    }
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No office staff profile found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve all office staff details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAllOfficeStaffDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllOfficeStaffDetails()
        {
            List<OfficeStaffRegistration> officeStaffRegistration = new List<OfficeStaffRegistration>();
            try
            {
                var staffDetails = await _officeStaffService.GetAllOfficeStaffDetails();
                string result = JsonConvert.SerializeObject(staffDetails);
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
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        
        /// <summary>
        /// method to retrieve all office staff details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/GetAllOfficeStaffDetailsByLocation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllOfficeStaffDetailsByLocation(LocationSearchInputs inputs)
        {
            List<OfficeStaffRegistration> officeStaffRegistration = new List<OfficeStaffRegistration>();
            try
            {
                var staffDetails = await _officeStaffService.GetAllOfficeStaffDetailsByLocation(inputs);
                string result = JsonConvert.SerializeObject(staffDetails);
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
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to delete office staff details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/DeleteOfficeStaffDetails/{id}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteOfficeStaffDetails(int id)
        {
            try
            {
                int staffDetails = await _officeStaffService.DeleteOfficeStaffProfile(id);
                if (staffDetails == 0)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Country Deletion failed.");
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent("Delete successfully", System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete office staff details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);

            }

        }

        #endregion

        [HttpPost]
        [Route("api/Admin/SaveServiceDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveServiceDetails(Services serviceDetails)
        {
            try
            {
                string result = "Service Details saved successfully";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save Service Details.");
                }
                int serviceId = await _servicesService.AddServices(serviceDetails);
                var response = new HttpResponseMessage();
                if (serviceId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save Service details.");
                if (ex.InnerException.Message.Contains("UNIQUE KEY") || ex.InnerException.Message.Contains("duplicate key row"))
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
        [Route("api/Admin/RetrieveServiceDetails/{serviceId}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> RetrieveServiceDetails(int serviceId)
        
        {
            try
            {
                string result = "Service Details saved successfully";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save Service Details.");
                }
                var services = await _servicesService.RetrieveServices(serviceId);
                var response = Request.CreateResponse(HttpStatusCode.OK, services);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Service details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }



       


        /// <summary>
        /// To retrieve required city details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/City/5
        [Route("api/Admin/GetCity")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCity(string flag, string value)
        {
            Cities City = new Cities();
            //List<Cities> lstCity = new List<Cities>();
            try
            {

                var lstCity = await _pcmsService.RetrieveCities(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        #region created by silpa on branch dropdown
        [Route("api/Admin/GetAllBranch")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetAllBranch()
        {
            Cities City = new Cities();
            //List<Cities> lstCity = new List<Cities>();
            try
            {

                var lstCity = await _pcmsService.RetrieveAllBranches();
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion


        #region created by silpa on branch dropdown
        [Route("api/Admin/GetBranch")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetBranch(string flag, string value)
        {
            Cities City = new Cities();
            //List<Cities> lstCity = new List<Cities>();
            try
            {

                var lstCity = await _pcmsService.RetrieveBranches(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion


        #region created by silpa on branch dropdown
        [Route("api/Admin/GetBranchById")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetBranchById(string flag, string value)
        {
            Cities City = new Cities();
          
            try
            {

                var lstCity = await _pcmsService.RetrieveBranchesById(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion
        [Route("api/Admin/GetBranchByIds")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetBranchByIds(int BranchId)
        {
            Cities City = new Cities();
            //List<Cities> lstCity = new List<Cities>();
            try
            {

                var lstCity = await _pcmsService.RetrieveBranchById(BranchId);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        #region created by silpa on branch dropdown
        [Route("api/Admin/GetCountry")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCountry(string flag, string value)
        {
            Cities City = new Cities();
            //List<Cities> lstCity = new List<Cities>();
            try
            {

                var lstCity = await _pcmsService.RetrieveCountry(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion


        #region created by silpa on branch dropdown
        [Route("api/Admin/GetstateId")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetstateId(string flag, string value)
        {
            Cities City = new Cities();
            //List<Cities> lstCity = new List<Cities>();
            try
            {

                var lstCity = await _pcmsService.Retrievestates(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion



        #region created by silpa for city dp
        [Route("api/Admin/GetCityDetails")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCityDetails(string flag, string value)
        {
            Cities City = new Cities();
            try
            {

                var lstCity = await _pcmsService.RetrievecityDetails(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get City details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion

        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="cityModel"></param>
        /// <returns></returns>
        // POST: api/City
        [HttpPost]
        [Route("api/Admin/SaveCity")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveCity(Cities City)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "City creation failed.");
                }
                int CityId = await _pcmsService.AddCity(City);

                var response = new HttpResponseMessage();
                if (CityId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
               
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save City details.");
                if (ex.InnerException.Message.Contains("UNIQUE KEY") || ex.InnerException.Message.Contains("duplicate key row"))
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
        /// To delete city
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/City/5
        [HttpPost]
        [Route("api/Admin/DeleteCity/{CityId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteCity(int CityId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "City delete failed.");
                }
                int id = await _pcmsService.DeleteCity(CityId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete City details. City Id: {0}", CityId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To get cities by state id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetCityByStateId/{stateId}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCityByStateId(int stateId)
        {
            List<Cities> states = new List<Cities>();
            try
            {
                var cityList = await _pcmsService.GetCityById(stateId);
                string result = JsonConvert.SerializeObject(cityList);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No city found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get City details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required city details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/City/5
        [Route("api/Admin/GetCategory")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCategory(string flag, string value)
        {
            CategoryModel City = new CategoryModel();
            //List<CategoryModel> lstCity = new List<CategoryModel>();
            try
            {

                var lstCity = await _pcmsService.RetrieveCategory(flag, value);
                string result = JsonConvert.SerializeObject(lstCity);
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
                _logger.Error(ex, "Failed to get Category details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [Route("api/Admin/GetCaregiverServices")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCaregiverServices()
        {
            Services services = new Services();
            
            try
            {

                var lstservices = await _pcmsService.RetrieveCaregiverServices();
                string result = JsonConvert.SerializeObject(lstservices);
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
                _logger.Error(ex, "Failed to get Service details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="CategoryModel"></param>
        /// <returns></returns>
        // POST: api/SaveCategory
        [HttpPost]
        [Route("api/Admin/SaveCategory")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveCategory(CategoryModel CategoryModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Category creation failed.");
                }
                int CategoryId = await _pcmsService.AddCategory(CategoryModel);
                var response = new HttpResponseMessage();
                if (CategoryId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save Category details.");
                if (ex.InnerException.Message.Contains("UNIQUE KEY") || ex.InnerException.Message.Contains("duplicate key row"))
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
        /// To delete city
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/City/5
        [HttpPost]
        [Route("api/Admin/DeleteCategory/{CategoryId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteCategory(int CategoryId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Category delete failed.");
                }
                int id = await _pcmsService.DeleteCategory(CategoryId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete Category details. CategoryId: {0}", CategoryId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required city details
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        // GET: api/GetDesignation/0
        [HttpGet]
        [Route("api/Admin/GetDesignation/{designationId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetDesignation(int designationId)
        {
            //List<DesignationDetails> lstDesignation = new List<DesignationDetails>();
            try
            {
                var lstDesignation = await _pcmsService.RetrieveDesignation(designationId);
                string result = JsonConvert.SerializeObject(lstDesignation);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Designation details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        // POST: api/SaveDesignation
        [HttpPost]
        [Route("api/Admin/SaveDesignation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveDesignation(DesignationDetails designation)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation creation failed.");
                }
                int CategoryId = await _pcmsService.AddDesignation(designation);
                var response = new HttpResponseMessage();
                if (CategoryId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create Designation");
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
        /// To delete city
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        // DELETE: api/DeleteDesignation/1
        [HttpPost]
        [Route("api/Admin/DeleteDesignation/{designationId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteDesignation(int designationId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteDesignation(designationId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Designation details. designationId: {0}", designationId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required city details
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        // GET: api/GetDesignation/0
        [Route("api/Admin/GetQualification/{qualificationId}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetQualification(int qualificationId)
        {
            //List<QualificationDetails> lstQualification = new List<QualificationDetails>();
            try
            {

                var lstQualification = await _pcmsService.RetrieveQualification(qualificationId);
                string result = JsonConvert.SerializeObject(lstQualification);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Qualification not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Qualification details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="qualification"></param>
        /// <returns></returns>
        // POST: api/SaveDesignation
        [HttpPost]
        [Route("api/Admin/SaveQualification")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveQualification(QualificationDetails qualification)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation creation failed.");
                }
                int id = await _pcmsService.AddQualification(qualification);
                var response = new HttpResponseMessage();
                if (id == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create Qualification");
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
        /// To delete city
        /// </summary>
        /// <param name="QualificationId"></param>
        /// <returns></returns>
        // DELETE: api/DeleteDesignation/1
        [HttpPost]
        [Route("api/Admin/DeleteQualification/{qualificationId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteQualification(int qualificationId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteQualification(qualificationId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Designation details. designationId: {0}", qualificationId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        #endregion

        #region Work Shifts

        /// <summary>
        ///To add work shift
        /// </summary>
        /// <param name="workShift"></param>
        /// <returns></returns>
        [Route("api/Admin/AddWorkShift")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> AddWorkShift(WorkShifts workShift)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Work Shift creation failed.");
                }
                int workShiftId = await _pcmsService.AddWorkShift(workShift);
                var response = new HttpResponseMessage();
                if (workShiftId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create work shift");
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
        /// To retrieve work shift details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetWorkShiftDetails/{shiftId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetWorkShiftDetails(int shiftId)

        {
            try
            {
                var lstWorkShift = await _pcmsService.RetrieveWorkShift(shiftId);
                string result = JsonConvert.SerializeObject(lstWorkShift);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstWorkShift);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No work shift found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve work shift details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/DeleteWorkShift/{workShiftId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteWorkShift(int workShiftId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteWorkShift(workShiftId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Time Shift details. TimeShiftId: {0}", workShiftId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion


        #region Time Shifts

        /// <summary>
        ///To add time shift
        /// </summary>
        /// <param name="timeShift"></param>
        /// <returns></returns>
        [Route("api/Admin/AddTimeShift")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> AddTimeShift(ClientTimeShifts timeShift)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Time Shift creation failed.");
                }
                int timeShiftId = await _pcmsService.AddTimeShift(timeShift);

                var response = new HttpResponseMessage();
                if (timeShiftId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create time shift");
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
        [Route("api/Admin/DeleteClientTimeShiftDetail/{TimeShiftId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteClientTimeShiftDetail(int TimeShiftId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteClientShiftDetail(TimeShiftId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Time Shift details. TimeShiftId: {0}", TimeShiftId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve time shift details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetTimeShiftDetails/{timeShiftId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetTimeShiftDetails(int timeShiftId)

        {
            try
            {
                var lstTimeShift = await _pcmsService.RetrieveTimeShift(timeShiftId);
                string result = JsonConvert.SerializeObject(lstTimeShift);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstTimeShift);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No time shift found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve time shift details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve time shift details by client id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetTimeShiftDetailsByClientId/{clientId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetTimeShiftDetailsByClientId(int clientId)

        {
            try
            {
                var lstTimeShift = await _pcmsService.RetrieveTimeShiftByClientId(clientId);
                string result = JsonConvert.SerializeObject(lstTimeShift);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstTimeShift);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No time shift found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve time shift details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion

        #region Holidays

        /// <summary>
        ///To add holiday
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        [Route("api/Admin/AddHoliday")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> AddHoliday(Holidays holiday)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Holiday creation failed.");
                }
                int holidayId = await _pcmsService.AddHoliday(holiday);

                var response = new HttpResponseMessage();
                if (holidayId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create holiday");
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

        [Route("api/Admin/OverrideHoliday")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> OverrideHoliday()
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Holiday Override failed creation failed.");
                }
                int holidayId = await _pcmsService.OverrideHoliday();

                var response = new HttpResponseMessage();
                if (holidayId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create holiday");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist.", System.Text.Encoding.UTF8, "application/json");
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
        /// To retrieve holiday details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetHolidayPayDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetHolidayPayDetails()

        {
            try
            {
                var lstHoliday = await _pcmsService.RetrieveHolidayPayDetails();
                string result = JsonConvert.SerializeObject(lstHoliday);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstHoliday);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No holiday found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve holiday details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Admin/GetIntervalHours")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetIntervalHours()

        {
            try
            {
                var lstHoliday = await _pcmsService.RetrieveGetIntervalHours();
                string result = JsonConvert.SerializeObject(lstHoliday);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstHoliday);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No holiday found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve holiday details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        ///To add holiday
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        [Route("api/Admin/CreateHolidayPay")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> CreateHolidayPay(Holidays holiday)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Holiday creation failed.");
                }
                int holidayId = await _pcmsService.AddHolidayPay(holiday);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create holiday");
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
        ///To add interval hours
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        [Route("api/Admin/CreateIntervalHours")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> CreateIntervalHours(ClientTimeShifts shift)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Holiday creation failed.");
                }
                int holidayId = await _pcmsService.AddIntervalHours(shift);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create holiday");
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
        /// To retrieve holiday details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/GetHolidayDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetHolidayDetails(Holidays holidaySearchModel)

        {
            try
            {
                var lstHoliday = await _pcmsService.RetrieveHoliday(holidaySearchModel);
                string result = JsonConvert.SerializeObject(lstHoliday);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstHoliday);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No holiday found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve holiday details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// To delete city
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        // DELETE: api/DeleteDesignation/1
        [HttpPost]
        [Route("api/Admin/DeleteHoliday/{holidayId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteHoliday(int holidayId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteHoliday(holidayId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Designation details. designationId: {0}", holidayId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion

        #region invoice
        /// <summary>
        /// method to get invoice details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetUserPaymentInvoiceDetails/{invoiceNumber}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetUserPaymentInvoiceDetails(string invoiceNumber)
        {
            UserPaymentInvoiceModel userPaymentInvoice = new UserPaymentInvoiceModel();
            try
            {
                var invoiceDetails = await _invoiceService.GetUserPaymentInvoiceDetails(invoiceNumber);
                if (invoiceDetails != null)
                {
                    string result = JsonConvert.SerializeObject(invoiceDetails);
                    if (result != null)
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                        return response;
                    }
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No invoice details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Method to add user payment transaction details
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/SaveUserPaymentTransactions")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SaveUserPaymentTransactions(UserPaymentTransactionModel transactionModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save payment transaction details");
                }
                int transactionDetails = await _invoiceService.AddPaymentTransactionDetails(transactionModel);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add payment transaction details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        #endregion
        /// <summary>
        /// To retrieve required city details
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        // GET: api/GetDesignation/0
        [HttpGet]
        [Route("api/Admin/GetQuestions/{questionId}")]

        public async Task<HttpResponseMessage> GetQuestions(int questionId)

        {
            //List<QuestionareModel> questions = new List<QuestionareModel>();
            try
            {
                var questions = await _pcmsService.RetrieveQuestions(questionId);
                string result = JsonConvert.SerializeObject(questions);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Questions not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Designation details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("api/Admin/GetTestimonials/{testimonialId}")]
        public async Task<HttpResponseMessage> GetTestimonials(int testimonialId)
        {
            //List<QuestionareModel> questions = new List<QuestionareModel>();
            try
            {
                var testimonials = await _pcmsService.RetrieveTestimonials(testimonialId);
                string result = JsonConvert.SerializeObject(testimonials);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Testimonials not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Testimonials details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        // POST: api/SaveDesignation
        [HttpPost]
        [Route("api/Admin/AddQuestions")]
        public async Task<HttpResponseMessage> AddQuestions(QuestionareModel questionare)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation creation failed.");
                }
                int questionId = await _pcmsService.AddQuestions(questionare);
                var response = new HttpResponseMessage();
                if (questionId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create Designation");
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
        /// To delete city
        /// </summary>
        /// <param name="designationId"></param>
        /// <returns></returns>
        // DELETE: api/DeleteDesignation/1
        [HttpPost]
        [Route("api/Admin/DeleteQuestions/{questionId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteQuestions(int questionId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteQuestions(questionId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Question. questionId: {0}", questionId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/DeleteTestimonial/{testimonialId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteTestimonial (int testimonialId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation delete failed.");
                }
                int id = await _pcmsService.DeleteTestimonial(testimonialId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Testimonial. questionId: {0}", testimonialId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Admin/InsertUpdateTestimonials")]
        public async Task<HttpResponseMessage> InsertUpdateTestimonials(Testimonial testimonial)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation creation failed.");
                }
                int testimonialId = await _pcmsService.InsertUpdateTestimonials(testimonial);
                var response = new HttpResponseMessage();
                if (testimonialId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create Designation");
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
        [Route("api/Admin/SelectCaretakersByCaretakerStatus/{status}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SelectCaretakersByCaretakerStatus(int status)
        {
            try
            {
                var lstRegdCaretakers = await _pcmsService.SelectRegisteredCaretakers(status);
                string result = JsonConvert.SerializeObject(lstRegdCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstRegdCaretakers);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        
        [HttpPost]
        [Route("api/Admin/SelectCaretakersByLocation/{status}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SelectCaretakersByLocation(int status,LocationSearchInputs inputs)
        {
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
                var lstRegdCaretakers = await _pcmsService.SelectRegisteredCaretakersByLocation(status,inputs);
                string result = JsonConvert.SerializeObject(lstRegdCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstRegdCaretakers);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("api/Admin/SelectCaretakers/{status}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SelectCaretakers(int status)
        {
            try
            {
                var lstRegdCaretakers = await _pcmsService.SelectCaretakers(status);
                string result = JsonConvert.SerializeObject(lstRegdCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstRegdCaretakers);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Admin/RejectCaretakerApplication")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> RejectCaretakerApplication(RejectCareTaker rejectCareTaker)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int id = await _pcmsService.RejectCaretakerApplication(rejectCareTaker);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}", rejectCareTaker.Userid);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/DeleteCaretaker/{userId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteCaretaker(int userId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int id = await _pcmsService.DeleteCaretaker(userId);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}", userId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/ApproveCaretaker")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> ApproveCaretaker(ApproveCaretaker approveCaretaker)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to approve the Caregiver.");
                }
                int id = await _pcmsService.ApproveCaretaker(approveCaretaker);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to approve the Caregiver. UserId: {0}", approveCaretaker.CareTakerId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/ChangeUserStatus/{userId}/{status}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> ChangeUserStatus(int userId, int status)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int id = await _pcmsService.ChangeUserStatus(userId, status);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}", userId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/VerifyEmail/{userId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> VerifyEmail(int userId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int id = await _pcmsService.VerifyEmail(userId);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}", userId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to add roles
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/SaveRoles")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveRoles(Roles roles)
        {
                var response = new HttpResponseMessage();
            try
            {

                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to add roles");
                }
                int rolesDetails = await _pcmsService.AddRoles(roles);
                if (rolesDetails == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add roles");
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Data already exist. Please enter different data.", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }


        }

        /// <summary>
        /// method to get roles
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetRoles/{roleId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetRoles(int roleId)
        {
            try
            {

                 var lstRoles = await _pcmsService.RetrieveRolesById(roleId);
                string result = JsonConvert.SerializeObject(lstRoles);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "roles not found not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get role details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        [HttpGet]
        [Route("api/Admin/GetCaretakerType")]
        public async Task<HttpResponseMessage> GetCaretakerType()
        {
            
            try
            {

                 var ctType = await _pcmsService.GetCaretakerType();
                string result = JsonConvert.SerializeObject(ctType);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "roles not found not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get role details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        /// <summary>
        /// Selects the privileges assigned against a Role
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetRolePrivileges/{RoleId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetRolePrivileges(int RoleId)
        {
            try
            {

                var lstRoles = await _pcmsService.SelectRolePrivileges(RoleId);
                string result = JsonConvert.SerializeObject(lstRoles);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "roles not found not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get role details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        [HttpPost]
        [Route("api/Admin/SaveRolePrivileges")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveRolePrivileges(SaveRolePrivileges saveRolePrivileges)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to approve the Caregiver.");
                }
                int id = await _pcmsService.SaveRolePrivileges(saveRolePrivileges);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Save the Role Privileges. RoleId: {0}", saveRolePrivileges.RoleId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/GetRolePrivilege")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetRolePrivilege(GetRolePrivilegeModel getRolePrivilege)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Roles deletion failed.");
                }
                var rolePrivileges = await _adminService.GetRolePrivilege(getRolePrivilege);
                var response = Request.CreateResponse(HttpStatusCode.OK, rolePrivileges);
                //response.Content = new StringContent(hasAccess.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get roles privilede. roleId: {0}", getRolePrivilege.RoleId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// method to delete roles
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/DeleteRoles/{roleId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteRoles(int roleId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Roles deletion failed.");
                }
                int id = await _pcmsService.DeleteRoles(roleId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete roles. roleId: {0}", roleId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/GetBookingHistoryList")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingHistoryList(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingHistoryList(bookingHistorySearch);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking History details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking History details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/GetBookingInvoiceList")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingInvoiceList(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingInvoiceList(bookingHistorySearch);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking History details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking History details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        
        [HttpGet]
        [Route("api/Admin/GetBookingInvoiceListforUserDashBoard/{publicUserId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingInvoiceListforUserDashBoard(int publicUserId)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingInvoiceListforUserDashBoard(publicUserId);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking History details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking History details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Admin/GetBookingHistoryListById/{publicUserId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingHistoryListById(int publicUserId)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingHistoryListById(publicUserId);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking History details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking History details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/GetBookingHistoryListForInvoiceGeneration")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingHistoryListForInvoiceGeneration(BookingHistorySearch bookingHistorySearch)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingHistoryListForInvoiceGeneration(bookingHistorySearch);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking History details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking History details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }



        [HttpPost]
        [Route("api/Admin/GetCaretakerBookings")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async  Task<HttpResponseMessage> GetCaretakerBookings(CaretakerWiseSearchReport bookingHistorySearch)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetCaretakerBookings(bookingHistorySearch);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking History details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking History details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/GetBookingHistoryReport")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingHistoryReport(CaretakerBookingReportModel caretakerBookingReport)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingHistoryReport(caretakerBookingReport);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("api/Admin/GetBookingHistoryDetail/{BookingId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingHistoryDetail(int BookingId)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetBookingHistoryDetail(BookingId);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Admin/GetAdminDashboardBookingHistoryDetail/{BookingId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminDashboardBookingHistoryDetail(int BookingId)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetAdminDashboardBookingHistoryDetail(BookingId);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve paymentdetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetPaymentDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetPaymentDetails()
        {
            List<PaymentHistory> paymentDetails= new List<PaymentHistory>();
            try
            {
                var paymentHistory = await _invoiceService.GetPaymentDetails();
                string result = JsonConvert.SerializeObject(paymentHistory);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        /// Method to retrieve payment details using advanced search
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Home/SearchPaymentDetails")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SearchPaymentDetails(PaymentAdvancedSearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.SearchPaymentDetails(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
        [Route("api/Home/SearchClientInvoiceReport")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SearchClientInvoiceReport(PaymentAdvancedSearch inputs)
        {

            try
            {
                var invoiceDetails = await _invoiceService.SearchClientInvoiceReport(inputs);
                string result = JsonConvert.SerializeObject(invoiceDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Home/SearchClientInvoiceReportSummary")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SearchClientInvoiceReportSummary(PaymentAdvancedSearch inputs)
        {

            try
            {
                var invoiceSummaryDetails = await _invoiceService.SearchClientInvoiceReportSummary(inputs);
                string result = JsonConvert.SerializeObject(invoiceSummaryDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Home/GetClientScheduledDetails")]
        public async Task<HttpResponseMessage> GetClientScheduledDetails(PaymentAdvancedSearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.GetClientScheduledDetails(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Home/GetClientScheduledDetailsByBranchWise")]
        public async Task<HttpResponseMessage> GetClientScheduledDetailsByBranchWise(PaymentAdvancedSearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.GetClientScheduledDetailsByBranchWise(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Home/GetInvoiceGenerationDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetInvoiceGenerationDetails(PaymentAdvancedSearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.GetInvoiceGenerationDetails(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/GetUserInvoiceGenerationDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetUserInvoiceGenerationDetails(BookingHistorySearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.GetUserInvoiceGenerationDetails(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Home/SearchClientScheduleReoprt")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SearchClientScheduleReoprt(PaymentAdvancedSearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.SearchClientScheduleReoprt(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Home/SearchCaretakerPayHoursReoprt")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SearchCaretakerPayHoursReoprt(PaymentAdvancedSearch inputs)
        {

            try
            {
                var paymentDetails = await _invoiceService.SearchCaretakerPayHoursReoprt(inputs);
                string result = JsonConvert.SerializeObject(paymentDetails);
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
                _logger.Error(ex, "Failed to get SearchCaretakerPayHoursReoprt");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/SearchUserPaymentReport")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SearchUserPaymentReport(PaymentReport inputs)
        {

            try
            {
                var paymentReport = await _invoiceService.SearchUserPaymentReport(inputs);
                string result = JsonConvert.SerializeObject(paymentReport);
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
                _logger.Error(ex, "Failed to get Invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve booking notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAdminNotification")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminNotification()
        {
            try
            {
               var notificationDetails = await _invoiceService.GetAdminNotification();
                string result = JsonConvert.SerializeObject(notificationDetails);
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

        /// <summary>
        /// method to retrieve booking list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAdminBookingList")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminBookingList()
        {
            try
            {
                var bookingDetails = await _invoiceService.GetAdminBookingList();
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
                _logger.Error(ex, "Failed to get Payment details");
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
        /// method to retrieve scheduling list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAdminSchedulingList")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminSchedulingList()
        {
            try
            {
                var schedulingDetails = await _invoiceService.GetAdminSchedulingList();
                string result = JsonConvert.SerializeObject(schedulingDetails);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        /// method to retrieve bookingList by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetBookingDetailsById/{bookingId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBookingDetailsById(int bookingId)
        {
            AdminBookingDetails bookingDetails = new AdminBookingDetails();
            try
            {
                bookingDetails = await _invoiceService.GetBookingDetailsById(bookingId);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        /// method to retrieve bookingList by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetSchedulingDetailsById/{schedulingId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetSchedulingDetailsById(int schedulingId)
        {
            AdminSchedulingDetails schedulingDetails = new AdminSchedulingDetails();
            try
            {
                schedulingDetails = await _invoiceService.GetSchedulingDetailsById(schedulingId);
                string result = JsonConvert.SerializeObject(schedulingDetails);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        /// method to send otp to email
        /// </summary>
        /// <param name="input"></param>
        //[ApiAuthorize("blogger_user")]
        [HttpPost]
        [Route("api/Admin/SendInvoice")]     
        public async Task<HttpResponseMessage> SendInvoice(InvoiceMail invoiceMail)
        {
            try
            {
                EmailInput inputs = new EmailInput();
                inputs.EmailId = invoiceMail.EmailId;
                inputs.Subject = "Payment Link";
                inputs.Body = "";
                inputs.EmailType = EmailType.UserPayment;
                inputs.Body += "<br /><br />Please click on the following link for payment.<br/>";
                inputs.Body += invoiceMail.paymentLink;
                inputs.Body += "<br /><br />Thanks";
                var branchDetails = _pcmsService.GetBranchByUserId(1);//need a user
                inputs.EmailIdConfig = await _pcmsService.GetEmailIdConfigByType(inputs.EmailType, branchDetails.Result.BranchId);
                //inputs.EmailIdConfig = await _pcmsService.GetEmailIdConfigByType(inputs.EmailType);
                inputs.EmailConfig = await _pcmsService.GetDefaultConfiguration();
                bool result = await _notifactionService.SendEMail(inputs);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send otp to email.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/GenerateInvoice")]
        public async Task<HttpResponseMessage> GenerateInvoice(InvoiceMail invoiceMail)
        {
           
            try
            {
                int invoice = await _invoiceService.GenerateInvoice(invoiceMail);
                string result = JsonConvert.SerializeObject(invoice);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        [Route("api/Admin/AddPaymentInvoiceDetails")]
        public async Task<HttpResponseMessage> AddPaymentInvoiceDetails(InvoiceSearchInpts invoiceMail)
        {

            try
            {
                int invoice = await _invoiceService.AddPaymentInvoiceDetails(invoiceMail);
                string result = JsonConvert.SerializeObject(invoice);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        [Route("api/Admin/GetUserDetail/{BookingId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetUserDetail(int BookingId)
        {
            try
            {
                var lstBookingHistory = await _adminService.GetUserDetail(BookingId);
                string result = JsonConvert.SerializeObject(lstBookingHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstBookingHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No registered Caregiver details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Registered Caregiver details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/ChangeBookingStatus")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> ChangeBookingStatus(BookingStatusUpdate bookingStatus)
        {
            try
            {
             
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int id = await _pcmsService.ChangeBookigStatus(bookingStatus.userId, bookingStatus.status, bookingStatus.SiteUrl,bookingStatus.Reason);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}",bookingStatus.userId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Admin/DownloadDbBackup/{path}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DownloadDbBackup(string path)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                string downloadPath =  _adminService.DownloadDbBackup(path);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if(downloadPath !=null)
                {
                    response.Content = new StringContent(downloadPath, System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response.Content = new StringContent("Unable to download", System.Text.Encoding.UTF8, "application/json");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to download db backup", path);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// method to retrieve required office staff profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAdminProfile/{id}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminProfile(int id)
        {
            UsersDetails adminProfile = new UsersDetails();
            try
            {
                var adminDetails =await _pcmsService.GetAdminProfile(id);
                if (adminDetails != null)
                {
                    string result = JsonConvert.SerializeObject(adminDetails);
                    if (result != null)
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                        return response;
                    }
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No office staff profile found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/UpdateAdminProfile")]
        //[BasicAuthentication("Public User")]
        public async Task<HttpResponseMessage> UpdateAdminProfile(UsersDetails adminDetails)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Update user failed");
                }
                int UserDetailId = await _pcmsService.UpdateAdminProfile(adminDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update user details. User Id: {0}", adminDetails.UserRegnId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve required office staff profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
      
        [HttpPost]
        [Route("api/Admin/UpdateUserEmail")]
        public async Task<HttpResponseMessage> UpdateUserEmail(UsersDetails usersDetails)
        {

            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int id = usersDetails.UserRegnId;
                string emailId = usersDetails.EmailAddress;
                int resultid = await _pcmsService.UpdateUserEmail(id, emailId);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }
        /// <summary>
        /// method to retrieve required office staff profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/Admin/LoadPhoneCodeByCountryId/{countryID}")]

        public async Task<HttpResponseMessage> LoadPhoneCodeByCountryId(int countryID)
        {

            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
             
                string resultid = await _pcmsService.LoadPhoneCodeByCountryId(countryID);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(resultid, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        /// <summary>
        /// method to add resident details
        /// </summary>
        /// <param name="Resident Details"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("api/Admin/SaveResidentDetails")]
       
        public async Task<HttpResponseMessage> SaveResidentDetails(Resident residentDetails)
        {
            try
            {
                string result = "Details successfully saved";
                var response = new HttpResponseMessage();
                if (result == null)
                {
                    
                    //return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save Resident Details");
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Not Found", System.Text.Encoding.UTF8, "application/json");
                }

                int userId = await _pcmsService.SaveResidentDetails(residentDetails);
               
                if (userId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Name", System.Text.Encoding.UTF8, "application/json");
                }
                else if (userId == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent("OK", System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to post resident details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// method to get resident list
        /// </summary>
        /// <param name="residentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetResidentDetails")]
        public async Task<HttpResponseMessage> GetResidentDetails()
        {
            try
            {

                var lstResident = await _pcmsService.RetrieveResidentDetails();
                string result = JsonConvert.SerializeObject(lstResident);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Residents not found ");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get resident details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }

        /// <summary>
        /// method to get resident list
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetResidentDetailsById/{clientId}")]
        public async Task<HttpResponseMessage> GetResidentDetailsById(int clientId)
        {
            try
            {

                var lstResident = await _pcmsService.RetrieveResidentDetailsById(clientId);
                string result = JsonConvert.SerializeObject(lstResident);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Residents not found ");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get resident details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }
        [HttpPost]
        [Route("api/Admin/DeleteResident/{residentId}")]
        public async Task<HttpResponseMessage> DeleteResident(int residentId)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Resident Deletion failed.");
                }
                int residentID = await _pcmsService.DeleteResident(residentId);
                var response = new HttpResponseMessage();
                if (residentID == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete Resident details. Resident Id: {0}", residentId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        // POST: api/SaveDesignation
        [HttpPost]
        [Route("api/Admin/AddEmailConfiguration")]
        public async Task<HttpResponseMessage> AddEmailConfiguration(EmailConfiguration emailConfiguration)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation creation failed.");
                }
                int questionId = await _pcmsService.AddEmailConfiguration(emailConfiguration);
                var response = new HttpResponseMessage();
             

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add email configuration");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        ///To add Email type configuration
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        // POST: api/SaveDesignation
        [HttpPost]
        [Route("api/Admin/AddEmailTypeConfiguration")]
        public async Task<HttpResponseMessage> AddEmailTypeConfiguration(EmailTypeConfiguration emailTypeConfiguration)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Designation creation failed.");
                }
                int questionId = await _pcmsService.AddEmailTypeConfiguration(emailTypeConfiguration);
                var response = new HttpResponseMessage();
                if (questionId == 10001)
                {
                    response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add email configuration");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve booking list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetConfigList")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetConfigList()
        {
            try
            {
                var configList = await _pcmsService.GetConfigList();
                string result = JsonConvert.SerializeObject(configList);
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
                _logger.Error(ex, "Failed to get config details");
               
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                    return response;
                
            }
        }

        /// <summary>
        /// method to retrieve email type config
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetEmailTypeConfig")]
        public async Task<HttpResponseMessage> GetEmailTypeConfig()
        {
            try
            {
                var configList = await _pcmsService.GetEmailTypeConfig();
                string result = JsonConvert.SerializeObject(configList);
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
                _logger.Error(ex, "Failed to get config details");

                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                return response;

            }
        }

        /// <summary>
        /// method to retrieve booking list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/SetConfig/{configId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SetConfig(int configId)
        {
            try
            {
                var configList = await _pcmsService.SetConfig(configId);
                string result = JsonConvert.SerializeObject(configList);
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
                _logger.Error(ex, "Failed to set config details");

                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                return response;

            }
        }

        /// <summary>
        /// To delete city
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/City/5
        [HttpPost]
        [Route("api/Admin/DeleteConfigDetails/{configId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteConfigDetails(int configId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Config delete failed.");
                }
                int id = await _pcmsService.DeleteConfigDetails(configId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete Config details. City Id: {0}", configId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }       

        /// <summary>
        /// Delete email type config
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/DeleteEmailTypeConfig/{configId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteEmailTypeConfig(int configId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Config delete failed.");
                }
                int id = await _pcmsService.DeleteEmailTypeConfig(configId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete Config details. City Id: {0}", configId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/GetBookingPayriseList")]
        public async Task<HttpResponseMessage> GetBookingPayriseList(BookingPayriseModel bookingPayriseModel)
        {
            try
            {
                var BookingPayriseHistory = await _pcmsService.GetBookingPayriseList(bookingPayriseModel);
                string result = JsonConvert.SerializeObject(BookingPayriseHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, BookingPayriseHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Booking Payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Booking Payrise details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Admin/GetInvoicePayriseList")]
        public async Task<HttpResponseMessage> GetInvoicePayriseList(InvoicePayriseModel invoicePayriseModel)
        {
            try
            {
                var lstInvoicePayriseHistory = await _pcmsService.GetInvoicePayriseList(invoicePayriseModel);
                string result = JsonConvert.SerializeObject(lstInvoicePayriseHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstInvoicePayriseHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Invoice Payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Invoice Payrise details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/GetPayrollPayriseList")]
        public async Task<HttpResponseMessage> GetPayrollPayriseList(PayrollPayriseModel payrollPayriseModel)
        {
            try
            {
                var lstPayrollPayriseHistory = await _pcmsService.GetPayrollPayriseList(payrollPayriseModel);
                string result = JsonConvert.SerializeObject(lstPayrollPayriseHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstPayrollPayriseHistory);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Payroll Payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Payroll Payrise details.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/DeleteBookingPayrise/{bookingPayriseId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteBookingPayrise(int bookingPayriseId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Payrise delete failed.");
                }
                int id = await _pcmsService.DeleteBookingPayrise(bookingPayriseId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete Payrise details. BookingPayriseId: {0}", bookingPayriseId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/DeleteInvoicePayrise/{invoicePayriseId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteInvoicePayrise(int invoicePayriseId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Payrise delete failed.");
                }
                int id = await _pcmsService.DeleteBookingPayrise(invoicePayriseId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete Payrise details. BookingPayriseId: {0}", invoicePayriseId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/Admin/DeletePayrollPayrise/{payrollPayriseId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeletePayrollPayrise(int payrollPayriseId)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Payrise delete failed.");
                }
                int id = await _pcmsService.DeletePayrollPayrise(payrollPayriseId);
                var response = new HttpResponseMessage();
                if (id == 10002)
                {
                    response = Request.CreateResponse(HttpStatusCode.Ambiguous);
                    response.Content = new StringContent("Duplicate", System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Delete Payrise details. PayrollPayriseId: {0}", payrollPayriseId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Admin/GetAllBookingPayriseDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllBookingPayriseDetails()
        {

            try
            {
                var payriseDetails = await _pcmsService.GetAllBookingPayriseDetails();
                string result = JsonConvert.SerializeObject(payriseDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Admin/GetAllInvoicePayriseDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllInvoicePayriseDetails()
        {

            try
            {
                var payriseDetails = await _pcmsService.GetAllInvoicePayriseDetails();
                string result = JsonConvert.SerializeObject(payriseDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("api/Admin/GetAllPayrollPayriseDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllPayrollPayriseDetails()
        {

            try
            {
                var payriseDetails = await _pcmsService.GetAllPayrollPayriseDetails();
                string result = JsonConvert.SerializeObject(payriseDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAllUserDetails")]
        public async Task<HttpResponseMessage> GetAllUserDetails()
        {

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
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/GetAllUserDetailsByLocation")]
        public async Task<HttpResponseMessage> GetAllUserDetailsByLocation(LocationSearchInputs inputs)
        {

            try
            {
                var userDetails = await _pcmsService.GetAllUserDetailsByLocation(inputs);
                string result = JsonConvert.SerializeObject(userDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <param name="usersDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/UpdateUserDetails")]
        public async Task<HttpResponseMessage> UpdateUserDetails(UsersDetails usersDetails)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.UpdateUserDetails(usersDetails);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client mapping failed.");
                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to map Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetAllCaretakerDocuments")]
        public async Task<HttpResponseMessage> GetAllCaretakerDocuments()
        {

            try
            {
                var docs = await _pcmsService.GetAllCaretakerDocuments();
                string result = JsonConvert.SerializeObject(docs);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No payrise details found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// DB migration purpose only
        /// </summary>
        /// <param name="usersDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/UpdateCaretakerDocuments")]
        public async Task<HttpResponseMessage> UpdateCaretakerDocuments(DocumentsList doc)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.UpdateCaretakerDocuments(doc);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client mapping failed.");
                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to map Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        /// <summary>
        /// method to retrieve booking list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/GetAdminBookingListByLocation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminBookingListByLocation(LocationSearchInputs inputs)
        {
            try
            {
                var bookingDetails = await _invoiceService.GetAdminBookingListByLocation(inputs);
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
                _logger.Error(ex, "Failed to get Payment details");
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
        /// method to retrieve booking notification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Admin/GetAdminNotificationByLocation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAdminNotificationByLocation(LocationSearchInputs inputs)
        {
            try
            {
                var notificationDetails = await _invoiceService.GetAdminNotificationByLocation(inputs);
                string result = JsonConvert.SerializeObject(notificationDetails);
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


        /// <summary>
        /// method to get role details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Admin/GetUserRoleDetails/{userId}")]
        public async Task<HttpResponseMessage> GetUserRoleDetails(int userId)
        {
            try
            {
                var userDetails = await _pcmsService.GetUserRoleDetails(userId);
                string result = JsonConvert.SerializeObject(userDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Role not found ");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Role details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

        }




        [HttpPost]
        [Route("api/Admin/GetBranchByLocation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetBranchByLocation(LocationSearchInputs inputs)
        {
            try
            {
                var BranchList = await _pcmsService.GetBranchesByLocation(inputs);
                string result = JsonConvert.SerializeObject(BranchList);
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
                _logger.Error(ex, "Failed to get office staff details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
    }
}


