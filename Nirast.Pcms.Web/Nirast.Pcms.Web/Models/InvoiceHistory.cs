using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class InvoiceHistory
    {
        public string InvoiceNumber { get; set; }

        public int? ClientId { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>

        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>

        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Get or Set the Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Get or Set the Month
        /// </summary>
        public int? Month { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public int? InvoiceSearchInputId { get; set; }

        public Nullable<int> CountryId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a State")]
        public Nullable<int> StateId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a City")]
        public Nullable<int> CityId1 { get; set; }
        public Nullable<int> BranchId2 { get; set; }
    }
}