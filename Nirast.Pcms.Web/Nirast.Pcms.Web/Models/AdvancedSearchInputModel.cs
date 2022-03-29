using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class AdvancedSearchInputModel
    {

        #region public properties

        /// <summary>
        /// Get or Set the location
        /// </summary>
        /// 
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only alphabets are allowed")]
        public string Location { get; set; }

        /// <summary>
        /// Get or Set the category
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// Get or Set from date
        /// </summary>
        [Required (ErrorMessage = "* Required")]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "From Time")]
        public DateTime? FromTime { get; set; }
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "To Time")]
        public DateTime? ToTime { get; set; }

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
        /// 
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphanumeric values are allowed")]
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
        /// Get or Set gender id
        /// </summary>
        public int? Gender { get; set; }
        
        #endregion
    }
}