using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CompanyProfile
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
        public string TagLine { get; set; }
        public string AddressLine { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public int StateId { get; set; }
        public string State { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string Fax { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        //public HttpPostedFileBase Logo { get; set; }
        public byte[] Logo { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }

    }
    public class UsersDetails
    {
        #region Public properties

        /// <summary>
        /// Get or Set uaser detail id
        /// </summary>
        public int UserRegnId { get; set; }

        /// <summary>
        /// Get or Set first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get or Set last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Get or Set photo
        /// </summary>
        public byte[] ProfilePicByte { get; set; }

        /// <summary>
        /// Get or Set gender id
        /// </summary>
        public int GenderId { get; set; }

        /// <summary>
        /// Get or Set gender 
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Get or Set country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country 
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Get or Set state id
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Get or Set city id
        /// </summary>
        public Nullable<int> CityId { get; set; }

        /// <summary>
        /// Get or Set city 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Get or Set date of birth
        /// </summary>
        public DateTime DOB { get; set; }

        /// <summary>
        /// Gets or sets the name of the house.
        /// </summary>
        /// <value>
        /// The name of the house.
        /// </value>
        public string HouseName { get; set; }

        /// <summary>
        /// Get or Set date of location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set zip code
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Get or Set user tyype id
        /// </summary>
        public int UserTypeId { get; set; }

        /// <summary>
        /// Get or Set user type name
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        public string PrimaryPhoneNo { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        public string SecondaryPhoneNo { get; set; }

        /// <summary>
        /// Get or Set email id
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Get or Set password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Set or Get User Verified status
        /// </summary>
        public bool UserVerified { get; set; }

        /// <summary>
        /// Gets or sets the user status.
        /// </summary>
        /// <value>
        /// The user status.
        /// </value>
        public int UserStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SiteURL { get; set; }

        /// <summary>
        /// Gets or sets the profile picture path
        /// </summary>
        public string ProfilePicPath { get; set; }
        public string InvoicePrefix { get; set; }

        public int InvoiceNumber { get; set; }
        public Nullable<int> CountryId1 { get; set; }
        public Nullable<int> StateId1 { get; set; }
        public Nullable<int> CityId1 { get; set; }
        public Nullable<int> BranchId1 { get; set; }

        #endregion
    }

    public class OfficeStaffRegistration : UsersDetails
    {
        #region public properties

        /// <summary>
        /// Get or Set the office staff id
        /// </summary>
        public int OfficeStaffId { get; set; }

        /// <summary>
        /// Get or Set the user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set the designation id
        /// </summary>
        public int? DesignationId { get; set; }
        public int? QualificationId { get; set; }

        /// <summary>
        /// Get or Set the designation 
        /// </summary>
        public string Designation { get; set; }
        public string QualificationName { get; set; }
        /// <summary>
        /// Get or Set the Role Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Get or Set the Role
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Get or Set the TestRoleId
        /// </summary>
        public int WorkRoleTableId { get; set; }
        /// <summary>
        /// Get or Set the TestRole
        /// </summary>
        public TestRole Stafftype { get; set; }

        public string EmployeeNo { get; set; }
        #endregion
    }

    public class PublicUserRegistration : UsersDetails
    {
        public int CardId { get; set; }
        public string NameOnCard { get; set; }
        public string UserCard_CardNumber { get; set; } 
		public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }

        public int CardTypeId { get; set; }
        public string CardType { get; set; }
        public bool IsDefault { get; set; }
        public string EmployeeNumber { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
        public string SiteURL { get; set; }
    }

    public class ChangePassword
    {
        public string EmailId { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }

    public class PayPalAccount
    {
        public int PaypalId { get; set; }

        public string ClientId { get; set; }

        public string SecretKey { get; set; }
    }

    public class LoginLog
    {
        public string UserType { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public DateTime LoginDate { get; set; }
    }
}