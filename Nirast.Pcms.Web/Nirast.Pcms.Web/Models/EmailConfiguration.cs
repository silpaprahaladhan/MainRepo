using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class EmailConfiguration
    {
        public int ConfigId { get; set; }
        [Required(ErrorMessage = "* Required")]
        [StringLength(30, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string MailHost { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public int MailPort { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(30, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string ConfigName { get; set; }

        [Required(ErrorMessage = "* Required")]
        public bool SSL { get; set; }
        public bool IsDefault { get; set; }
    }

    public class EmailTypeViewModel
    {
        public string Emailtype { get; set; }
        public int EmailtypeId { get; set; }
    }


    public class EmailTypeConfiguration
    {
        public int ConfigId { get; set; }

        public string Emailtype { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int EmailtypeId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Maximum {1} characters allowed")]
        public string FromEmail { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string Password { get; set; }
        public int BranchId { get; set; }
        public Nullable<int> CountryId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a State")]
        public Nullable<int> StateId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a City")]
        public Nullable<int> CityId1 { get; set; }
        public Nullable<int> branchId1 { get; set; }
        public string BranchName { get; set; }
    }
}