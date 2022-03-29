using Newtonsoft.Json;
using NIRAST.PCMS.API.Helpers;
using NIRAST.PCMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NIRAST.PCMS.API.Controllers
{
    public class UsersController : ApiController
    {
        /// <summary>
        /// To display user booking history
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        public HttpResponseMessage Get()
        {
            UserBookingHistoryModel bookingHistoryModel = new UserBookingHistoryModel();
            List<UserBookingHistoryModel> lstUserBookingHistoryModel = new List<UserBookingHistoryModel>();
            try
            {
                lstUserBookingHistoryModel.Add(bookingHistoryModel);
                string result = JsonConvert.SerializeObject(lstUserBookingHistoryModel);
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
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To display user booking details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Users/GetBookingDetails/{id}")]
        public HttpResponseMessage GetBookingDetails(int id)
        {
            UserBookingDetails bookingDetails = new UserBookingDetails();
            try
            {
                string result = JsonConvert.SerializeObject(bookingDetails);
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
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To save user registration details
        /// </summary>
        /// <param name="userRegistration"></param>
        /// <returns></returns>
        [HttpPost]
        // POST: api/Users
        public HttpResponseMessage Post(UserRegistrationModel userRegistration)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "User registration creation failed.");
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
