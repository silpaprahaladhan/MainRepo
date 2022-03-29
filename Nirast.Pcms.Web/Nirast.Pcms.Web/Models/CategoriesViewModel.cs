using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class CategoryViewModel
    {
        /// <summary>
        /// Get or Set category id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Get or Set Category name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(30, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "Name")]
        public string Name { get; set; }

		/// <summary>
		/// Get or set Color
		/// </summary>
		public string Color { get; set; }
       
        public Dictionary<string,string> ColorList { get; set; }
    }

    public class ManageSubcategoryViewModel
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public int SubCategorySortOrder { get; set; }
        public int SubCategoryCreatedUserId { get; set; }
        public string SubcategoryCreatedUsername { get; set; }
        public DateTime SubcategoryCreatedTime { get; set; }
        public bool SubcategoryRecordActive { get; set; }
    }
}