using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    [Table("tbl_CateTaker")]
    public class CareTakerBooking
    {
        /// <summary>
        /// Get or Set the patient id
        /// </summary>
        [Column("Table1Id")]
        public int PatientId { get; set; }

        /// <summary>
        /// Get or Set the patient name
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// Get or Set the patient address
        /// </summary>
        public int PatientAddress { get; set; }

        /// <summary>
        /// Get or Set the country Id
        /// </summary>
        public int PatientCountryId { get; set; }

        /// <summary>
        /// Get or Set the country name
        /// </summary>
        public int PatientCountryName { get; set; }

        /// <summary>
        /// Get or Set the state Id
        /// </summary>
        public int PatientStateId { get; set; }

        /// <summary>
        /// Get or Set the state name
        /// </summary>
        public string PatientStateName { get; set; }

        /// <summary>
        /// Get or Set the city id
        /// </summary>
        public int PatientCityId { get; set; }

        /// <summary>
        /// Get or Set the city name
        /// </summary>
        public string PatientCityName { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>
        public DateTime PatientFromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        public DateTime PatientToDate { get; set; }

        /// <summary>
        /// Get or Set from time
        /// </summary>
        public string PatientFromTime { get; set; }

        /// <summary>
        /// Get or Set from time
        /// </summary>
        public string PatientToTime { get; set; }

        /// <summary>
        /// Get or Set theb first phone number
        /// </summary>
        public string PhoneNumber1 { get; set; }

        /// <summary>
        /// Get or Set the second phone number
        /// </summary>
        public string PhoneNumber2 { get; set; }

        /// <summary>
        /// Get or Set purpose of booking
        /// </summary>
        public string PatientBookingPurpose { get; set; }

        /// <summary>
        /// Get or Set created user id
        /// </summary>
        public int PatientCreatedUserId { get; set; }

        /// <summary>
        /// Get or Set updated user id
        /// </summary>
        public int PatientUpdatedUserId { get; set; }

        /// <summary>
        /// Get or Set patient is active or not
        /// </summary>
        public int PatientRecordActive { get; set; }
    }
}