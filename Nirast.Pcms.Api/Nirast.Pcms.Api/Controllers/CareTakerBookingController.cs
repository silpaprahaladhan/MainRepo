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
    public class CareTakerBookingController : ApiController
    {
        /// <summary>
        /// Method to save patient details
        /// </summary>
        /// <param name="careTakerBookingModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SavePatientDetails(CareTakerBookingModel careTakerBookingModel, QuestionsModel questionModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save patient details");
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
