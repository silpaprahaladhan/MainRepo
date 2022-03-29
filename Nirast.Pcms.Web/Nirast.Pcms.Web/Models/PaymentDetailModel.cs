using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class PaymentDetailModel
    {
        public string InvoiceNumber { get; set; }
        public string Total { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}