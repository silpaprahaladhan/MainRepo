using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CareTakerRegistrationModel : UsersDetails
    {
        #region public properties

        /// <summary>
        /// Get or Set registration id
        /// </summary>
        public int CaretakerDetailId { get; set; }

        /// <summary>
        /// Get or Set profile id
        /// </summary>
        public string CaretakerProfileId { get; set; }

        /// <summary>
        /// Get or Set user id
        /// </summary>
        public int UserId { get; set; }      

        /// <summary>
        /// Get or Set category id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Get or Set category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Get or Set category name
        /// </summary>
        public string SSID { get; set; }


        /// <summary>
        /// Get or Set the description about care taker
        /// </summary>
        public string AboutMe { get; set; }

        /// <summary>
        /// Gets or sets the account status.
        /// </summary>
        /// <value>
        /// The account status.
        /// </value>
        public int AccountStatus { get; set; }        

        /// <summary>
        /// Get or Set key skills
        /// </summary>
        public string KeySkills { get; set; }

        /// <summary>
        /// Gets or sets the total experience.
        /// </summary>
        /// <value>
        /// The total experience.
        /// </value>
        public float TotalExperience { get; set; }

        /// <summary>
        /// Gets or sets the full name of the care taker.
        /// </summary>
        /// <value>
        /// The full name of the care taker.
        /// </value>
        public string CareTakerFullName { get; set; }

        public List<HttpPostedFileBase> CertificateFiles { get; set; }
        public List<HttpPostedFileBase> SINFile { get; set; }
        public List<HttpPostedFileBase> OtherDocuments { get; set; }


        public byte[] ConsentForm { get; set; }
        /// <summary>
        /// Get or set document lists of Caretaker
        /// </summary>
        public List<DocumentsList> CareTakerDocuments = new List<DocumentsList>();
        public List<CareTakerExperiences> CareTakerExperiences = new List<CareTakerExperiences>();
        public List<CareTakerQualifications> CareTakerQualifications = new List<CareTakerQualifications>();
        public List<CareTakerServices> CareTakerServices = new List<CareTakerServices>();
        public List<CareTakerClients> CareTakerClientsRates = new List<CareTakerClients>();

        /// <summary>
        /// Gets or sets a value indicating whether [send through fax].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send through fax]; otherwise, <c>false</c>.
        /// </value>
        public bool SendThroughFax { get; set; }

        /// <summary>
        /// Gets or sets the siteURL.
        /// </summary>
        public string SiteURL { get; set; }

        //For Adnace Search
        public float DisplayRate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public bool IsPrivate { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
        public string ConsentDocPath { get; set; }
        public string EmployeeNumber { get; set; }
        public Nullable<int> branchId1 { get; set; }

        #endregion
    }

    public class CareTakerExperiences
    {
        /// <summary>
        /// Get or Set Experience Date From
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Get or Set Experience Date To
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Get or Set Experience Company
        /// </summary>
        public string Company { get; set; }
    }

    public class CareTakerQualifications
    {
        /// <summary>
        /// Get or Set qualification id
        /// </summary>
        public int QualificationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CareTakerQualificationId { get; set; }

        /// <summary>
        /// Get or Set qualification
        /// </summary>
        public string QualificationName { get; set; }
    }

    public class ServicePayRates
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public float PayRiseRate { get; set; }
        public float DisplayRiseRate { get; set; }
    }
    public class CareTakerServices
    {
        public int ServiceId { get; set; }

        /// <summary>
        /// Get or Set Service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Get or Set Expected Service Rate
        /// </summary>
        public float PayingRate { get; set; }

        /// <summary>
        /// Get or Set Approved Service Rate
        /// </summary>
        public float DisplayRate { get; set; }
        public int CareTakerId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
    }

    public class CaretakerType
    {
        public string TypeName { get; set; }
        public string Color { get; set; }
    }

    public class CareTakerClients
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public int ClientRateId { get; set; }

        /// <summary>
        /// Get or Set Client id
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Get or Set Client id
        /// </summary>
        public string ClientFirstName { get; set; }

        /// <summary>
        /// Get or Set Client id
        /// </summary>
        public string ClientLastName { get; set; }

        /// <summary>
        /// Get or Set Expected Service Rate
        /// </summary>
        public float PayingRate { get; set; }

        /// <summary>
        /// Get or Set Approved Service Rate
        /// </summary>
        public float DisplayRate { get; set; }
    }

    public class DocumentsList
    {
        public int CaretakerDocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string ContentType { get; set; }
        public byte[] DocumentContent { get; set; }
        public string DocumentPath { get; set; }
    }

    public enum eDocumentTypes : int
    {
        Certificate = 1,
        SINFile = 2,
        Others = 3
    }

    public class CareTakers
    {
        public string CareTakerName { get; set; }
        public int CaretakerId { get; set; }
        public string ProfileId { get; set; }

        public string Phone1 { get; set; }

        public string TypeName { get; set; }
        public int RejectedId { get; set; }
        public string rejectedstatus { get; set; }
    }

    public class ApproveCaretaker
    {
        public int CareTakerId { get; set; }

        public bool IsPrivate { get; set; }
        public List<CareTakerServices> ApprovedServiceRates { get; set; }

        public List<ServicePayRates> PayRiseServiceRates { get; set; }

        public string SiteURL { get; set; }
    }

    public class RejectCareTaker
    {
        public int Userid { get; set; }

        public string SiteURL { get; set; }
    }

    public class SearchedCareTakers
    {
        public int CaretakerId { get; set; }

        public string CareTakerFirstName { get; set; }

        public string CareTakerLastName { get; set; }
        public string Fullname { get; set; }

        public int ServiceId { get; set; }

        public string ServiceName { get; set; }
        public string Location { get; set; }
        public string State { get; set; }


        public float TotalExperience { get; set; }

        public float DisplayRate { get; set; }

        public byte[] ProfilePicture { get; set; }

        public string Currency { get; set; }

        public string CurrencySymbol { get; set; }

        public string Category { get; set; }


        public string ProfilePicPath { get; set; }
    }

    public class CaretakerAvailableReport
    {
        public int ClientId { get; set; }
        public int CaretakerUserId { get; set; }

        public string   ClientName { get; set; }
        public string CareTakerName { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime Dates { get; set; }
        public string TotalHours { get; set; }
        public string Timeshifts { get; set; }
        public string Time { get; set; }
        public string StartTime { get; set; }
        public string Description { get; set; }

        public string branch { get; set; }
        public string Commission { get; set; }
        public string Total { get; set; }
    }
}
