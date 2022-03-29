using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class SubCategoryModel
    {
        /// <summary>
        /// Get or Set subcategory id
        /// </summary>
        public int SubCategoryId { get; set; }

        /// <summary>
        /// Get or Set category id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Get or Set category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Get or Set subcategory name
        /// </summary>
        public string SubCategoryName { get; set; }

        /// <summary>
        /// Get or Set subcategory order
        /// </summary>
        public int SubCategorySortOrder { get; set; }

        /// <summary>
        /// Get or Set created user 
        /// </summary>
        public string SubCategoryCreatedUser { get; set; }

        /// <summary>
        /// Get or Set created date of subcategory
        /// </summary>
        public DateTime SubCategoryCreatedDate { get; set; }

        /// <summary>
        /// Get or Set updated user 
        /// </summary>
        public string SubCategoryUpdatedUser { get; set; }

        /// <summary>
        /// Get or Set updated date of subcategory
        /// </summary>
        public DateTime SubCategoryUpdatedDate { get; set; }

        /// <summary>
        /// Get or Set the subcategory is active or not
        /// </summary>
        public bool SubCategoryRecordActive { get; set; }
    }
}