using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Web.Models
{
    public class LocationSearchInputs
    {
        /// <summary>
        /// Get or Set CountryId
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Get or Set StateId
        /// </summary>
        public int? StateId { get; set; }

        /// <summary>
        /// Get or Set CityId
        /// </summary>
        public int? CityId { get; set; }
        //public int? Adminchecking { get; set; }
    }
}
