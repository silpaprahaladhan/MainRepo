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
    public class CityController : ApiController
    {
        // GET: PublicUser
        private IPCMSService _pcmsService;
        private IPCMSLogger _logger;

        public CityController() { }
        public CityController(IPCMSService pcmsService, IPCMSLogger logger)
        {
            _pcmsService = pcmsService;
            _logger = logger;
        }

        /// <summary>
        /// To retrieve required city details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/City/5
        [Route("api/City/GetCity")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetCity(string flag,string value)
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
                _logger.Error(ex, "Failed to get city");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add city
        /// </summary>
        /// <param name="cityModel"></param>
        /// <returns></returns>
        // POST: api/City
        [HttpPost]
        [Route("api/City/SaveCity")]
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
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save city");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }



        /// <summary>
        /// To delete city
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/City/5
        [HttpPost]
        [Route("api/City/DeleteCity")]
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
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delate city");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To get cities by state id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/City/GetCityByStateId/{stateId}")]
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
                _logger.Error(ex, "Failed to get city by state id");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
    }
}
