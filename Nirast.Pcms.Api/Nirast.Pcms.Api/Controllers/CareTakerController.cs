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
using System.Web.Http.Cors;

namespace Nirast.Pcms.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CareTakerController : ApiController
    {
        private IPCMSService _pcmsService;
        private IPCMSLogger _logger;
        public CareTakerController(IPCMSService pcmsService, IPCMSLogger logger)
        {
            _pcmsService = pcmsService;
            _logger = logger;
        }
        /// <summary>
        /// To retrieve careTaker details
        /// </summary>
        /// <returns></returns>
        // GET: api/CareTakerRegistration
        public HttpResponseMessage Get()
        {
            CareTakerRegistrationModel CareTakerRegistration = new CareTakerRegistrationModel();
            List<CareTakerRegistrationModel> lstCountry = new List<CareTakerRegistrationModel>();
            try
            {
                lstCountry.Add(CareTakerRegistration);
                string result = JsonConvert.SerializeObject(lstCountry);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No careTaker found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Caregiver");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("Api/CareTaker/GetCareTakerProfile/{id}")]
        //[BasicAuthentication("Caretaker", "Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetCareTakerProfile(int id)
        {
            CareTakerRegistrationModel CareTakerRegistration = new CareTakerRegistrationModel();
            //CareTakerRegistrationModel lstCaretakers;
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCaretakerDetails(id);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("Api/CareTaker/GetAllBookingSchedulingData")]
        public async Task<HttpResponseMessage> GetAllBookingSchedulingData(CalenderEventInput calenderEventInput)
        {
            try
            {
                var lstCaretakers = await _pcmsService.GetAllBookingSchedulingData(calenderEventInput);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("Api/CareTaker/GetCareTakerDetails/{id}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCareTakerDetails(int id)
        {
            CareTakerRegistrationModel CareTakerRegistration = new CareTakerRegistrationModel();
            CareTakerRegistrationModel lstCaretakers;
            try
            {
                lstCaretakers = await _pcmsService.RetrieveCaretakerDetails(id);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add careTaker
        /// </summary>
        /// <param name="CareTakerRegistration"></param>
        /// <returns></returns>
        // POST: api/CareTakerRegistration
        [Route("Api/CareTaker/SaveCareTaker")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> SaveCareTaker(CareTakerRegistrationModel CareTakerRegistration)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Caregiver registration  failed.");
                }
                int userId = await _pcmsService.AddCareTaker(CareTakerRegistration);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(userId.ToString(), System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [Route("api/CareTaker/GetCaretakerPayRiseRates/{caretakerId}")]
        public async Task<HttpResponseMessage> GetMappedCaretakerPayRiseRates(int caretakerId)
        {
            try
            {
                var mappedCaretakerRates = await _pcmsService.GetCaretakerPayRiseRates( caretakerId);
                string result = JsonConvert.SerializeObject(mappedCaretakerRates);
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
                _logger.Error(ex, "Failed to get mapped raretaker rates");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("api/CareTaker/GetCaretakerPayRiseRatesonDateChange")]
        public async Task<HttpResponseMessage> GetCaretakerPayRiseRatesonDateChange(PayriseData bookingPayRiseData)
        {
            try
            {
                var mappedCaretakerRates = await _pcmsService.GetCaretakerPayRiseRatesonDateChange(bookingPayRiseData.Date, bookingPayRiseData.CaretakerId);
                string result = JsonConvert.SerializeObject(mappedCaretakerRates);
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
                _logger.Error(ex, "Failed to get mapped raretaker rates");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/CareTaker/SaveCareTakerPayRise")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveCareTakerPayRise(List<CareTakerServices> careTaker)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.SaveCareTakerPayRise(careTaker);

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

        [HttpGet]
        [Route("CareTakerRegistration/Test")]
        public string Test()
        {
            return "Test Message";
        }

        /// <summary>
        /// To edit careTaker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/CareTakerRegistration/5
        public HttpResponseMessage Put(int id)
        {
            try
            {
                string result = "Successfully Updated";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, " Updation failed.");
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update Caregiver");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To delete careTaker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/CareTakerRegistration/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Caregiver Registration Deletion failed.");
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete Caregiver");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [Route("api/Caretaker/GetCaretakerProfileId")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCaretakerProfileId()
        {
            string profileId = null;
            try
            {

                profileId = await _pcmsService.CaretakerProfileId();
                //string result = JsonConvert.SerializeObject(lstCity);
                if (profileId != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(profileId, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get caeretakerprofileId");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("api/CareTaker/GetCareTakerListByCategory/{CategoryId}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetCareTakerListByCategory(int categoryId)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListByCategory(categoryId);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CareTaker/GetCareTakerListByCategoryAndLocation/{CategoryId}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetCareTakerListByCategoryAndLocation(int categoryId , LocationSearchInputs inputs)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListByCategoryAndLocation(categoryId,inputs);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("api/CareTaker/GetCaretakersByService/{serviceid}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetCaretakersByService(int serviceid)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListByService(serviceid);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("api/CareTaker/GetCareTakerListByCategoryAndClientID/{CategoryId}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetCareTakerListByCategoryAndClientId(int categoryId,int clientId)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListByCategoryAndClientId(categoryId,clientId);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("api/CareTaker/GetCareTakerListByCategoryAndDateTime/{CategoryId}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetCareTakerListByCategoryAndDateTime(int categoryId,string startDateTime,int hours,int clientId)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListByCategoryAndDate(categoryId,startDateTime,hours,clientId);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [Route("api/CareTaker/GetAvailableCareTakerListByCategoryAndDateTime/{CategoryId}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetAvailableCareTakerListByCategoryAndDateTime(int categoryId, string startDateTime, int hours, int clientId,int Workshift)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                _logger.Error("API receive start " + DateTime.Now.ToString() + "RetrieveAvailableCareTakerListByCategoryAndDate -"+ categoryId+" - "+ startDateTime + " - " + hours + " - " + clientId + " - " + Workshift);
                var lstCaretakers = await _pcmsService.RetrieveAvailableCareTakerListByCategoryAndDate(categoryId, startDateTime,  hours, clientId, Workshift);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    _logger.Error("API receive end " + DateTime.Now.ToString() + "RetrieveAvailableCareTakerListByCategoryAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - " + clientId + " - " + Workshift);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [Route("api/CareTaker/GetAvailableCareTakerListforPublicUser/{CategoryId}")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetAvailableCareTakerListforPublicUser(int categoryId, string startDateTime, int hours, int Workshift)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                _logger.Error("API receive start " + DateTime.Now.ToString() + "RetrieveAvailableCareTakerListByCategoryAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - "   + Workshift);
                var lstCaretakers = await _pcmsService.RetrieveAvailableCareTakerListForPublicUser(categoryId, startDateTime, hours , Workshift);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    _logger.Error("API receive end " + DateTime.Now.ToString() + "RetrieveAvailableCareTakerListByCategoryAndDate -" + categoryId + " - " + startDateTime + " - " + hours + " - "  + " - " + Workshift);
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [HttpPost]
        [Route("api/CareTaker/GetAvailableCareTakerListReport")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetAvailableCareTakerListReport(PaymentAdvancedSearch inputs)
        {
            //List<CaretakerAvailableReport> lstCaretakers = new List<CaretakerAvailableReport>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveAvailableCareTakerListReport(inputs);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }



        /// <summary>
        /// To retrieve required careTaker details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CareTakerRegistration/5
        [HttpPost]
        [Route("api/CareTaker/GetCommissionListReport")]
        //[BasicAuthentication("Administrator")]
        public async Task<HttpResponseMessage> GetCommissionListReport(PaymentAdvancedSearch inputs)
        {
            //List<CaretakerAvailableReport> lstCaretakers = new List<CaretakerAvailableReport>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCommissionReport(inputs);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }









        /// <summary>
        /// To retrieve required careTaker list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/RetrieveCareTakerListForDdl/
        [Route("api/CareTaker/RetrieveCareTakerListForDdl")]
        public async Task<HttpResponseMessage> GetCareTakerList()
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListForDdl();
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required careTaker list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: api/RetrieveCareTakerListForDdl/
        [HttpPost]
        [Route("api/CareTaker/RetrieveCareTakerListForDdlByLocation")]
        public async Task<HttpResponseMessage> GetCareTakerListByLocation(LocationSearchInputs inputs)
        {
            //List<CareTakers> lstCaretakers = new List<CareTakers>();
            try
            {
                var lstCaretakers = await _pcmsService.RetrieveCareTakerListForDdlByLocation(inputs);
                string result = JsonConvert.SerializeObject(lstCaretakers);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Caregiver found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load Caregiver details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// method to retrieve client scheduling details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/CareTaker/GetSchedulingDetails/{caretakerId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetSchedulingDetails(int caretakerId)
        {
            List<ClientScheduling> clientSchedulingDetails = new List<ClientScheduling>();
            try
            {
                var schedulingDetails = await _pcmsService.GetSchedulingDetails(caretakerId);
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
        /// method to retrieve booking details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CareTaker/GetUserBookingDetails/{caretakerId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetUserBookingDetails(CaretakerScheduleListSearch caretakerBookingListSearch)
        {
            try
            {
                var bookingDetails = await _pcmsService.GetUserBookingDetails(caretakerBookingListSearch);
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
        /// method to retrieve notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/CareTaker/GetNotification/{caretakerId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetNotification(int caretakerId)
        {
            List<Notification> notificationDetails = new List<Notification>();
            try
            {
                var notification = await _pcmsService.GetNotification(caretakerId);
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

        /// <summary>
        /// method to retrieve notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        [Route("api/CareTaker/GetNotificationDetailsById/{bookingId}")]
        public async Task<HttpResponseMessage> GetNotificationDetailsById(int bookingId)
        {
            try
            {
                var notification = await _pcmsService.GetNotificationDetailsById(bookingId);
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

        /// <summary>
        /// method to retrieve upcoming notifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/CareTaker/GetUpcomingNotifications/{caretakerId}")]
        public async Task<HttpResponseMessage> GetUpcomingNotifications(int caretakerId)
        {
            try
            {
                var notification = await _pcmsService.GetUpcomingNotifications(caretakerId);
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
        [Route("api/CareTaker/GetCaretakerScheduleList")]
        public async Task<HttpResponseMessage> GetCaretakerScheduleList(CaretakerScheduleListSearch caretakerScheduleListSearch)
        {
            try
            {
                var scheduleList = await _pcmsService.GetCaretakerScheduleList(caretakerScheduleListSearch);
                string result = JsonConvert.SerializeObject(scheduleList);
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
                _logger.Error(ex, "Failed to get schedule list Details");

                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Unable to save data. Please try again after sometime.", System.Text.Encoding.UTF8, "application/json");
                return response;
            }
        }

        [HttpPost]
        [Route("api/CareTaker/ConfirmAppointments")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> ConfirmAppointments(UpcomingAppointment upcomingAppointment)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to change status of User.");
                }
                int appointment = await _pcmsService.ConfirmAppointments(upcomingAppointment);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change status of User. UserId: {0}", upcomingAppointment.AppointmentId);
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

  
    }
}