using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{       
    public class PaymentReportDetails
    {
	//	cast((((t1.TotalHours-isnull(t2.HoildayHours,0))*isnull(t1.ServiceRate,0)) + (isnull(t2.HoildayHours,0)*isnull(t1.ServiceRate,0))*isnull(t2.HolidayPayValue,0)) as decimal (10,2)) as ,
	//	cast((((t1.TotalHours-isnull(t2.HoildayHours,0))*isnull(t1.DisplayRate,0)) + (isnull(t2.HoildayHours,0)*isnull(t1.DisplayRate,0))*isnull(t2.HolidayPayValue,0)) as decimal (10,2)) as TotalDisplayAmount,
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
        public float Amount { get; set; }
        public float PayingAmount { get; set; }
        public float DisplayAmount { get; set; }
        public float HoildayAmount { get; set; }
        public float TotalPayingAmount { get; set; }
        public string Symbol { get; set; }
        public string Currency { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentStatus { get; set; }
        public float HoildayHours { get; set; }
        public float TotalHours { get; set; }
        public float DisplayRate { get; set; }
        public float ServiceRate { get; set; }
        public string CareRecipient { get; set; }
        public string UserLocation { get; set; }
    }
}

