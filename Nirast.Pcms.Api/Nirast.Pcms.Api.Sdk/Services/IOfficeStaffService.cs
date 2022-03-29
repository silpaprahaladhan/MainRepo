using Nirast.Pcms.Api.Sdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Services
{
   public interface IOfficeStaffService
    {
        /// <summary>
        /// method to add office stff details
        /// </summary>
        /// <param name="officeStaff"></param>
        /// <returns></returns>
        Task<int> AddOfficeStaff(OfficeStaffRegistration officeStaff);

        /// <summary>
        /// method to get office staff details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OfficeStaffRegistration> GetOfficeStaffProfile(int id);

        /// <summary>
        /// method to get all office staff details
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OfficeStaffRegistration>> GetAllOfficeStaffDetails();

        /// <summary>
        /// method to get all office staff details
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OfficeStaffRegistration>> GetAllOfficeStaffDetailsByLocation(LocationSearchInputs inputs);

        /// <summary>
        /// method to delate office staff details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteOfficeStaffProfile(int id);
    }
}
