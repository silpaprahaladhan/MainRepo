using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class PaymentReportDetails
    {
        public int Id { get; set; }
        public int PublicUserId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Username { get; set; }
        public string BookingDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CareTakerTypeId { get; set; }
        public string CareTakerType { get; set; }
        public int CareTaker { get; set; }
        public string CareTakerName { get; set; }
        public string Service { get; set; }
        public string FromTime { get; set; }
        public string EndTime { get; set; }
        public string Amount { get; set; }
        public string PayingAmount { get; set; }
        public string DisplayAmount { get; set; }
        public string HoildayAmount { get; set; }
        public string TotalPayingAmount { get; set; }
        public string Symbol { get; set; }
        public string Currency { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentStatus { get; set; }
        public string HoildayHours { get; set; }
        public string TotalHours { get; set; }
        public string DisplayRate { get; set; }
        public string ServiceRate { get; set; }
        public string CareRecipient { get; set; }
        public string UserLocation { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoicePrefix { get; set; }

    }
}