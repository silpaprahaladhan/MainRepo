using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{

    public class AdvancedSearchInputModel
    {
        #region public properties

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set the category
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Get or Set the price
        /// </summary>
        public float? Price { get; set; }

        /// <summary>
        /// Get or Set the services
        /// </summary>
        public int? Services { get; set; }

        /// <summary>
        /// Get or Set the profile Id
        /// </summary>
        public string ProfileId { get; set; }

        /// <summary>
        /// Get or Set the experience
        /// </summary>
        public float? Experience { get; set; }

        /// <summary>
        /// Get or Set the zip code
        /// </summary>
        public int? ZipCode { get; set; }

        /// <summary>
        /// Get or Set the country
        /// </summary>
        public int? Country { get; set; }

        /// <summary>
        /// Get or set the state
        /// </summary>
        public int? State { get; set; }
        
        /// <summary>
        /// Get or Set the city
        /// </summary>
        public int? City { get; set; }

        /// <summary>
        /// Get or Set the from time
        /// </summary>
        public DateTime? FromTime { get; set; }

        /// <summary>
        /// Get or Set the to time
        /// </summary>
        public DateTime? ToTime { get; set; }

        /// <summary>
        /// Get or Set gender id
        /// </summary>
        public int? Gender { get; set; }
        #endregion
    }

}