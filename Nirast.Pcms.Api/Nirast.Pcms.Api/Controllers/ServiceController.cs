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
    public class ServiceController : ApiController
    {
        /// <summary>
        /// To retrieve services
        /// </summary>
        /// <returns></returns>
        // GET: api/Service
        public HttpResponseMessage Get()
        {
            ServiceModel serviceModel = new ServiceModel();
            List<ServiceModel> lstServiceModel = new List<ServiceModel>();
            try
            {
                lstServiceModel.Add(serviceModel);
                string result = JsonConvert.SerializeObject(lstServiceModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No service found");
            }
            catch (Exception ex)
            {
                //bloggerLogs.LogError(ex, "Failed to get recent topic");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required service details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Service/5
        public HttpResponseMessage Get(int id)
        {
            ServiceModel serviceModel = new ServiceModel();
            try
            {
                string result = JsonConvert.SerializeObject(serviceModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No service found");
            }
            catch (Exception ex)
            {
                //bloggerLogs.LogError(ex, "Failed to get recent topic");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add services
        /// </summary>
        /// <param name="serviceModel"></param>
        /// <returns></returns>
        // POST: api/Service
        public HttpResponseMessage Post(ServiceModel serviceModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Service creation failed.");
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
        /// To edit services
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/Service/5
        public HttpResponseMessage Put(int id)
        {
            ServiceModel serviceModel = new ServiceModel();
            try
            {
                string result = "Successfully Updated";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Service updation failed.");
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
        /// To delete service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Service/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Service Deletion failed.");
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
