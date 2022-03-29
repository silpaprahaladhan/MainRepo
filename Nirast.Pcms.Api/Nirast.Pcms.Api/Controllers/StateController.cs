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
    public class StateController : ApiController
    {
        /// <summary>
        /// To retrieve state details
        /// </summary>
        /// <returns></returns>
        // GET: api/State
        public HttpResponseMessage Get()
        {
            StateModel stateModel = new StateModel();
            List<StateModel> lstStateModel = new List<StateModel>();
            try
            {
                lstStateModel.Add(stateModel);
                string result = JsonConvert.SerializeObject(lstStateModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No state found");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required state details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/State/5
        public HttpResponseMessage Get(int id)
        {
            StateModel stateModel = new StateModel();
            try
            {
                string result = JsonConvert.SerializeObject(stateModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No state found");
            }
            catch (Exception ex)
            {
                //bloggerLogs.LogError(ex, "Failed to get recent topic");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add states
        /// </summary>
        /// <param name="stateModel"></param>
        /// <returns></returns>
        // POST: api/State
        public HttpResponseMessage Post(StateModel stateModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "State creation failed.");
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
        /// To edit states
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/State/5
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
        /// To delete state
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/State/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "State Deletion failed.");
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
