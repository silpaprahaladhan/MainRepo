using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class CaretakerBookingReportModel
    {
        public int? CategoryId { get; set; }

        public int? ServiceId { get; set; }

        public int? CaretakerName { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }
    }
}
