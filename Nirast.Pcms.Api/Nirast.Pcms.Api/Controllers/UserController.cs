using NIRAST.PCMS.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NIRAST.PCMS.API.Controllers
{
    public class UserController : ApiController
    {
        #region Private Members
        IPCMSService _blogService;
        #endregion

        public UserController(IPCMSService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IHttpActionResult> GetAllUsers(string role)
        {
            var resultData = await _blogService.GetAllUsers(role);
            if (resultData == null)
            {
                return NotFound();
            }
            return Ok(resultData);
        }

        [Route("upload")]
        [HttpGet]
        public string Get()
        {
            return "Test Message";
        }
    }
}
