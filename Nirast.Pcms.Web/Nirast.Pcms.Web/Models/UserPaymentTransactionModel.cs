using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class UserPaymentTransactionModel
    {
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>
        /// The transaction identifier.
        /// </value>
        public int TransactionId { get; set; }

        /// <summary>
        /// Get or Set the invoice number
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Get or Set the date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Get or Set the amount
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Get or Set the status
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Get or Set the method description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get or Set cash transfer method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }

        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the transaction number.
        /// </summary>
        /// <value>
        /// The transaction number.
        /// </value>
        public string TransactionNumber { get; set; }

        /// <summary>
        /// Gets or sets the transaction number.
        /// </summary>
        /// <value>
        /// The transaction number.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the transaction details.
        /// </summary>
        /// <value>
        /// The transaction details.
        /// </value>
        public string TransactionDetails { get; set; }

        public string SiteURL { get; set; }
    }
}