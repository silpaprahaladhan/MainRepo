using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Web.Models.Enums;

namespace Nirast.Pcms.Web.Models
{
    public class CompanyProfile
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string CompanyName { get; set; }

        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string TagLine { get; set; }


        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "* Required")]
        //[MinLength(1, ErrorMessage ="* Required")]
        public int CountryId { get; set; }
        public string Country { get; set; }

        [Required(ErrorMessage = "* Required")]
        //[MinLength(1, ErrorMessage = "* Required")]
        public int StateId { get; set; }
        public string State { get; set; }

        [Required(ErrorMessage = "* Required")]
        //[MinLength(1, ErrorMessage = "* Required")]
        public int CityId { get; set; }
        public string City { get; set; }

        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string PhoneNo1 { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string PhoneNo2 { get; set; }

        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string EmailAddress { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Website { get; set; }

        //[Required(ErrorMessage = "* Required")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        public HttpPostedFileBase LogoImage { get; set; }
        public byte[] Logo { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Description1 { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Description2 { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Description3 { get; set; }

    }
    public class UsersDetails
    {
        #region Public properties

        /// <summary>
        /// Get or Set first name
        ///</summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string FirstName { get; set; }
        /// <summary>
        /// Get or Set uaser detail id
        /// </summary>
        public int UserRegnId { get; set; }
        public string BranchId1Name { get; set; }
        /// <summary>
        /// Get or Set last name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z](.*[a-za-zA-Z])?$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// Get or Set photo
        /// </summary>
        public byte[] ProfilePicByte { get; set; }

        /// <summary>
        /// Get or Set gender id
        /// </summary>
        public int GenderId { get; set; }

        /// <summary>
        /// Get or Set country id
        /// </summary>

        [Required(ErrorMessage = "* Required")]
        public int CountryId { get; set; }
        /// <summary>
        /// Get or Set gender 
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Get or Set country 
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public string Country { get; set; }

        /// <summary>
        /// Get or Set state id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public string State { get; set; }

        /// <summary>
        /// Get or Set city id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public Nullable<int> CityId { get; set; }

        /// <summary>
        /// Get or Set city 
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public string City { get; set; }


        public Nullable<int> BranchId1 { get; set; }

        /// <summary>
        /// Get or Set city 
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public string Branch { get; set; }

        /// <summary>
        /// Get or Set date of birth
        /// </summary>
        public DateTime DOB { get; set; }

        /// <summary>
        /// Get or Set zip code
        /// </summary>
        //[Required(ErrorMessage = "* Required")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(10, ErrorMessage = "Maximum {1} characters allowed")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Get or Set date of location
        /// </summary>
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the house.
        /// </summary>
        /// <value>
        /// The name of the house.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9].*[a-zA-Z0-9 ]+$", ErrorMessage = "Contains invalid characters.")]
        public string HouseName { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        //[Required(ErrorMessage = "* Required")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(500, ErrorMessage = "Maximum {1} characters exceeded")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string PrimaryPhoneNo { get; set; }
        /// <summary>
        /// Get or Set user tyype id
        /// </summary>
        public int UserTypeId { get; set; }

        /// <summary>
        /// Get or Set user type name
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Get or Set email id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [RegularExpression("^[a-zA-Z0-9][a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Maximum {1} characters allowed")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        [DataType(DataType.PhoneNumber)]

        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string SecondaryPhoneNo { get; set; }

        /// <summary>
        /// Get or Set password
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and maximum {1} characters long.", MinimumLength = 8)]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must be minimum 8 characters including 1 uppercase , one special character and alphanumeric characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be minimum 8 characters including 1 uppercase , one special character and alphanumeric characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Get or Set confirm password
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(25, ErrorMessage = "The confirm password must be at least {2} and maximum {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user verified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user verified]; otherwise, <c>false</c>.
        /// </value>
        public bool UserVerified { get; set; }

        /// <summary>
        /// Gets or sets the user status.
        /// </summary>
        /// <value>
        /// The user status.
        /// </value>
        public UserStatus UserStatus { get; set; }

        /// <summary>
        /// Gets or sets the siteURL.
        /// </summary>
        public string SiteURL { get; set; }

        /// <summary>
        /// Gets or sets the profile picture path
        /// </summary>
        public string ProfilePicPath { get; set; }
        [Required(ErrorMessage = "* Required")]
        public string InvoicePrefix { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int InvoiceNumber { get; set; }

        [Required(ErrorMessage = "* Required")]
        public Nullable<int> CountryId1 { get; set; }
        public Nullable<int> StateId1 { get; set; }
        public Nullable<int> CityId1 { get; set; }

        public string CountryId1Name { get; set; }
        public string StateId1Name { get; set; }
        public string CityId1Name { get; set; }

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
        /// Get or Set the designation 
        /// </summary>

        public string Designation { get; set; }
        public string QualificationName { get; set; }
        /// <summary>
        /// Get or Set the user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set the designation id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public int? DesignationId { get; set; }

        //[Required(ErrorMessage = "* Required")]
        public int? QualificationId { get; set; }

        /// <summary>
        /// Get or Set the designation id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public int RoleId { get; set; }

        /// <summary>
        /// Get or Set the designation 
        /// </summary>

        public string RoleName { get; set; }

        /// <summary>
        /// Get or Set the TestRole 
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public TestRole Stafftype { get; set; }

        /// <summary>
        /// Get or Set the TestRoleId
        /// </summary>
        public int WorkRoleTableId { get; set; }
        public string EmployeeNo { get; set; }

        #endregion
    }
    public class PublicUserRegistration : UsersDetails
    {
        //public int CardId { get; set; }

        ////[Required(ErrorMessage = "Please Name in card")]
        //public string NameOnCard { get; set; }

        //// [Required(ErrorMessage = "Please Name card number")]
        //[RegularExpression("^[0-9]{16}$", ErrorMessage = "Invalid card number")]

        //[DataType(DataType.CreditCard)]
        //public string UserCard_CardNumber { get; set; }
        ////[Required(ErrorMessage = "Please Enter expiry month")]
        //// [Display(Name = "ExpiryMonth")]
        //public int ExpiryMonth { get; set; }

        ////  [Required(ErrorMessage = "Please Enter Expiry year")]
        ////[Display(Name = "ExpiryYear")]
        //public int ExpiryYear { get; set; }

        ////[Required(ErrorMessage = "Please Enter Card Type")]
        //// [Display(Name = "CardTypeId")]
        //public int CardTypeId { get; set; }

        public string CardType { get; set; }
        public bool IsDefault { get; set; }
        public string EmployeeNumber { get; set; }


        
    }

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

    public class SendOTPOutput
    {
        public string Otp { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "* Required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "* Enter atleast 6 digits")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "OTP must be numeric")]
        public string InputOtp { get; set; }
        public OTPStatus OtpStatus { get; set; }
    }

    public class VerifyUserAccount
    {
        public int UserId { get; set; }
        public bool Verified { get; set; }
    }
    public class CardTypeViewModel
    {
        public int CardTypeId { get; set; }
        public string CardType { get; set; }
    }

    public class ChangeUserStatus
    {
        public int UserRegnId { get; set; }

        public int AccountStatus { get; set; }
    }
    public class LoginLogModel
    {
        public long LogId { get; set; }
        public int UserID { get; set; }
        public DateTime LoginDate { get; set; }
        public string LoginIP { get; set; }
    }
    public class LoginLogs
    {
        public string UserType { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public DateTime LoginDate { get; set; }
    }
}
