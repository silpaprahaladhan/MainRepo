using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class AdminBookingDetails
    {
        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string BookingDate { get; set; }

        /// <summary>
        /// Get or Set the Booked User
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set the Caretaker
        /// </summary>
        public string Caretaker { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set the From Time
        /// </summary>
        public DateTime FromDateTime { get; set; }

        /// <summary>
        /// Get or Set the To Time
        /// </summary>
        public DateTime ToDateTime { get; set; }

        /// <summary>
        /// Get or Set the Service
        /// </summary>
        public string Service { get; set;}

        /// <summary>
        /// Get or Set the Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Get or Set the amount
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Get or Set the Currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Get or Set the Amount
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}
