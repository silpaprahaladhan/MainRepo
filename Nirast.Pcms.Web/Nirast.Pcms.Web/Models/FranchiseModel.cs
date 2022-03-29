using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Web.Models.Enums;

namespace Nirast.Pcms.Web.Models
{
    public class FranchiseModel
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(100, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = " Alphanumeric values with first character as an alphabet is only allowed")]
        public string ClientName { get; set; }
        public string EmployeeNumber { get; set; }
        [Required(ErrorMessage = "* Required")]
        //[RegularExpression(@"^[a-zA-Z].*[a-zA-Z0-9]+$", ErrorMessage = "First and Last letter should be an alphabet")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a Country")]
        public Nullable<int> CountryId1 { get; set; }

        public string Country1 { get; set; }

        public string State1 { get; set; }

        public string City1 { get; set; }
        public string InvoiceAddress { get; set; }
        public string Commission { get; set; }

        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a State")]

        public Nullable<int> StateId1 { get; set; }
        [Required(ErrorMessage = "* Required")]
        // [Required(ErrorMessage = "Please select a City")]
        public Nullable<int> CityId1 { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        public string PhoneNo1 { get; set; }

        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"[0-9a-zA-Z #,-,/]+", ErrorMessage = "Invalid Address")]
        public string Address2 { get; set; }

        public Nullable<int> CountryId2 { get; set; }

        public Nullable<int> StateId2 { get; set; }

        public string Country2 { get; set; }

        public string State2 { get; set; }

        public Nullable<int> CityId2 { get; set; }

        public string City2 { get; set; }

        //[Required(ErrorMessage = "Please provide PhoneNo")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        public string PhoneNo2 { get; set; }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string EmailId { get; set; }

        //[Required(ErrorMessage = "Please provide WebsiteAddress")]
        [RegularExpression(@"(([\w]+:)?\/\/)?(([\d\w]|%[a-fA-f\d]{2,2})+(:([\d\w]|%[a-fA-f\d]{2,2})+)?@)?([\d\w][-\d\w]{0,253}[\d\w]\.)+[\w]{2,4}(:[\d]+)?(\/([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)*(\?(&?([-+_~.\d\w]|%[a-fA-f\d]{2,2})=?)*)?(#([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)?$", ErrorMessage = "Website is not valid")]
        public string WebsiteAddress { get; set; }

        public Enums.ClientStatus ClientStatus { get; set; }
        public Enums.ClientEmailStatus ClientEmailStatus { get; set; }

        public List<ClientCategoryRate> CategoryRates { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public List<ClientCategoryRate> CategoryPayRiseRates { get; set; }
        public List<ClientCaretakers> ClientCaretakers { get; set; }
        public List<ClientShiftDetails> ClientShiftList { get; set; }
        public List<ClientCaretakerMap> ClientCaretakerMaps { get; set; }

        // public List<CareTakerRegistrationViewModel> CareTakers { get; set; }
        //public List<CareTakerRegistrationViewModel> RegistredCaretakers { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        public string SecondaryPhoneNo1 { get; set; }
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(500, ErrorMessage = "Maximum {1} characters allowed")]
        public string SecondaryPhoneNo2 { get; set; }

        public bool HasMidnightCut { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(5, ErrorMessage = "Maximum {1} characters allowed")]
        public string InvoicePrefix { get; set; }

        [Required(ErrorMessage = "* Required")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Range(0, int.MaxValue, ErrorMessage = "Exceeded the maximum allowed value")]
        public int InvoiceNumber { get; set; }

        public string SiteURL { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and maximum {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be minimum 8 characters including 1 uppercase , one special character and alphanumeric characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Get or Set confirm password
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string franchise { get; set; }
    }

    public class FranchiseAddressModel
    {
        public int AddressId { get; set; }
        public string BuildingName { get; set; }
        public string Location { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> StateId { get; set; }
        public int CountryId { get; set; }
        public string Zipcode { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
    }

    public class FranchiseScheduling : ClientModel
    {
        public int CaretakerId { get; set; }

        public string CaretakerName { get; set; }

        public List<string> ServicesByCaretaker { get; set; }

        public string CaretakerCategory { get; set; }
    }


    [Serializable]
    public class FranchiseCategoryRate
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public float Rate { get; set; }
        public bool IsTaxApplicable { get; set; }
        public int ClientId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
    }


    public class FranchiseDetailModel : CaretakerScheduling
    {
        public string CareShift { get; set; }

        public string WorkShift { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string TimeIn { get; set; }

        public string TimeOut { get; set; }
    }
    [Serializable]
    public class FranchiseShiftDetails
    {
        public int ClientId { get; set; }
        public int TimeShiftId { get; set; }
        public string TimeShiftName { get; set; }
    }
    [Serializable]
    public class FranchiseCaretakers
    {
        public int ClientId { get; set; }
        public int CaretakerId { get; set; }
        public int CategoryTypeId { get; set; }
        public float Rate { get; set; }
        public List<WorkShiftRates> MapRates { get; set; }
        public string CareTakerName { get; set; }
        public string CategoryName { get; set; }
        public string CaretakerProfileId { get; set; }
        public string CareTakerCountryName { get; set; }
        public string TotalExperience { get; set; }
    }

    public class FranchiseWorkShiftPayRates
    {
        public int ClientId { get; set; }
        public int CaretakerId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public List<WorkShiftRates> WorkShiftRates { get; set; }
        public string CareTakerName { get; set; }
        public string CategoryName { get; set; }
    }
    public class FranchiseWorkShiftRates
    {
        public int WorkShiftId { get; set; }
        public string WorkShiftName { get; set; }
        public float Rate { get; set; }
        public int ClientId { get; set; }
        public int CaretakerId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public int MapStatus { get; set; }
    }
    public class FranchiseInvoicePayriseData
    {
        public int ClientId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public List<ClientCategoryRate> InvoiceRates { get; set; }
    }

    public class FranchiseData
    {
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public int CaretakerId { get; set; }
    }

    public class FranchiseCaretakerMap : FranchiseWorkShiftRates
    {
        public int CaretakerId { get; set; }
        public string CareTakerName { get; set; }
        public string CategoryName { get; set; }
    }

    public class FranchiseCaretaker
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string CaretakerName { get; set; }
        public int CareTakerId { get; set; }

        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public int Workshift { get; set; }
        public string WorkshiftName { get; set; }

        public DateTime ScheduleDate { get; set; }
    }



    public class FranchiseScheduledData : FranchiseAddressModel
    {
        public int Id { get; set; }
        public int SchedulingId { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoicePrefix { get; set; }
        public string InvoiceAddress { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int CareTakerType { get; set; }
        public string CareTakerTypeName { get; set; }

        public string ServiceTypeName { get; set; }
        public int WorkMode { get; set; }
        public string WorkModeName { get; set; }

        public int? WorkTime { get; set; }
        public string WorkTimeName { get; set; }

        public string ThemeColor { get; set; }

        public System.DateTime Start { get; set; }
        public Nullable<System.DateTime> End { get; set; }
        public string Description { get; set; }

        public int CareTaker { get; set; }
        public string CareTakerName { get; set; }

        public string WorkTimeDetails { get; set; }
        public string WorkShifDetails { get; set; }
        public string ContactPerson { get; set; }

        public bool CustomTiming { get; set; }
        public string FromTime { get; set; }
        public string EndTime { get; set; }


        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Hours { get; set; }
        public string Rate { get; set; }
        public string TypeRate { get; set; }
        public string Amount { get; set; }
        public List<SchedulingDate> ClientSchedulingDate { get; set; }


        public decimal HoildayHours { get; set; }
        public decimal HoildayAmout { get; set; }
        public decimal Total { get; set; }
        public decimal HolidayPayValue { get; set; }

        public decimal HST { get; set; }
        public string CurrencySymbol { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public bool IsSeparateInvoice { get; set; }

        public string SiteURL { get; set; }

        public double HoursInHoliday { get; set; }
        public int UserId { get; set; }
        public AuditLogType AuditLogType { get; set; }
        public AuditLogActionType AuditLogActionType { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int LogId { get; set; }
        public string MessageContent { get; set; }
    }

    public class FranchiseScheduledDataTable
    {
        public int AddressId { get; set; }
        public string BuildingName { get; set; }
        public string Location { get; set; }

        public int CountryId { get; set; }
        public string Zipcode { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int CareTakerType { get; set; }
        public string CareTakerTypeName { get; set; }

        public string ServiceTypeName { get; set; }
        public int WorkMode { get; set; }
        public string WorkModeName { get; set; }


        public string WorkTimeName { get; set; }

        public string ThemeColor { get; set; }

        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public string Description { get; set; }

        public int CareTaker { get; set; }
        public string CareTakerName { get; set; }

        public string WorkTimeDetails { get; set; }
        public string WorkShifDetails { get; set; }
        public string ContactPerson { get; set; }

        public bool CustomTiming { get; set; }

        public string FromTime { get; set; }
        public string EndTime { get; set; }


        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Hours { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }




        public decimal HoildayHours { get; set; }
        public decimal HoildayAmout { get; set; }
        public decimal Total { get; set; }

        public decimal HST { get; set; }
        public char CurrencySymbol { get; set; }
        public DateTime StartDateTime { get; set; }

        public bool IsSeparateInvoice { get; set; }

    }

    public class FranchiseSchedulingDate
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
    }

    public class FrancgiseScheduleDeleteData
    {
        public int ScheduleId { get; set; }
        public int ClientId { get; set; }
        public string SiteURL { get; set; }
        public int UserId { get; set; }
        public AuditLogType AuditLogType { get; set; }
        public AuditLogActionType AuditLogActionType { get; set; }
    }

    public class FranchiseCalenderEventInput
    {
        public int ScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CaretakerId { get; set; }
        public int BookingId { get; set; }
    }
    public class FranchiseNewScheduleData : FranchiseAddressModel
    {
        public int Id { get; set; }
        public int SchedulingId { get; set; }
        public int InvoiceNumber { get; set; }

        public string InvoicePrefix { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int CareTakerType { get; set; }
        public string CareTakerTypeName { get; set; }

        public string ServiceTypeName { get; set; }
        public int WorkMode { get; set; }
        public string WorkModeName { get; set; }
        public int? WorkTime { get; set; }
        public string WorkTimeName { get; set; }

        public string ThemeColor { get; set; }

        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public string Description { get; set; }

        public int CareTaker { get; set; }
        public string CareTakerName { get; set; }

        public string WorkTimeDetails { get; set; }
        public string WorkShifDetails { get; set; }
        public string ContactPerson { get; set; }

        public bool CustomTiming { get; set; }

        public string FromTime { get; set; }
        public string EndTime { get; set; }


        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public float Hours { get; set; }
        public string Rate { get; set; }
        public string TypeRate { get; set; }
        public string Amount { get; set; }
        public List<SchedulingDate> ClientSchedulingDate { get; set; }

        public decimal HoildayHours { get; set; }
        public decimal HoildayAmout { get; set; }
        public decimal Total { get; set; }
        public decimal HST { get; set; }

        public decimal HolidayPayValue { get; set; }

        public string CurrencySymbol { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsSeparateInvoice { get; set; }
        public string SiteURL { get; set; }

        public double HoursInHoliday { get; set; }

        public int UserId { get; set; }
        public int LogId { get; set; }
        public int AuditLogType { get; set; }
        public int AuditLogActionType { get; set; }
        [JsonIgnore]
        public string OldData { get; set; }
        [JsonIgnore]
        public string NewData { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UpdatedDate { get; set; }
        public string MessageContent { get; set; }
    }


}