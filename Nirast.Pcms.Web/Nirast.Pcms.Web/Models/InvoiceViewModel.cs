using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class InvoiceViewModel
    {
       public DateTime InvoiceDate { get; set; }
        public string InvoiceDateString { get; set; }

        public string CaretakerName { get; set; }
        public string CaretakerCategory { get; set; }
        public string Shift { get; set; }
        public string TimeIn { get; set; }
        public string Timeout { get; set; }
        public float Hours { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
    }

    public class CommissionInputs
    {
        public int BranchId { get; set; }

        public DateTime Date { get; set; }
    }
}