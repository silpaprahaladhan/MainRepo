using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class DesignationViewModel
    {
        /// <summary>
        /// Get or Set Designation Id
        /// </summary>
        public int DesignationId { get; set; }

        /// <summary> 
        /// Get or Set Designation
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Designation")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        public string Designation { get; set; }
    }
}