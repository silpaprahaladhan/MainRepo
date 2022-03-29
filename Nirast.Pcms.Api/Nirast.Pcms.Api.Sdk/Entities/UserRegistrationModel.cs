using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
	//public class UserRegistrationModel
	//{
	//	/// <summary>
	//	/// Get or Set registration id
	//	/// </summary>
	//	public int UserRegistrationId { get; set; }

	//	/// <summary>
	//	/// Get or Set first name
	//	/// </summary>
	//	public string UserRegistrationFirstName { get; set; }

	//	/// <summary>
	//	/// Get or Set last name
	//	/// </summary>
	//	public string UserRegistrationLastName { get; set; }

	//	/// <summary>
	//	/// Get or Set gender id
	//	/// </summary>
	//	public int UserRegistrationGenderId { get; set; }

	//	/// <summary>
	//	/// Get or Set city id
	//	/// </summary>
	//	public int UserRegistrationCityId { get; set; }

	//	/// <summary>
	//	/// Get or Set state id
	//	/// </summary>
	//	public int UserRegistrationStateId { get; set; }

	//	/// <summary>
	//	/// Get or Set zip code
	//	/// </summary>
	//	public string UserRegistrationZipCode { get; set; }

	//	/// <summary>
	//	/// Get or Set phone number
	//	/// </summary>
	//	public string UserRegistrationPhoneNumber { get; set; }

	//	/// <summary>
	//	/// Get or Set email id
	//	/// </summary>
	//	public string UserRegistrationEmailId { get; set; }

	//	/// <summary>
	//	/// Get or Set password
	//	/// </summary>
	//	public string UserRegistrationPassword { get; set; }

	//	/// <summary>
	//	/// Get or Set card id
	//	/// </summary>
	//	public int UserRegistrationCardId { get; set; }

	//	/// <summary>
	//	/// Get or Set card name
	//	/// </summary>
	//	public string UserRegistrationCardName { get; set; }

	//	/// <summary>
	//	/// Get or Set card number
	//	/// </summary>
	//	public int UserRegistrationCardNumber { get; set; }

	//	/// <summary>
	//	/// Get or Set expiry month
	//	/// </summary>
	//	public int UserRegistrationExpiryMonth { get; set; }

	//	/// <summary>
	//	/// Get or Set expiry year
	//	/// </summary>
	//	public int UserRegistrationExpiryYear { get; set; }
	//}

    public class VerifyEmail
    {
        public string Email { get; set; }
        public int UserId { get; set; }

        public string VerificationLink { get; set; }

        public string WelcomeMsg { get; set; }
        public string FirstName { get; set; }
        public string MailMsg { get; set; }
        public string Mailcontent { get; set; }
        public string ContactNo { get; set; }
        public string RegardsBy { get; set; }
        public string siteUrl { get; set; }
        public string CompanyName_TagLine { get; set; }
        public string CompanyName { get; set; }
        public string Subject { get; set; }
    }

    public class VerifyUserAccount
    {
        public int UserId { get; set; }
        public bool Verified { get; set; }
    }
}