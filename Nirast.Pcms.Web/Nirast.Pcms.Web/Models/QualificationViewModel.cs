using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class QualificationViewModel
    {
        /// <summary>
        /// Get or Set Designation Id
        /// </summary>
        public int QualificationId { get; set; }

        /// <summary>
        /// Get or Set Designation
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Qualification")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        public string Qualification { get; set; }
    }
}