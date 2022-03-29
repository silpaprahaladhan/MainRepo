using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CareTakerRegistrationModel
    {
        #region public properties

        /// <summary>
        /// Get or Set registration id
        /// </summary>
        public int CareTakerId { get; set; }

        /// <summary>
        /// Get or Set profile id
        /// </summary>
        public int CareTakerProfileId { get; set; }

        /// <summary>
        /// Get or Set user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Get or Set first name
        /// </summary>
        public string CareTakerFirstName { get; set; }

        /// <summary>
        /// Get or Set last name
        /// </summary>
        public string CareTakerLastName { get; set; }

        /// <summary>
        /// Get or Set address
        /// </summary>
        public string CareTakerAddress { get; set; }

        /// <summary>
        /// Get or Set country id
        /// </summary>
        public int CareTakerCountryId { get; set; }

        /// <summary>
        /// Get or Set state id
        /// </summary>
        public int CareTakerStateId { get; set; }

        /// <summary>
        /// Get or Set city id
        /// </summary>
        public int CareTakerCityId { get; set; }

        /// <summary>
        /// Get or Set gender id
        /// </summary>
        public int CareTakerGenderId { get; set; }

        /// <summary>
        /// Get or Set phone number
        /// </summary>
        public string CareTakerPhoneNumber { get; set; }

        /// <summary>
        /// Get or Set email 
        /// </summary>
        public string CareTakerEmailId { get; set; }

        /// <summary>
        /// Get or Set the description about care taker
        /// </summary>
        public string CareTakerAboutMe { get; set; }

        /// <summary>
        /// Get or Set qualification id
        /// </summary>
        public int CareTakerQualifiicationId { get; set; }

        /// <summary>
        /// Get or Set current company
        /// </summary>
        public string CareTakerCurrentCompany { get; set; }

        /// <summary>
        /// Get or Set total work experience
        /// </summary>
        public int CareTakerWorkExperience1 { get; set; }

        /// <summary>
        /// Get or Set previous company
        /// </summary>
        public string CareTakerPreviousCompany { get; set; }

        /// <summary>
        /// Get or Set total work experience 
        /// </summary>
        public int CareTakerWorkExperience2 { get; set; }

        /// <summary>
        /// Get or Set key skills
        /// </summary>
        public string CareTakerKeySkills { get; set; }

        /// <summary>
        /// Get or Set service id
        /// </summary>
        public int CareTakerServiceId { get; set; }

        /// <summary>
        /// Get or Set salary per hour
        /// </summary>
        public int CareTakerSalaryPerHour { get; set; }

        /// <summary>
        /// Get or Set service description
        /// </summary>
        public string CareTakerServiceDescription { get; set; }

        #endregion
    }
}