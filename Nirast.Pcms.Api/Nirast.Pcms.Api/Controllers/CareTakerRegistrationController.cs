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
    public class CareTakerRegistrationController : ApiController
    {
        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="careTakerRegistration"></param>
        /// <returns></returns>
        // POST: api/CareTakerRegistration
        public HttpResponseMessage Post(CareTakerRegistrationModel careTakerRegistration)
        {
            try
            {
                string result = "Care taker registration success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Failed to save care taker registration");
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

        #endregion
    }
}
