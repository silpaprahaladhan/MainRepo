using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{

    public class ContactModel
    {
        /// <summary>
        /// Get or Set name
        ///</summary>

        public string Name { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Get or Set email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Get or Set description
        /// </summary>
        public string Description { get; set; }

        public string SiteURL { get; set; }

    }
}
