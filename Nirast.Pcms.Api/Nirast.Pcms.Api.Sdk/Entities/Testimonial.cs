using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Testimonial
    {
        /// <summary>
        /// Get or Set first name
        ///</summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Get or Set last name
        ///</summary>
        public string Designation { get; set; }


        public string Description { get; set; }

        public string URL { get; set; }

        public int Rating { get; set; }

        public int TestimonialId { get; set; }
        
    }
}

