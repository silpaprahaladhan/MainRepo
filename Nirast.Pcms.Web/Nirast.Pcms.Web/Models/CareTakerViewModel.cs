using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Web.Models.Enums;

namespace Nirast.Pcms.Web.Models
{
    [Serializable]
    public class CareTakerRegistrationViewModel
    {

        public CareTakerRegistrationViewModel()
        {
            CertificateFiles = new List<HttpPostedFileBase>();
            SINFile = new List<HttpPostedFileBase>();
            OtherDocuments = new List<HttpPostedFileBase>();
        }

        /// <summary>
        /// Gets or sets the siteURL.
        /// </summary>
        public string SiteURL { get; set; }

        /// <summary>
        /// Gets or sets the caretaker detail identifier.
        /// </summary>
        /// <value>
        /// The caretaker detail identifier.
        /// </value>
        public int CaretakerDetailId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user type identifier.
        /// </summary>
        /// <value>
        /// The user type identifier.
        /// </value>
        public int UserTypeId { get; set; }

        /// <summary>
        /// Gets or sets the caretaker profile identifier.
        /// </summary>
        /// <value>
        /// The caretaker profile identifier.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CaretakerProfileId")]
        public string CaretakerProfileId { get; set; }

        /// <summary>
        /// Gets or sets the total experience.
        /// </summary>
        /// <value>
        /// The total experience.
        /// </value>
        [RegularExpression(@"^\d{0,2}(\.\d{1,2})?$", ErrorMessage = "Invalid Experience")]
        public float TotalExperience { get; set; }

        /// <summary>
        /// Gets or sets the dob.
        /// </summary>
        /// <value>
        /// The dob.
        /// </value>
        public DateTime Dob { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the gender identifier.
        /// </summary>
        /// <value>
        /// The gender identifier.
        /// </value>
        public int GenderId { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
          [Required(ErrorMessage = "* Required")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>
        /// The country identifier.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Country Id")]
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the state identifier.
        /// </summary>
        /// <value>
        /// The state identifier.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CareTakerStateId")]
        public int StateId { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        /// 
     
        public string City{ get; set; }

        /// <summary>
        /// Gets or sets the city identifier.
        /// </summary>
        /// <value>
        /// The city identifier.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CareTakerCityId")]
        public int CityId { get; set; }

        /// <summary>
        /// Gets or sets the primary phone no.
        /// </summary>
        /// <value>
        /// The primary phone no.
        /// </value>
        [DataType(DataType.PhoneNumber)]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
       
        public string PrimaryPhoneNo { get; set; }


        /// <summary>
        /// Gets or sets the secondary phone no.
        /// </summary>
        /// <value>
        /// The secondary phone no.
        /// </value>
        [DataType(DataType.PhoneNumber)]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
      
        public string SecondaryPhoneNo { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [RegularExpression("^[a-zA-Z0-9][a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Maximum {1} characters allowed")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the about me.
        /// </summary>
        /// <value>
        /// The about me.
        /// </value>
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
       
        public string AboutMe { get; set; }

        /// <summary>
        /// Gets or sets the account status.
        /// </summary>
        /// <value>
        /// The account status.
        /// </value>
        public CaretakerStatus AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the key skills.
        /// </summary>
        /// <value>
        /// The key skills.
        /// </value>
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        public string KeySkills { get; set; }

        public List<CaretakerExperiences> CareTakerExperiences { get; set; }
        public List<CareTakerQualifications> CareTakerQualifications = new List<CareTakerQualifications>();
        public List<CareTakerServices> CareTakerServices = new List<CareTakerServices>();
        public List<CareTakerClients> CareTakerClients = new List<CareTakerClients>();
        public List<DocumentsList> CareTakerDocuments = new List<DocumentsList>();

        public byte[] ConsentForm  { get; set; }
        public string ConcentFormData { get; set; }

        [DataType(DataType.Upload)]
        public List<HttpPostedFileBase> CertificateFiles { get; set; }
        public List<HttpPostedFileBase> SINFile { get; set; }
        public List<HttpPostedFileBase> OtherDocuments { get; set; }
        public HttpPostedFileBase ConsentDocument { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [send through fax].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send through fax]; otherwise, <c>false</c>.
        /// </value>
        public bool SendThroughFax { get; set; }

        /// <summary>
        /// Gets or sets the full name of the care taker.
        /// </summary>
        /// <value>
        /// The full name of the care taker.
        /// </value>
        public string CareTakerFullName { get { return FirstName + " " + LastName; } }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CareTakerFirstName")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
      
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CareTakerLastName")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
       
        //[RegularExpression(@"^[a-zA-Z](.*[a-za-zA-Z])?$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
     
        public string SSID { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
      
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the house.
        /// </summary>
        /// <value>
        /// The name of the house.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "HouseNumber")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
     
        public string HouseName { get; set; }

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        /// <value>
        /// The category identifier.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        /// <value>
        /// The name of the category.
        /// </value>
        /// 
        public string CategoryName { get; set; }        

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        /// <value>
        /// The zip code.
        /// </value>
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(10, ErrorMessage = "Maximum {1} characters allowed")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the profile picture.
        /// </summary>
        /// <value>
        /// The profile picture.
        /// </value>
        public byte[] ProfilePicByte { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Password")]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be minimum 8 characters including 1 uppercase , one special character and alphanumeric characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the retype password.
        /// </summary>
        /// <value>
        /// The retype password.
        /// </value>
        [NotMapped] // Does not effect with your database
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string RetypePassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user verified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user verified]; otherwise, <c>false</c>.
        /// </value>
        public bool UserVerified{ get; set; }

        /// <summary>
        /// Gets or sets the user status.
        /// </summary>
        /// <value>
        /// The user status.
        /// </value>
        public UserStatus UserStatus { get; set; }

        //For Advance Search
        public float DisplayRate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }

        public bool IsPrivate { get; set; }

        public string Currency { get; set; }

        public string CurrencySymbol { get; set; }
        public string ProfilePicPath { get; set; }
        public string ConsentDocPath { get; set; }

        public Nullable<int> CountryId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a State")]
        public Nullable<int> StateId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a City")]
        public Nullable<int> CityId1 { get; set; }
        public Nullable<int> branchId1 { get; set; }
        public string Branch { get; set; }
        public string EmployeeNumber { get; set; }

    }
    [Serializable]
    public class CaretakerExperiences
    {
        /// <summary>
        /// Get or Set Experience Date From
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Get or Set Experience Date To
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Get or Set Experience Company
        /// </summary>
        public string Company { get; set; }
    }
    [Serializable]
    public class CareTakerQualifications
    {
        /// <summary>
        /// Get or Set qualification id
        /// </summary>
        public int QualificationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CareTakerQualificationId { get; set; }

        /// <summary>
        /// Get or Set qualification
        /// </summary>
        public string QualificationName { get; set; }
    }
    [Serializable]
    public class CareTakerServices
    {
        /// <summary>
        /// Get or Set Service Id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set Service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Get or Set Expected Service Rate
        /// </summary>
        public float DisplayRate { get; set; }

        /// <summary>
        /// Get or Set Approved Service Rate
        /// </summary>
        public float PayingRate { get; set; }
        public int CareTakerId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
    }
    public class ServicePayRates
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public float PayRiseRate { get; set; }
        public float DisplayRiseRate { get; set; }
    }
    [Serializable]
    public class CareTakerClients
    {
        /// <summary>
        /// Get or Set Client id
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Get or Set Client id
        /// </summary>
        public string ClientFirstName { get; set; }

        /// <summary>
        /// Get or Set Client id
        /// </summary>
        public string ClientLastName { get; set; }

        public string fullname
        {
            get { return ClientFirstName + " " + ClientLastName; }
        }
    }

    public class GenderViewModel
    {
        public int GenderId { get; set; }
        public string Gender { get; set; }
    }

    public class DocumentsList
    {
        public int CaretakerDocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string ContentType { get; set; }
        public byte[] DocumentContent { get; set; }

        public string DocumentPath { get; set; }
    }

    public class ApproveCaretaker
    {
        public int CareTakerId { get; set; }
        public bool IsPrivate { get; set; }
        public List<CareTakerServices> ApprovedServiceRates { get; set; }
        public List<ServicePayRates> PayRiseServiceRates { get; set; }
        public string SiteURL { get; set; }
        public int ServiceId { get; set; }
    }

    public class RejectCareTaker
    {
        public int Userid { get; set; }

        public string SiteURL { get; set; }
    }

    public class CaretakerType
    {
        public string TypeName { get; set; }
        public string Color { get; set; }
    }
    [Serializable]
    public class SearchedCareTakers
    {
        public int CaretakerId { get; set; }

        public string CareTakerFirstName { get; set; }

        public string CareTakerLastName { get; set; }
        public string Fullname { get; set; }

        public int ServiceId { get; set; }
        
        public string ServiceName { get; set; }
        public string Location { get; set; }
        public string State { get; set; }

        public float TotalExperience { get; set; }

        public float DisplayRate { get; set; }

        public byte[] ProfilePicture { get; set; }

        public string Currency { get; set; }

        public string CurrencySymbol { get; set; }

        public string Category { get; set; }

        public string ProfilePicPath { get; set; }
    }
    public class CaretakerAvailableReport
    {
        public int ClientId { get; set; }
        public int CaretakerUserId { get; set; }
        public string ClientName { get; set; }
        public string CareTakerName { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime Dates { get; set; }
        public string TotalHours { get; set; }
        public string Timeshifts { get; set; }
        public string Time { get; set; }
        public string StartTime { get; set; }
        public string Description { get; set; }
        public string branch { get; set; }
        public string Commission { get; set; }
        public string Total { get; set; }
    }

}