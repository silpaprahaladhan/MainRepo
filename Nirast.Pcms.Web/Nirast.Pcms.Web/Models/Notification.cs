using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Notification
    {

        /// <summary>
        /// Get or Set Booking Id
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public DateTime FromDateTime{ get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Get or Set the Booked User
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string Location { get; set; }

        ///// <summary>
        ///// Get or Set the location
        ///// </summary>
        //public string Purpose { get; set; }
    }
}