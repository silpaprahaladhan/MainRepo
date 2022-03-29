using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Web.Models
{
    public class PatientDetails
    {
        #region Public properties

        /// <summary>
        /// Get or Set Patient detail id
        /// </summary>
        public int PatientId { get; set; }

        public string SiteURL { get; set; }

        /// <summary>
        /// Get or Set user detail id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set first name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Get or Set last name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z](.*[a-za-zA-Z])?$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string LastName { get; set; }
        /// <summary>
        /// Get or Set country id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country 
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Get or Set state id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Get or Set city id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public int CityId { get; set; }

        /// <summary>
        /// Get or Set city 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Get or Set date of address
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters exceeded")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9].*[a-zA-Z0-9 ]+$", ErrorMessage = "Contains invalid characters.")]
        public string Address { get; set; }

        /// <summary>
        /// Get or Set date of location
        /// </summary>
        /// 
        [StringLength(50, ErrorMessage = "Maximum {1} characters exceeded")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Location { get; set; }

        /// <summary>
        /// Get or Set zip code
        /// </summary>
         [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        //[RegularExpression(@"^(?!.*[DFIOQU])[A-VXY][0-9][A-Z] ?[0-9][A-Z][0-9]$", ErrorMessage = "ZipCode format is not valid")]
        [StringLength(10, ErrorMessage = "Maximum {1} characters exceeded")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters exceeded")]
        public string PrimaryPhoneNo { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters exceeded")]
        public string SecondaryPhoneNo { get; set; }

        /// <summary>
        /// Get or Set remarks
        /// </summary>

     
        public string Remarks { get; set; }
        /// <summary>
        /// Get or Set created User Id
        /// </summary>
        public int CreatedUpdatedUserId { get; set; }
        private string service = "What are the service details required?";
        public string ServiceDetailsQuestion
        {
            get { return service; }
        }
        private string Disease = "Explain the disease details of patient";
        public string DiseasesQuestion
        {
            get { return Disease; }
        }

        private string condition = "What is the patient condition?";
        public string PatientconditionQn
        {
            get { return condition; }
        }

        private string History = "Any medication history?";
        public string MedicationHistory
        {
            get { return History; }
        }

        private string Allergy = "Explain the allergy details";
        public string AllergyQn
        {
            get { return Allergy; }
        }

        private string ExtraServices = "Extra services required?";
        public string ExtraServicesQn
        {
            get { return ExtraServices; }
        }

        private string RemarkAny = "Any remarks?";
        public string RemarksAnyQn
        {
            get { return RemarkAny; }
        }

        #endregion
    }
    public class CareRecipientQuestionare
    {
        public int QuestionnaireId { get; set; }
        public string BookingId { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }
        public string Answer6 { get; set; }
    }
    public class CaretakerBooking : PatientDetails
    {
        public int BookingId { get; set; }
        public int CareTakerId { get; set; }

        public string CareTakerName { get; set; }
        public int ServiceRequiredId { get; set; }
        public int BookedUserId { get; set; }
        public DateTime BookingDate { get; set; }
        [Required(ErrorMessage = "* Required")]
        public DateTime BookingStartTime { get; set; }
        [Required(ErrorMessage = "* Required")]
        public DateTime BookingEndTime { get; set; }
        [Required(ErrorMessage = "* Required")]
        public string Purpose { get; set; }
        public string Service { get; set; }
        public int Status { get; set; }
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "From Time")]
        public DateTime? FromTime { get; set; }
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "To Time")]
        public DateTime? ToTime { get; set; }
        public bool IsFullDay { get; set; }
        public CareRecipientQuestionare Questionaire { get; set; }

        public List<BookigDate> PublicUserBookigDates { get; set; }
    }

    public class BookigDate
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
    }
}
