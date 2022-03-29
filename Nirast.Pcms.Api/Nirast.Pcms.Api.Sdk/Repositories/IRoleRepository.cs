using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
   public interface IRoleRepository: IGenericRepository<Roles>
    {
        /// <summary>
        /// method to add roles
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<int> AddRoles(Roles roles);

        /// <summary>
        /// method to get roles
        /// </summary>
        /// <param name="QualificationId"></param>
        /// <returns></returns>
      Task<IEnumerable<Roles>> RetrieveRoles(int roleId);

        /// <summary>
        /// method to dalete roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteRoles(int id);

       Task<IEnumerable<Privileges>> SelectRolePrivileges(int RoleId);

        Task<int> SaveRolePrivileges(SaveRolePrivileges saveRolePrivileges);

        Task<RoleModulePrivileges> GetRolePrivilege(GetRolePrivilegeModel getRolePrivilege);

    }
}
