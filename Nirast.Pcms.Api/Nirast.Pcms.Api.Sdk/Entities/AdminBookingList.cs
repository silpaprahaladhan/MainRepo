using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class AdminBookingList
    {
        /// <summary>
        /// Get or Set Booking Id
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Get or Set Booking Date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Get or Set the Booked User
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string UserLocation { get; set; }

        /// <summary>
        /// Get or Set the Caretaker
        /// </summary>
        public string Caretaker { get; set; }

        /// <summary>
        /// Get or Set the CaretakerLocation
        /// </summary>
        public string CaretakerLocation { get; set; }


    }
}
