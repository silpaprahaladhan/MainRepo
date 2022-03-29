using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class InvoicePaymentModel
    {
        /// <summary>
        /// Get or Set the invoice number
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Get or Set invoice description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get or Set the date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Get or Set the amount
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Get or Set the amount
        /// </summary>
        public float TaxAmount { get; set; }

        /// <summary>
        /// Get or Set the amount
        /// </summary>
        public float TotalAmount { get; set; }

        /// <summary>
        /// Get or Set the status
        /// </summary>
        public bool PaidStatus { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }
    }
}