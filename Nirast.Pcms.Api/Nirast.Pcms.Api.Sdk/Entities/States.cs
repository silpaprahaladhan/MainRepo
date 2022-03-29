using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class States
    {
        /// <summary>
        /// Get or Set state id
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Get or Set country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Get or Set Tax Amount
        /// </summary>
        public float TaxPercent { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string PhoneCode { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string Symbol { get; set; }
    }
}