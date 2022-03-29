using Newtonsoft.Json;
using Nirast.Pcms.Web.Helpers;
using Nirast.Pcms.Web.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Configuration
{
    public class PaypalConfiguration
    {
       
        //Constructor  
        public PaypalConfiguration()
        {
            var config = GetConfig();
        }
        // getting properties from the web.config  
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private string GetAccessToken(string clientId, string secretKey)
        {
            try
            {
                // getting accesstocken from paypal  
                string accessToken = new OAuthTokenCredential(clientId, secretKey, GetConfig()).GetAccessToken();
                return accessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }
        public APIContext GetAPIContext(string clientId,string secretKey)
        {
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken(clientId,secretKey));
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}