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
    public class SubCategoryController : ApiController
    {
        /// <summary>
        /// To retrieve subcategory details
        /// </summary>
        /// <returns></returns>
        // GET: api/SubCategory
        public HttpResponseMessage Get()
        {
            SubCategoryModel subCategoryModel = new SubCategoryModel();
            List<SubCategoryModel> lstSubCategoryModel = new List<SubCategoryModel>();
            try
            {
                lstSubCategoryModel.Add(subCategoryModel);
                string result = JsonConvert.SerializeObject(lstSubCategoryModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No subcategory found");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required subcategory details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/SubCategory/5
        public HttpResponseMessage Get(int id)
        {
            SubCategoryModel subCategoryModel = new SubCategoryModel();
            try
            {
                string result = JsonConvert.SerializeObject(subCategoryModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No subcategory found");
            }
            catch (Exception ex)
            {
                //bloggerLogs.LogError(ex, "Failed to get recent topic");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add subcategory
        /// </summary>
        /// <param name="subCategoryModel"></param>
        /// <returns></returns>
        // POST: api/SubCategory
        public HttpResponseMessage Post(SubCategoryModel subCategoryModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "SubCategory creation failed.");
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
        /// To edit subcategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/SubCategory/5
        public HttpResponseMessage Put(int id)
        {
            try
            {
                string result = "Successfully Updated";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "SubCategory updation failed.");
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
        /// To delete subcategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/SubCategory/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "SubCategory Deletion failed.");
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
