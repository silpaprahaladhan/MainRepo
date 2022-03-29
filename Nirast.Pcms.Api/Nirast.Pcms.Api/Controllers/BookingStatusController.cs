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
    public class BookingStatusController : ApiController
    {
        /// <summary>
        /// To save booking status
        /// </summary>
        /// <param name="bookingStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveBookingStatus(BookingStatusModel bookingStatusModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Booking status change failed.");
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
