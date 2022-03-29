using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    [Table("PCMS_Countries")]
    public class Country
    {
        /// <summary>
        /// Get or Set country id
        /// </summary>
        [Column("Country_Id")]
        public int Country_Id { get; set; }

        /// <summary>
        /// Get or Set country code
        /// </summary>
        [Column("Country_Code")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        [Column("Country_Name")]
        public string CountryName { get; set; }

        /// <summary>
        /// Get or Set the country is active or not
        /// </summary>
        [Column("Country_RecordActive")]
        public bool CountryRecordActive { get; set; }

        /// <summary>
        /// Get or Set the created user
        /// </summary>
        [Column("Country_CreatedUser")]
        public int CountryCreatedUser { get; set; }

        /// <summary>
        /// Get or Set the created time
        /// </summary>
        [Column("Country_CreatedTime")]
        public DateTime CountryCreatedTime { get; set; }
    }
}