using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class UserBookingHistoryModel
    {
        /// <summary>
        /// Get or Set booking id
        /// </summary>
        public int UserBookingId { get; set; }

        /// <summary>
        /// Get or Set booking date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Get or Set caretaker name
        /// </summary>
        public string CareTakerName { get; set; }

        /// <summary>
        /// Get or Set profile id
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// Get or Set service
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Get or Set status
        /// </summary>
        public string Status { get; set; }
    }

    public class UserBookingDetails : UserBookingHistoryModel
    {
        /// <summary>
        /// Get or Set user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>
        public DateTime BookingDateFrom { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        public DateTime BookingDateTo { get; set; }

        /// <summary>
        /// Get or Set rate per hour
        /// </summary>
        public int BookingSalaryOfferedPerHour { get; set; }

        /// <summary>
        /// Get or Set time from
        /// </summary>
        public DateTime BookingPerDayTimeFrom { get; set; }

        /// <summary>
        /// Get or Set time to
        /// </summary>
        public DateTime BookingPerDayTimeTo { get; set; }

        /// <summary>
        /// Get or Set care taker location
        /// </summary>
        public string CareTakerLocation { get; set; }

        /// <summary>
        /// Get or Set user location
        /// </summary>
        public string UserLocation { get; set; }

        /// <summary>
        /// Get or Set booking purpose
        /// </summary>
        public string BookingPurpose { get; set; }
    }
}