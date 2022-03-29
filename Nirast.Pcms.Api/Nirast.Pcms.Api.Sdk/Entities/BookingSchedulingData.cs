using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class BookingSchedulingData
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
        /// Get or Set FromTime
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Get or Set ToTime
        /// </summary>
        public DateTime EndDateTime { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string ClientName { get; set; }

        public string CareTakerTypeName { get; set; }

        public string WorkModeName { get; set; }
        public string WorkTimeName { get; set; }
        
        public string Description { get; set; }


        public string CareTakerName { get; set; }

        public string FromTime { get; set; }
        public string EndTime { get; set; }

        public string Type { get; set; }

        public int CaretakerId { get; set; }

        public string ClientLocation { get; set; }
    }
}
