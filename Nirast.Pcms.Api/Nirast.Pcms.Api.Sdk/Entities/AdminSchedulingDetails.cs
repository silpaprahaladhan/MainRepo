using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class AdminSchedulingDetails
    {
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
        /// Get or Set the Start Date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Get or Set the End Date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Get or Set the WorkMode
        /// </summary>
        public string WorkMode { get; set; }

        /// <summary>
        /// Get or Set the Time Shift
        /// </summary>
        public string TimeShift { get; set; }
    }
}
