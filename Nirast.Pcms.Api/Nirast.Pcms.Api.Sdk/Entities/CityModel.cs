using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CityModel
    {
        /// <summary>
        /// Get or Set city id
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Get or Set city name
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        ///  Get or Set country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Get or Set state id
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state name
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// Get or Set created user id
        /// </summary>
        public string CityCreatedUserId { get; set; }

        /// <summary>
        /// Get or Set created date of city
        /// </summary>
        public DateTime CityCreatedDate { get; set; }

        /// <summary>
        /// Get or Set updated user id
        /// </summary>
        public string CityUpdatedUserId { get; set; }

        /// <summary>
        /// Get or Set updated date of city
        /// </summary>
        public DateTime CityUpdatedDate { get; set; }

        /// <summary>
        /// Get or Set the city is active or not
        /// </summary>
        public bool CityRecordActive { get; set; }
    }
}