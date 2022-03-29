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

namespace Nirast.Pcms.Api.Controllers
{
    public class ClientController : ApiController
    {
        private IClientService _clientService;
        private IPCMSService _pcmsService;
        private IPCMSLogger _logger;

        public ClientController(IClientService clientService, IPCMSLogger logger, IPCMSService pcmsService)
        {
            _clientService = clientService;
            _pcmsService = pcmsService;
            _logger = logger;
        }

        [HttpPost]
        [Route("api/Client/SaveClientDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveClientDetails(ClientDetails clientRegistration)
        {
            try
            {
                ClientDetails client = await _pcmsService.AddClientDetails(clientRegistration);
                string result = JsonConvert.SerializeObject(client);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (client != null && client.ClientId > 0)
                {
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client registration failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to register Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Client/SaveFranchiseDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveFranchiseDetails(ClientDetails clientRegistration)
        {
            try
            {
                ClientDetails client = await _pcmsService.AddFranchiseDetails(clientRegistration);
                string result = JsonConvert.SerializeObject(client);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (client != null && client.ClientId > 0)
                {
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client registration failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to register Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }



        [HttpPost]
        [Route("api/Client/SaveClientInvoiceDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveClientInvoiceDetails(InvoiceSearchInpts invoiceDetails)
        {
            try
            {
                int userDetailId = await _pcmsService.AddClientInvoiceDetails(invoiceDetails);

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


        [HttpPost]
        [Route("api/Client/GetInvoiceHistoryList")]
        public async Task<HttpResponseMessage> GetInvoiceHistoryList(InvoiceHistory invoiceHistory)
        {
            try
            {
                var lstInvoiceHistory = await _pcmsService.GetInvoiceHistoryList(invoiceHistory);
                string result = JsonConvert.SerializeObject(lstInvoiceHistory);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK, lstInvoiceHistory);
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
        [Route("api/Client/GetInvoiceHistoryById/{Id}")]
        public async Task<HttpResponseMessage> GetInvoiceHistoryById(int Id)
        {
            try
            {
                var invoiceDetails = await _pcmsService.GetInvoiceHistoryById(Id);
                string result = JsonConvert.SerializeObject(invoiceDetails);
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
                _logger.Error(ex, "Failed to get invoice details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Client/UpdateClientInvoice")]
        public async Task<HttpResponseMessage> UpdateClientInvoice(InvoiceSearchInpts searchInpts)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _clientService.UpdateClientInvoice(searchInpts);

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


        [HttpPost]
        [Route("api/Client/SaveClientCareTakerMapping")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveClientCareTakerMapping(WorkShiftPayRates shiftPayRates)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _clientService.SaveClientCareTakerMapping(shiftPayRates);

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

        [HttpPost]
        [Route("api/Client/SaveClientCareTakerPayRise")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveClientCareTakerPayRise(List<WorkShiftRates> workShiftRates)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _clientService.SaveClientCareTakerPayRise(workShiftRates);

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

        [HttpPost]
        [Route("api/Client/SaveClientcategoryCareTakerPayRise")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveClientcategoryCareTakerPayRise(List<ClientCategoryRate> categoryRates)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _clientService.SaveClientcategoryCareTakerPayRise(categoryRates);

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
        [HttpPost]
        [Route("api/Client/DeleteClientCareTakerMapping")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> DeleteClientCareTakerMapping(ClientCaretakers clientCareTaker)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _clientService.DeleteClientCareTakerMapping(clientCareTaker);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client unmapping failed.");
                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to unmap Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Client/SaveScheduleRejectedCareTaker")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveScheduleRejectedCareTaker(RejectedCaretaker careTaker)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _clientService.SaveScheduleRejectedCareTaker(careTaker);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Client unmapping failed.");
                }
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to unmap Client.");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Client/SaveScheduleDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SaveScheduleDetails(ScheduledData scheduledData)
        {
            try
            {
                string result = "success";

                int UserDetailId = await _pcmsService.AddScheduledDetails(scheduledData);
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
        [Route("api/Client/GetAllScheduledetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllScheduledetails(CalenderEventInput calenderEventInput)
        {
            try
            {
                var scheduledDetails = await _clientService.GetAllScheduledetails(calenderEventInput);
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
        [Route("api/Client/GetLoginLogDetailsByTypeId/{typeId}")]
        public async Task<HttpResponseMessage> GetLoginLogDetailsByTypeId(int typeId)
        {
            try
            {
                var clientDetails = await _clientService.GetLoginLogDetailsByTypeId(typeId);
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
                _logger.Error(ex, "Failed to get LoginLogDetails");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
        // GET: api/Client
        /// <summary>
        /// method to retrieve all client details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Client/GetAllClientDetails/")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllClientDetails()
        {
            List<ClientDetails> officeStaffRegistration = new List<ClientDetails>();
            try
            {
                var clientDetails = await _clientService.GetAllClientDetails();
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
        [Route("api/Client/GetAllClientDetailsByLocation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllClientDetailsByLocation(LocationSearchInputs inputs)
        {
            List<ClientDetails> officeStaffRegistration = new List<ClientDetails>();
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
            try
            {
                var clientDetails = await _clientService.GetAllClientDetailsByLocation(inputs);
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

        //franchise details 03-03-2022

        [HttpPost]
        [Route("api/Client/GetAllFranchiseDetailsByLocation")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllFranchiseDetailsByLocation(LocationSearchInputs inputs)
        {
            List<ClientDetails> officeStaffRegistration = new List<ClientDetails>();
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
            try
            {
                var clientDetails = await _clientService.GetAllFranchiseDetailsByLocation(inputs);
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






        /// <summary>
        /// method to retrieve all client details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Client/GetAllScheduleLogDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllScheduleLogDetails()
        {
            List<ScheduledData> schedulaData = new List<ScheduledData>();
            try
            {
                var scheduleLogData = await _clientService.GetAllScheduleLogDetails();
                string result = JsonConvert.SerializeObject(scheduleLogData);
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
        [Route("api/Client/GetAllScheduleRejectedCaretaker")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllScheduleRejectedCaretaker(BookingHistorySearch bookingHistorySearch)
        {
            List<RejectedCaretaker> officeStaffRegistration = new List<RejectedCaretaker>();
            try
            {
                var clientDetails = await _clientService.GetAllScheduleRejectedCaretaker(bookingHistorySearch);
                string result = JsonConvert.SerializeObject(clientDetails);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No Schedule Rejected Caretakers");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get Schedule Rejected Caregiver");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        // GET: api/Client
        /// <summary>
        /// method to retrieve  client details by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Client/GetAllClientDetailsById")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllClientDetailsById(int clientId)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _clientService.GetClientDetailsByID(clientId);
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



        // GET: api/Client
        /// <summary>
        /// method to retrieve  client details by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Client/GetAllFranchiseDetailsById")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetAllFranchiseDetailsById(int clientId)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _clientService.GetFranchiseDetailsByID(clientId);
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





        [Route("api/Client/GetMappedCaretakerRates/{clientId}/{caretakerId}")]
        public async Task<HttpResponseMessage> GetMappedCaretakerRates(int clientId, int caretakerId)
        {
            try
            {
                var mappedCaretakerRates = await _clientService.GetMappedCaretakerRates(clientId, caretakerId);
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

        [Route("api/Client/GetMappedCaretakersLatestPayRiseRates/{clientId}/{caretakerId}")]
        public async Task<HttpResponseMessage> GetMappedCaretakersLatestPayRiseRates(int clientId, int caretakerId)
        {
            try
            {
                var mappedCaretakerRates = await _clientService.GetMappedCaretakersLatestPayRiseRates(clientId, caretakerId);
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
        [Route("api/Client/GetMappedCaretakersPayRiseRatesByDate")]
        public async Task<HttpResponseMessage> GetMappedCaretakersPayRiseRatesByDate(PayriseData payriseData)
        {
            try
            {
                var mappedCaretakerRates = await _clientService.GetMappedCaretakersPayRiseRatesByDate(payriseData);
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

        [Route("api/Client/GetCategoryClientPayRiseRates/{clientId}")]
        public async Task<HttpResponseMessage> GetCategoryClientPayRiseRates(int clientId)
        {
            try
            {
                var mappedCaretakerRates = await _clientService.GetCategoryClientPayRiseRates(clientId);
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
        [Route("api/Client/ModifyClientStatusById/{clientId}/{status}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> ModifyClientStatusById(int clientId, int status)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _clientService.ModifyClientStatusById(clientId, status);
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
                _logger.Error(ex, "Failed to get delete details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Client/ModifyFranchiseStatusById/{clientId}/{status}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> ModifyFranchiseStatusById(int clientId, int status)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _clientService.ModifyFranchiseStatusById(clientId, status);
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
                _logger.Error(ex, "Failed to get delete details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }


        [HttpPost]
        [Route("api/Client/ChangeClientEmailStatus/{clientId}/{emailstatus}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> ChangeEmailStatus(int clientId, int emailstatus)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _clientService.ChangeClientEmailStatus(clientId, emailstatus);
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
                _logger.Error(ex, "Failed to get delete details");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Client/UpdateClientInvoiceNumber/{clientId}/{InvoiceNumber}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> UpdateClientInvoiceNumber(int clientId, int InvoiceNumber)
        {
            ClientDetails client = new ClientDetails();
            try
            {
                var clientDetails = await _clientService.UpdateClientInvoiceNumber(clientId, InvoiceNumber);
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

        /// <summary>
        /// method to search client details
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Client/SearchClient")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> SearchClient(ClientSearchInputs inputs)
        {

            try
            {
                var clientDetails = await _clientService.SearchClient(inputs);
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
                _logger.Error(ex, "Failed to search clients");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpDelete]
        [Route("api/Client/DeleteCountry/{countryId}")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public HttpResponseMessage DeleteCountry(int countryId)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Country Deletion failed.");
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete country");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("api/Client/DeleteSchedule")]
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
                int id = await _clientService.DeleteSchedule(deleteData);
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
        // GET: api/Client
        /// <summary>
        /// method to retrieve all Client Invoice Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Client/GetClientInvoiceDetails")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetClientInvoiceDetails()
        {
            List<ClientDetails> clientInvoiceDetails = new List<ClientDetails>();
            try
            {
                var clientDetails = await _clientService.GetClientInvoiceDetails();
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

        /// <summary>
        /// method to save all Client Invoice Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Client/AddInvoiceDetails")]

        public async Task<HttpResponseMessage> AddInvoiceDetails(ClientDetails clientInvoiceDetails)
        {

            try
            {
                var clientDetails = await _clientService.AddInvoiceDetails(clientInvoiceDetails);
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

        [HttpGet]
        [Route("api/Client/GetClientFromUserId/{id}")]
        public async Task<HttpResponseMessage> GetClientFromUserId(int id)
        {
            try
            {
                int clientId = await _clientService.GetClientFromUserId(id);
                if (clientId != 0)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(clientId.ToString(), System.Text.Encoding.UTF8, "application/json");
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
        [Route("api/Client/GetSchedulingLogDetailsById/{logId}")]
        public async Task<HttpResponseMessage> GetSchedulingLogDetailsById(int logId)
        {
            // ScheduledData s = new ScheduledData();
            try
            {
                var clientDetails = await _clientService.GetSchedulingLogDetailsById(logId);
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
        [Route("api/Client/GetClientInvoicePayRiseRatesonDateChange")]
        public async Task<HttpResponseMessage> GetClientInvoicePayRiseRatesonDateChange(PayriseData invoicePayRiseData)
        {
            try
            {
               // var mappedCaretakerRates = 
                var invoicePayRiseRates = await _clientService.GetClientInvoicePayRiseRatesonDateChange(invoicePayRiseData.ClientId,invoicePayRiseData.Date);
                string result = JsonConvert.SerializeObject(invoicePayRiseRates);
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
        [HttpGet]
        [Route("api/Client/GetUserTypeId")]
        //[BasicAuthentication("Administrator", "Office Staff")]
        public async Task<HttpResponseMessage> GetUserTypeId()
        {
            List<UsersDetails> UserData = new List<UsersDetails>();
            try
            {
                var scheduleLogData = await _clientService.GetUserTypeId();
                string result = JsonConvert.SerializeObject(scheduleLogData);
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


    }
}
