using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Resident
    {
        /// <summary>
        /// Gets or Sets resident id
        /// </summary>
        public int ResidentId { get; set; }

        /// <summary>
        /// Gets or sets resident name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(100, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string ResidentName { get; set; }

        /// <summary>
        ///  Gets or sets street name
        /// </summary>
       
        [Required(ErrorMessage = "* Required")]
        [StringLength(200, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string ClientName { get; set; }

        /// <summary>
        ///  Gets or sets resident primary phone
        /// </summary>
        //[Required(ErrorMessage = "* Required")]
        public string OtherInfo { get; set; }

        /// <summary>
        /// Gets or Sets client id 
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public string ClientId { get; set; }
    }
}