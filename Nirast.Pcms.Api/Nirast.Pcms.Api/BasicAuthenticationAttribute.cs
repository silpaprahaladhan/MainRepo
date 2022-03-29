using Newtonsoft.Json;
using Nirast.Pcms.Ap.Application.Infrastructure;
using Nirast.Pcms.Api.Data.Infrastructure;
using Nirast.Pcms.Api.Data.Repositories;
using Nirast.Pcms.Api.Helpers;
using Nirast.Pcms.Api.Logger;
using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using Nirast.Pcms.Api.Sdk.Repositories;
using Nirast.Pcms.Api.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Nirast.Pcms.Api
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private static ILoggedInUserRepository login;
        IConnectionFactory connectionFactory;
        IPCMSLogger logger;
        string[] roles;

        public BasicAuthenticationAttribute(params string[] roles)
        {
            connectionFactory = new ConnectionFactory();
            logger = new PCMSLogger();
            login = new LoggedInUserRepository(connectionFactory, logger);
            this.roles = roles;
        }

        public bool checkLogin(string username, string password)
        {
            string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();
            UserCredential newUserCredential = new UserCredential()
            {
                LoginName = username,
                Password = password
            };
            Security security = new Security();
            var loggedInUserDetails = login.CheckLoginCredentialsCheckLoginCredentials(newUserCredential);
            string checkResult = JsonConvert.SerializeObject(loggedInUserDetails.Result);
            var resultObj = JsonConvert.DeserializeObject<LoggedInUser>(checkResult);
            string decryptedPassword = security.Decrypt(resultObj.Password, encryptionPassword);
            return (password.Equals(decryptedPassword, StringComparison.OrdinalIgnoreCase) && roles.Any(x => x.Equals(resultObj.UserType, StringComparison.OrdinalIgnoreCase)));
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Authorization == null)
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    Security security = new Security();
                    string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                    string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                    string[] usernamePassword = decodedAuthenticationToken.Split(':');
                    string username = usernamePassword[0];
                    string password = security.Decrypt(usernamePassword[1], ConfigurationManager.AppSettings["EncryptPassword"].ToString());
                    string[] userRoles = new string[] { "Administrator", "Caretaker", "Public User", "Office Staff" };
                    if (checkLogin(username, password))
                    {
                        Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Authorization Failed");
                actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);

            }
        }
    }
}