using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Web.Models
{
    public class Cities
    {
        /// <summary>
        /// Get or Set city id
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Get or Set city name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(30, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "CityName")]
        public string CityName { get; set; }

        /// <summary>
        ///  Get or Set country id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CountryId")]
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Get or Set state id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "StateId")]
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state name
        /// </summary>
        public string StateName { get; set; }
        public int UserId { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

    }
}
