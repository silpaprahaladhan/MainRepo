using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Services
    {
        /// <summary>
        /// Get or Set service id
        /// </summary>
        public int ServiceId { get;set; }

        /// <summary>
        /// Get or Set service name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the service image.
        /// </summary>
        /// <value>
        /// The service image picture.
        /// </value>
        public byte[] ServicePicture { get; set; }

        public int ServiceOrder { get; set; }

        /// <summary>
        /// Gets or Sets Service Description
        /// </summary>
        public string ServiceDescription { get; set; }
    }
}