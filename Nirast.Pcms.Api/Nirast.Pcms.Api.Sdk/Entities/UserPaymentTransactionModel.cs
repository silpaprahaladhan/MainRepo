using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
   public class UserPaymentTransactionModel
    {
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
        /// Get or Set transaction number
        /// </summary>
        public string TransactionNumber { get; set; }

        /// <summary>
        /// Get pr Set message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Get pr Set message
        /// </summary>
        public string TransactionDetails { get; set; }


        public string SiteURL { get; set; }
    }
}
