using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Countries
    {
        /// <summary>
        /// Get or Set country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone code.
        /// </summary>
        /// <value>
        /// The phone code.
        /// </value>
        public string PhoneCode { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        /// <value>
        /// The currency symbol.
        /// </value>
        public string CurrencySymbol { get; set; }

        public bool Isdefault { get; set; }
    }
}