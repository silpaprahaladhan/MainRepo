using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
	public class PublicUserCaretakerBooking
	{

        public int Id { get; set; }
        public int BookingId { get; set; }
        public int SchedulingId { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoicePrefix { get; set; }
        public int PublicUserId { get; set; }
        public string ClientName { get; set; }

        public int CareTakerType { get; set; }
        public string CareTakerTypeName { get; set; }

        public string ServiceTypeName { get; set; }
        public int WorkMode { get; set; }
        public string WorkModeName { get; set; }

        public int? WorkTime { get; set; }
        public string WorkTimeName { get; set; }
        public DateTime BookingStartTime { get; set; }
        public DateTime BookingEndTime { get; set; }

        public string ThemeColor { get; set; }
        public string SiteUrl { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        //public Nullable<System.DateTime> End { get; set; }
        public string Description { get; set; }

        public int CareTaker { get; set; }
        public string CareTakerName { get; set; }

        public string WorkTimeDetails { get; set; }
        public string WorkShifDetails { get; set; }
        public string ContactPerson { get; set; }

        public bool CustomTiming { get; set; }
        public string FromTime { get; set; }
        public string EndTime { get; set; }
        public List<BookingDate> PublicUserSchedulingDate { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDateTime { get; set; }

        public class BookingDate
        {
            public DateTime Date { get; set; }
            public double Hours { get; set; }
        }
        public class CalenderBookingEventInput
        {
            public int BookingId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }


    }
}
