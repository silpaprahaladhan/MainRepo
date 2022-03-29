using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Notification
    {

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string BookingId{ get; set; }

        /// <summary>
        /// Get or Set From Date
        /// </summary>
        public DateTime FromDateTime { get; set; }

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
        ///// Get or Set the Purpose
        ///// </summary>
        //public string Purpose { get; set; }
    }
}
