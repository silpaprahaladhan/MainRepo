using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class PaymentViewModel
    {
        public DateTime PaymentDate { get; set; }
        public int PaymentId { get; set; }
        public int PaymentToUserId { get; set; }
        public int PaymentByUserId { get; set; }
        public float ServiceAmountPaid { get; set; }
        public int PaymentModeId { get; set; }
        public int CardId { get; set; }
        public string PaymentRemarks { get; set; }
        public string PaymentService { get; set; }
        public string ServiceAmountPaidString { get; set; }

    }

    public class PaymentHistoryViewModel : PaymentViewModel
    {
        public string PaymentToUser { get; set; }
        public string PaymentByUser { get; set; }
        public string PaymentMode { get; set; }
    }
}