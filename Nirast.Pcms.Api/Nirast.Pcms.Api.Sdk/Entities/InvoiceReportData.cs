using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class InvoiceReportData
    {
        public string Category { get; set; }
        public string ClientName { get; set; }
        public string CaretakerName { get; set; }
        public string WorkShiftName { get; set; }
        public int ClientId { get; set; }
        public int CaretakerUserId { get; set; }
        public int WorkShiftId { get; set; }
        public double BillingHours { get; set; }
        public double HolidayHours { get; set; }
        public double HolidayRate { get; set; }
        public double NormalHours { get; set; }
        public double TypeRate { get; set; }
        public double HolidayPay { get; set; }
        public double NormalPay { get; set; }
        public double TotalPay { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string TimeShiftName { get; set; }
    }
}