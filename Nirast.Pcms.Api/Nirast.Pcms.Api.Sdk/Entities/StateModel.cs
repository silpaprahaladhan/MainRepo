using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class StateModel
    {
        /// <summary>
        /// Get or Set state id
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Get or Set state code
        /// </summary>
        public string StateCode { get; set; }

        //Get or Set state name 
        public string StateName { get; set; }

        /// <summary>
        /// Get or Set created user id
        /// </summary>
        public int StateCreatedUserId { get; set; }

        /// <summary>
        /// Get or Set created date of state
        /// </summary>
        public DateTime StateCreatedDate { get; set; }

        /// <summary>
        /// Get or Set updated user id
        /// </summary>
        public int StateUpdatedUserId { get; set; }

        /// <summary>
        /// Get or Set Update date of state
        /// </summary>
        public DateTime StateUpdatedDate { get; set; }

        /// <summary>
        /// Get or Set the state is active or not
        /// </summary>
        public bool StateRecordActive { get; set; }
    }
}