using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    class PublicUserBookingHistory
    {
        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Get or Set BookedUser 
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set Service
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Get or Set Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set FromTime
        /// </summary>
        public string FromTime { get; set; }

        /// <summary>
        /// Get or Set ToTime
        /// </summary>
        public string ToTime { get; set; }
    }
}
