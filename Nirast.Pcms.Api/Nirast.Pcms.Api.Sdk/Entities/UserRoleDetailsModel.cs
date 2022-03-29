using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class UserRoleDetailsModel
    {
        /// <summary>
        /// Get or Set the WorkRoleTableId
        /// </summary>
        public int WorkRoleTableId { get; set; }

        /// <summary>
        /// Get or Set the UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set the WorkRoleId
        /// </summary>
        public int WorkRoleId { get; set; }

        /// <summary>
        /// Get or Set the CountryId
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set the StateId
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set the CityId
        /// </summary>
        public int CityId { get; set; }


    }
}