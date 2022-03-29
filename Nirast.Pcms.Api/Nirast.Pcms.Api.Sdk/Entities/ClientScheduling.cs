using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class ClientScheduling
    {
        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string BookingDate { get; set; }

        /// <summary>
        /// Get or Set ClientName 
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Get or Set Work Mode
        /// </summary>
        public string WorkMode { get; set; }

        /// <summary>
        /// Get or Set TimeShift
        /// </summary>
        public string TimeShift { get; set; }
        public string Description { get; set; }
    }
}
