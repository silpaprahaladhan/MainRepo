using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class NotificationDetails
    {
        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public string BookingDate { get; set; }

        /// <summary>
        /// Get or Set the Booked User
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public DateTime FromDateTime { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public DateTime ToDateTime { get; set; }

        /// <summary>
        /// Get or Set the Service
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Get or Set the Status
        /// </summary>
        public string Status { get; set; }

        public string CareRecipient { get; set; }

        public string HouseName { get; set; }
        public string Phone1 { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }
        public string Answer6 { get; set; }
    }
}
