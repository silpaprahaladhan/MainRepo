using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Nirast.Pcms.Api.Helpers
{
	public class ApiResponse
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static HttpResponseMessage CreateErrorResponse(HttpStatusCode statusCode, string message)
		{
			return new HttpResponseMessage(statusCode)
			{
				ReasonPhrase = message
			};
		}
	}
}