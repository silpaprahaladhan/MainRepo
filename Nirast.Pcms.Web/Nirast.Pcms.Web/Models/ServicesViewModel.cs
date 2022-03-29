using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class ServicesViewModel
    {
        /// <summary>
        /// Get or Set service id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set service order
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "ServiceOrder")]
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "Service Order must be a non zero numeric")]
        public int ServiceOrder { get; set; }

        /// <summary>
        /// Get or Set service name
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public byte[] ServicePicture { get; set; }

        /// <summary>
        /// Gets or sets the Service image.
        /// </summary>
        /// <value>
        /// The Service image.
        /// </value>
        public HttpPostedFileBase ServicePicImage { get; set; }

        /// <summary>
        /// Gets or Sets the Description
        /// </summary>
        public string ServiceDescription { get; set; }
    }

    public class ServiceBasedListViewModel
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public int ServiceBookedCount { get; set; }
    }

    public class YearlyBookingCount
    {
        public int MonthlyCount { get; set; }

        public string BookingsMonth { get; set; }
    }
}