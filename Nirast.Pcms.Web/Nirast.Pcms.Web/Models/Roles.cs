using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Roles
    {
        /// <summary>
        /// Get or Set the role id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Get or set the role name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "Roles")]
        public string RoleName { get; set; }
    }

    public class GetRolePrivilegeModel
    {
        public int RoleId { get; set; }
        public int ModuleID { get; set; }
    }

    public class RoleModulePrivileges
    {
        public bool AllowView { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
    }
}