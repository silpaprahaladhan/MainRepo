using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class PaymentAdvancedSearch
    {
        /// <summary>
        /// Get or Set the category
        /// </summary>
        public int? Category { get; set; }

        public int? CareTakerId { get; set; }
        public int? ClientId { get; set; }
        /// <summary>
        /// Get or Set the Service
        /// </summary>
        public int? Service { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>

        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>

        public DateTime? ToDate { get; set; }
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Get or Set the Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is orientation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is orientation; otherwise, <c>false</c>.
        /// </value>
        public bool IsOrientation { get; set; }
        public string branch { get; set; }
        public int? BranchId { get; set; }
    }
}

