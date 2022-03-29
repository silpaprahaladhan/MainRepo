using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class PublicUserPaymentInvoiceInfo
    {
        /// <summary>
        /// Get or set InvoiceNo
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Get or set InvoicePath
        /// </summary>
        public string InvoicePath { get; set; }
        public string InvoicePrefix { get; set; }
    }
}