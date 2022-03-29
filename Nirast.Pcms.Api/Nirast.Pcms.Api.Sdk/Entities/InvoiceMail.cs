using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class InvoiceMail
    {
        // <summary>
        /// Get or Set Booking Id
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Get or Set the EmailId
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// Get or Set the payment Link
        /// </summary>
        public string paymentLink { get; set; }
        public float TotalPayingAmount { get; set; }

        public float TotalDisplayAmount { get; set; }

        public string SiteUrl { get; set; }
        public string InvoicePrefix { get; set; }

        public int InvoiceNo { get; set; }

        public byte[] Attachment { get; set; }
        public DateTime InvoiceDate { get; set; }

    }
}
