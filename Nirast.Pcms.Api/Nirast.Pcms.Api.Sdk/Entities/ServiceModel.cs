using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class ServiceModel
    {
        /// <summary>
        /// Get or Set service id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set service code
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Get or Set service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Get or Set created date of service
        /// </summary>
        public DateTime ServiceCreatedDate { get; set; }

        /// <summary>
        /// Get or Set created user id
        /// </summary>
        public int ServiceCreatedUserId { get; set; }

        /// <summary>
        /// Get or Set updated date of service
        /// </summary>
        public DateTime ServiceUpdatedDate { get; set; }

        /// <summary>
        /// Get or Set updated user id
        /// </summary>
        public int ServiceUpdatedUserId { get; set; }

        /// <summary>
        /// Get or Set the service is activate or not
        /// </summary>
        public bool ServiceRecordActive { get; set; }
    }
}