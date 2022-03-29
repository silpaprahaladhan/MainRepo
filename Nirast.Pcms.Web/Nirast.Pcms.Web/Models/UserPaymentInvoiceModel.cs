using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class UserPaymentInvoiceModel
    {
        /// <summary>
        /// Get or set the invoice number
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Get or set the date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Get oe set the first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get oe Set the last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Get or set the address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Get or set the phone number
        /// </summary>
        public string PrimaryPhoneNo { get; set; }

        /// <summary>
        /// Get or Set the booing id
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Get or set the amount
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Get or set the amount
        /// </summary>
        public float TaxAmount { get; set; }

        /// <summary>
        /// Get or set the amount
        /// </summary>
        public float TotalAmount { get; set; }

        /// <summary>
        /// Get or set the status
        /// </summary>
        public bool PaidStatus { get; set; }

        /// <summary>
        /// get or set the country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Get or set the state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Ge t or set the city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Ge t or set the city
        /// </summary>
        public string Currency { get; set; }

        public string CurrencySymbol { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<InvoicePaymentModel> AllPayments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<UserPaymentTransactionModel> PaymentHistory { get; set; }
    }
}