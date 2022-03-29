using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CategoryModel
    {
        /// <summary>
        /// Get or Set category id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Get or Set Category name
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Get or set color
		/// </summary>
		public string Color { get; set; }
    }
}
