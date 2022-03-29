using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class AdminSchedulingList
    {
        /// <summary>
        /// Get or Set Scheduling Id
        /// </summary>
        public int SchedulingId { get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string BookingDate { get; set; }

        /// <summary>
        /// Get or Set the Client
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Get or Set the Caretaker
        /// </summary>
        public string Caretaker { get; set; }

        /// <summary>
        /// Get or Set the Work Mode
        /// </summary>
        public string WorkMode { get; set; }

        /// <summary>
        /// Get or Set the Time Shift
        /// </summary>
        public string TimeShift { get; set; }
    }
}