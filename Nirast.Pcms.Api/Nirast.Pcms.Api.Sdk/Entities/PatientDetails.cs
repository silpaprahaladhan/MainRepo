using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    /// <summary>
    /// CaretakerBookingModel
    /// </summary>
    public class CaretakerBookingModel: CareRecipientModel
    {
        public int BookingId { get; set; }
        public int CareTakerId { get; set; }
        public int ServiceRequiredId { get; set; }
        public int BookedUserId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime BookingStartTime { get; set; }
        public DateTime BookingEndTime { get; set; }
        public int Status { get; set; }
        public bool IsFullDay { get; set; }
        public CareRecipientQuestionare Questionaire { get; set; }
        public List<BookigDate> PublicUserBookigDates { get; set; }
    }

    public class BookigDate
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
    }

    /// <summary>
    /// CareRecipientModel
    /// </summary>
    public class CareRecipientModel
    {
        #region Public properties

        /// <summary>
        /// Get or Set Patient detail id
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// Get or Set user detail id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get or Set last name
        /// </summary>
        public string LastName { get; set; }
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
        public int? CityId { get; set; }

        /// <summary>
        /// Get or Set city 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Get or Set date of address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Get or Set date of location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set zip code
        /// </summary>
        public string ZipCode { get; set; }

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
        /// Get or Set remarks
        /// </summary>
        public string Purpose  { get; set; }

        public string SiteURL { get; set; }

        #endregion
    }

    /// <summary>
    /// CareRecipientQuestionare
    /// </summary>
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
}
