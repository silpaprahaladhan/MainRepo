using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
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
        public string RoleName { get; set; }
    }

    public class Privileges
    {
        public int PrivilegeId { get; set; }

        public int RoleId { get; set; }

        public int ModuleId { get; set; }

        public string ModuleName { get; set; }

        public bool AllowView { get; set; }

        public bool AllowEdit { get; set; }

        public bool AllowDelete { get; set; }
    }

    public class SaveRolePrivileges
    {
        public int RoleId { get; set; }

        public List<Privileges> Privileges { get; set; }
    }

    public class GetRolePrivilegeModel
    {
        public int RoleId { get; set; }
        public int ModuleID { get; set; }
        public string PrivilegeType { get; set; }
    }

    public class RoleModulePrivileges
    {
        public bool AllowView { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
    }
}
