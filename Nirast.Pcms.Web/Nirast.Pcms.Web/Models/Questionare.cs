using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Questionare
    {
        /// <summary>
        /// Get or Set Questionare Id
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Get or SetQuestions
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Questions")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        public string Questions { get; set; }

        /// <summary>
        ///  Get or Set SortOrder
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [RegularExpression("[1-6]", ErrorMessage = "Only numerals 1 to 6 are allowed")]
        [Display(Name = "SortOrder")]
        public int SortOrder { get; set; }

    }

    public class PayPalAccount
    {
        public int PaypalId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string SecretKey { get; set; }
    }

    public class Testimonial
    {
        /// <summary>
        /// Get or Set first name
        ///</summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string ClientName { get; set; }

        /// <summary>
        /// Get or Set last name
        ///</summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "* Required")]
        public string URL { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression("^[1-5]$", ErrorMessage = "Rating must be numeric between 1 and 5")]
        public int Rating { get; set; }

        public int TestimonialId { get; set; }
    }
}