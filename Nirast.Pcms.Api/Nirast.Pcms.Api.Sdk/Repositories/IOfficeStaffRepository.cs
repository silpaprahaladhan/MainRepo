using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Repositories
{
   public interface IOfficeStaffRepository: IGenericRepository<OfficeStaffRegistration>

    {

        /// <summary>
        /// method to add office staff details
        /// </summary>
        /// <param name="officeStaff"></param>
        /// <returns></returns>
        Task<int> AddOfficeStaff(OfficeStaffRegistration officeStaff);

        /// <summary>
        /// method to get office staff profile
        /// </summary>
        /// <returns></returns>
        Task<OfficeStaffRegistration> GetOfficeStaffDetails(int id);

        /// <summary>
        /// method to get all staff details
        /// </summary>
        /// <returns></returns>
       Task<IEnumerable<OfficeStaffRegistration>> GetOfficeStaffDetails();

        /// <summary>
        /// method to get all staff details
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OfficeStaffRegistration>> GetOfficeStaffDetailsByLocation(LocationSearchInputs inputs);

        /// <summary>
        /// method to delete office staff details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteOfficeStaffDetails(int id);
    }
}
