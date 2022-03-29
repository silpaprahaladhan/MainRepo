using Newtonsoft.Json;
using NIRAST.PCMS.API.Helpers;
using NIRAST.PCMS.API.Models;
using NIRAST.PCMS.Sdk.Entities;
using NIRAST.PCMS.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NIRAST.PCMS.API.Controllers
{
    public class CountryController : ApiController
    {
        private IPCMSService _pcmsService;
        public CountryController(IPCMSService pcmsService)
        {
            _pcmsService = pcmsService;
        }
        /// <summary>
        /// To retrieve country details
        /// </summary>
        /// <returns></returns>
        // GET: api/Country
        public HttpResponseMessage Get()

        {
            Country Country = new Country();
            List<Country> lstCountry = new List<Country>();
            try
            {
                lstCountry = _pcmsService.RetrieveCountry();
                string result = JsonConvert.SerializeObject(lstCountry);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No country found");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required country details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Country/5
        public HttpResponseMessage Get(int id)
        {
            Country Country = new Country();
            try
            {
                string result = JsonConvert.SerializeObject(Country);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No country found");
            }
            catch (Exception ex)
            {
                //bloggerLogs.LogError(ex, "Failed to get recent topic");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add country
        /// </summary>
        /// <param name="Country"></param>
        /// <returns></returns>
        // POST: api/Country
        [Route("Country/AddCountry")]
        public async Task<HttpResponseMessage> AddCountry(Country Country)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Country creation failed.");
                }
                int countryId = await _pcmsService.AddCountry(Country);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("Country/Test")]
        public string Test()
        {
            return "Test Message";
        }

        /// <summary>
        /// To edit country
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/Country/5
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
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To delete country
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Country/5
        public HttpResponseMessage Delete(int id)
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
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
    }
}
