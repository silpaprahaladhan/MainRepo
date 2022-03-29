using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Web.Models.Enums;

namespace Nirast.Pcms.Web.Models
{
    public class PublicUserPaymentHistory
    {
        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string BookingDate { get; set; }

        /// <summary>
        /// Get or Set Service 
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Get or Set Caretaker
        /// </summary>
        public string Caretaker { get; set; }

        /// <summary>
        /// Get or Set Location
        /// </summary>
        public string PaymentDate { get; set; }

        /// <summary>
        /// Get or Set PaymentAmount
        /// </summary>
        public float PaymentAmount { get; set; }

        /// <summary>
        /// Get or Set the Currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Get or Set the Symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Get or Set PaymentStatus
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Get or Set BookingStatus
        /// </summary>
        public BookingStatus BookingStatus { get; set; }
    }
}