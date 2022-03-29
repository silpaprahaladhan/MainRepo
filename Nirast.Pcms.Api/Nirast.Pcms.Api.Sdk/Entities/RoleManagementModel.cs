using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class RoleManagementModel
    {
        /// <summary>
        /// Get or Set userprivilage id
        /// </summary>
        public int UserPrivilageId { get; set; }

        /// <summary>
        /// Get or Set usertype id
        /// </summary>
        public int UserPrivilageUserTypeId { get; set; }

        /// <summary>
        /// Get or Set user id
        /// </summary>
        public int UserPrivilageUserId { get; set; }

        /// <summary>
        /// Get or Set the status of view
        /// </summary>
        public bool UserPrivilageView { get; set; }

        /// <summary>
        /// Get or Set the status of add
        /// </summary>
        public bool UserPrivilageAdd { get; set; }

        /// <summary>
        /// Get or Set the status of edit
        /// </summary>
        public bool UserPrivilageEdit { get; set; }

        /// <summary>
        /// Get or Set the status of delete
        /// </summary>
        public bool UserPrivilageDelete { get; set; }

        /// <summary>
        /// Get or Set created user id
        /// </summary>
        public int UserPrivilageCreatedUserId { get; set; }

        /// <summary>
        /// Get or Set created user name
        /// </summary>
        public string UserPrivilageCreatedUserName { get; set; }

        /// <summary>
        /// Get or Set the created date
        /// </summary>
        public DateTime UserPrivilageCreatedDate { get; set; }

        /// <summary>
        /// Get or Set updated user id
        /// </summary>
        public int UserPrivilageUpdatedUserId { get; set; }

        /// <summary>
        /// Get or Set updated user name
        /// </summary>
        public string UserPrivilageUpdatedUserName { get; set; }

        /// <summary>
        /// Get or Set the updated date
        /// </summary>
        public DateTime UserPrivilageUpdatedDate { get; set; }

        /// <summary>
        /// Get or Set the rolemanagement is active or nor
        /// </summary>
        public bool RecordActive { get; set; }
    }
}