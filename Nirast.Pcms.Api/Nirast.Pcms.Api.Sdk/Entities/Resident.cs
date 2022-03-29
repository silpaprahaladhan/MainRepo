using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Resident
    {
        /// <summary>
        /// Gets or Sets resident id
        /// </summary>
        public int ResidentId { get; set; }

        /// <summary>
        /// Gets or sets resident name
        /// </summary>
        public string ResidentName { get; set; }

        /// <summary>
        ///  Gets or sets street name
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        ///  Gets or sets resident primary phone
        /// </summary>
        public string OtherInfo { get; set; }

        /// <summary>
        /// Gets or Sets client id 
        /// </summary>
        public string ClientId { get; set; }
    }
}
