using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class UserBooking
    {
        /// <summary>
        /// Get or Set Booking Id
        /// </summary>
        public int BookingId { get; set; }
        public int PublicUserId { get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Get or Set BookedUser 
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set Service
        /// </summary>
        public string Service { get; set; }
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set FromTime
        /// </summary>
        public DateTime FromDateTime { get; set; }

        /// <summary>
        /// Get or Set ToTime
        /// </summary>
        public DateTime ToDateTime { get; set; }
        public DateTime Start { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Get or Set InvoicePath
        /// </summary>
        public string InvoicePath { get; set; }
     

        /// <summary>
        /// Get or Set InvoiceNo
        /// </summary>
        public int InvoiceNo { get; set; }
        public bool IsFullDay { get; set; }
    }
}