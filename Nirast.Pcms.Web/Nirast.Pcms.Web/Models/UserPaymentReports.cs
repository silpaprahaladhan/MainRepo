using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class PaymentReport
    {
        /// <summary>
        /// Get or Set the Caretaker Type
        /// </summary>
        public int? CaretakerType { get; set; }

        /// <summary>
        /// Get or Set the Caretaker Type
        /// </summary>
        public int? CareTaker { get; set; }
     
        /// <summary>
        /// Get or Set from date
        /// </summary>

        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>

        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Get or Set the Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int TransactionStatus { get; set; }

        public int ServiceType { get; set; }
        public int? SearchType { get; set; }
    }

    public class CaretakerWiseSearchReport
    {

        /// <summary>
        /// Get or Set the Caretaker Type
        /// </summary>
        public string CareTaker { get; set; }
        public int PublicUserId { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>

        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>

        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Get or Set the Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int? Month { get; set; }

        public int ServiceId { get; set; }

        public int? SearchType { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}