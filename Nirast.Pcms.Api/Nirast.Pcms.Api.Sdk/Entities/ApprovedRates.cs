using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
   public class ApprovedRates

    {
        /// <summary>
        /// Get or Set service id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set expected rate
        /// </summary>
        public float ExpectedRate { get; set; }

        /// <summary>
        /// Get or Set approved rate
        /// </summary>
        public float ApprovedRate { get; set; }

        /// <summary>
        /// Get or Set care taker user id
        /// </summary>
        public float CareTakerUserId { get; set; }
    }
}
