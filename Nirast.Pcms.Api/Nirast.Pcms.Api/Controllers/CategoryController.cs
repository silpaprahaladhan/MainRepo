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
    public class CategoryController : ApiController
    {
        /// <summary>
        /// To retrieve category details
        /// </summary>
        /// <returns></returns>
        // GET: api/Category
        public HttpResponseMessage Get()
        {
            CategoryModel categoryModel = new CategoryModel();
            List<CategoryModel> lstCategoryModel = new List<CategoryModel>();
            try
            {
                lstCategoryModel.Add(categoryModel);
                string result = JsonConvert.SerializeObject(lstCategoryModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No category found");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// To retrieve required category details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Category/5
        public HttpResponseMessage Get(int id)
        {
            CategoryModel categoryModel = new CategoryModel();
            try
            {
                string result = JsonConvert.SerializeObject(categoryModel);
                if (result != null)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "No category found");
            }
            catch (Exception ex)
            {
                //bloggerLogs.LogError(ex, "Failed to get recent topic");
                return ApiResponse.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        /// <summary>
        ///To add category
        /// </summary>
        /// <param name="categoryModel"></param>
        /// <returns></returns>
        // POST: api/Category
        public HttpResponseMessage Post(CategoryModel categoryModel)
        {
            try
            {
                string result = "success";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Category creation failed.");
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
        /// To edit category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/Category/5
        public HttpResponseMessage Put(int id)
        {
            try
            {
                string result = "Successfully Updated";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Category updation failed.");
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
        /// To delete category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Category/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                string result = "Successfully Deleted";
                if (result == null)
                {
                    return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Category Deletion failed.");
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
