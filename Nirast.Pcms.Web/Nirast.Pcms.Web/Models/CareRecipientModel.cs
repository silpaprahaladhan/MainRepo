using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class CareRecipientModel
    {
        public int PatientId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }


        public int CountryId { get; set; }

        [Required]
        public string Country { get; set; }

        public int StateId { get; set; }

        [Required]
        public string State { get; set; }


        public int CityId { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime FromTime { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public DateTime ToTime { get; set; }

        [Required]
        public string ServiceRequiredDetails { get; set; }

        public List<PatientQuestionnaire> PatientQuestionnaires { get; set; }

    }
}