using Newtonsoft.Json;
using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nirast.Pcms.Web.Logger;

namespace Nirast.Pcms.Web.Helpers
{
    public class Service
    {
        string baseUrl = ConfigurationManager.AppSettings["PCMSAPIUrl"].ToString();
        PCMSLogger _pCMSLogger;

        public Service()
        {
            
        }

        public Service(PCMSLogger pCMSLogger)
        {
            _pCMSLogger = pCMSLogger;
        }

        public HttpStatusCode PostAPI(string requestContent, string api)
        {

            using (var client = new HttpClient())
            {
                if (HttpContext.Current.Session["loginName"] != null && HttpContext.Current.Session["loginPassword"] != null)
                {
                    string authInfo = HttpContext.Current.Session["loginName"].ToString() + ":" +
                       HttpContext.Current.Session["loginPassword"].ToString();
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                }

                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
                var content = (string.IsNullOrWhiteSpace(requestContent)) ? null :
                    new StringContent(requestContent, Encoding.UTF8, "application/json");
                var result = client.PostAsync(baseUrl + api, content).Result;
                return result.StatusCode;
            }
        }
        public Task<string> PostAPIWithData(string requestContent, string api)
        {
            using (var httpClient = new HttpClient())
            {
                if (HttpContext.Current.Session["loginName"] != null && HttpContext.Current.Session["loginPassword"] != null)
                {
                    string authInfo = HttpContext.Current.Session["loginName"].ToString() + ":" +
                       HttpContext.Current.Session["loginPassword"].ToString();
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                }
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
                var content = (string.IsNullOrWhiteSpace(requestContent)) ? null :
                new StringContent(requestContent, Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync(baseUrl + api, content).Result;
                return result.Content.ReadAsStringAsync();
            }
        }
        public string GetAPI(string api)
        {
            string queryString = baseUrl + api;
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(queryString);
            string data = string.Empty;
            webrequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            if (HttpContext.Current.Session["loginName"] != null && HttpContext.Current.Session["loginPassword"] != null)
            {
                string authInfo = HttpContext.Current.Session["loginName"].ToString() + ":" +
                   HttpContext.Current.Session["loginPassword"].ToString();
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                // webrequest.Headers.Add("Authorization", "Basic " + authInfo);
                webrequest.Headers["Authorization"] = "Basic " + authInfo;
            }
            using (HttpWebResponse res = webrequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(res.GetResponseStream());
                data = reader.ReadToEnd();
            }
            return data;

        }
    }
}