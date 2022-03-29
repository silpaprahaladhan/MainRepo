using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Cities
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
        public int BranchId { get; set; }
        public string BranchName { get; set; }

    }
}
