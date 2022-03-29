using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class PublicUserPaymentInvoiceInfo
    {
        public int InvoiceNumber { get; set; }

        public string InvoicePath { get; set; }
        public string InvoicePrefix { get; set; }
    }
}