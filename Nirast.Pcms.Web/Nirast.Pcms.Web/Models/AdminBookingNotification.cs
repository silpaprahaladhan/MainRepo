using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class AdminBookingNotification
    {

        /// <summary>
        /// Get or Set Booking Id
        /// </summary>
        public int BookingId { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public DateTime BookingDateTime { get; set; }

        /// <summary>
        /// Get or Set the Booked User
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set the Caretaker
        /// </summary>
        public string Caretaker { get; set; }

        /// <summary>
        /// Get or Set the CaretakerLocation
        /// </summary>
        public string CaretakerLocation { get; set; }

        /// <summary>
        /// Gets or sets from date time.
        /// </summary>
        /// <value>
        /// From date time.
        /// </value>
        public DateTime FromDateTime { get; set; }

        /// <summary>
        /// Gets or sets to date time.
        /// </summary>
        /// <value>
        /// To date time.
        /// </value>
        public DateTime ToDateTime { get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string EmailId { get; set; }
        public string Purpose { get; set; }
        public int InvoiceNumber { get; set; }
        public string RejectedReason { get; set; }
        public string CancelledReason { get; set; }
    }
}