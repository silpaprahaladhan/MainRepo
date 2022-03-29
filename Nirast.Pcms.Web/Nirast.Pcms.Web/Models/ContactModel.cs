using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class ContactModel
    {
        /// <summary>
        /// Get or Set name
        ///</summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string Name { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
       
        [Required(ErrorMessage = "* Required")]
      
        public string Phone { get; set; }


        /// <summary>
        /// Get or Set email
        /// </summary>
       
        [Required(ErrorMessage = "* Required")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        /// <summary>
        /// Get or Set description
        /// </summary>
         
        [Required(ErrorMessage = "* Required")]
        
        public string Description { get; set; }

        public string SiteURL { get; set; }

    }
}