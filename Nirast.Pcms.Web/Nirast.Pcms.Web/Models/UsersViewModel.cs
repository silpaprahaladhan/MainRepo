using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class UserRegistrationViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        public int CountryId { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string PhoneNo { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime UserRegisteredDate { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }

        public int UserTypeId { get; set; }
        public string UserRegisteredDateString { get; set; }
        public string UserLocation { get; set; }
    }

    public class UserCreditCarddRegistration : UserRegistrationViewModel
    {
        public string CardHolderName { get; set; }
        
        public string CardNo { get; set; }

        public int CardExpiryMonth { get; set; }

        public int CardExpiryYear { get; set; }
    }
}