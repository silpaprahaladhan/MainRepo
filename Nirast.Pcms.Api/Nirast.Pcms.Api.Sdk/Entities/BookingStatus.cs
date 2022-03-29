using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class BookingStatus
    {
        /// <summary>
        /// Get or Setbooking id
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Get or Set booking status id
        /// </summary>
        public int BookingStatusId { get; set; }

        /// <summary>
        /// Get or Set the accepted date
        /// </summary>
        public DateTime AcceptedDate { get; set; }

        /// <summary>
        /// Get or Set the rejected date
        /// </summary>
        public DateTime RejectedDate { get; set; }

        /// <summary>
        /// Get or Set the reason
        /// </summary>
        public string Reason { get; set; }
    }

}