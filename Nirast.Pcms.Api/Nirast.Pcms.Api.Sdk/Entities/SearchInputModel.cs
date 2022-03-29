using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class SearchInputModel
    {
        /// <summary>
        /// Get or Set year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Get or Set month
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        public DateTime ToDate { get; set; }
    }
}