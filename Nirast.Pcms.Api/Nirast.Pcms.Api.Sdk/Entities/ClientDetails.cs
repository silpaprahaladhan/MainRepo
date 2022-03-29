using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class ClientDetails
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string EmployeeNumber { get; set; }
        public Nullable<int> CountryId1 { get; set; }
        public string Country1 { get; set; }
        public Nullable<int> StateId1 { get; set; }
        public string State1 { get; set; }
        public Nullable<int> CityId1 { get; set; }
        public string City1 { get; set; }

        public Nullable<int> BranchId1 { get; set; }
        public string Branch1 { get; set; }
        public Nullable<int> BranchId2 { get; set; }
        public string Branch2 { get; set; }


        public string PhoneNo1 { get; set; }
        public string Address2 { get; set; }
        public Nullable<int> CountryId2 { get; set; }
        public Nullable<int> StateId2 { get; set; }
        public Nullable<int> CityId2 { get; set; }
        public string City2 { get; set; }
        public string State2 { get; set; }
        public string Country2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string InvoiceAddress { get; set; }
        public string SecondaryPhoneNo1 { get; set; }
        public string SecondaryPhoneNo2 { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public int ShiftId { get; set; }
        public string WebsiteAddress { get; set; }
        public int ClientStatus { get; set; }
        public int ClientEmailStatus { get; set; }
        public List<ClientAddressModel> AddressList { get; set; }
        public List<ClientShiftDetails> ClientShiftList { get; set; }
        public List<ClientCaretakers> ClientCaretakers { get; set; }
        public List<ClientCategoryRate> CategoryRates { get; set; }

        public List<ClientCategoryPayRiseRate> CategoryPayRiseRates { get; set; }
        public List<ClientCaretakerMap> ClientCaretakerMaps { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }

        public bool HasMidnightCut { get; set; }
        public string InvoicePrefix { get; set; }
        public int InvoiceNumber { get; set; }

        public string SiteURL { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public string Commission { get; set; }
        public string franchise { get; set; }

    }

    public class ClientAddressModel
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

    public class ClientCaretakers
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

  
    public class WorkShiftPayRates
    {
        public int ClientId { get; set; }
        public int CaretakerId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public List<WorkShiftRates> workShiftRates { get; set; }
        public string CareTakerName { get; set; }
        public string CategoryName { get; set; }

    }
    public class WorkShiftRates
    {
        public int WorkShiftId { get; set; }
        public string WorkShiftName { get; set; }
        public float Rate { get; set; }
        public int ClientId { get; set; }
        public int CaretakerId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public int MapStatus { get; set; }
    }
    public class ClientCategoryPayRiseRate
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        
        public float Rate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        //public int ClientId { get; set; }
        public int InvoicePayriseId { get; set; }
        
    }

    public class PayriseData
    {
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public int CaretakerId { get; set; }
    }

    public class ClientInvoicePayriseData
    {
        public int ClientId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public List<ClientCategoryRate> InvoiceRates { get; set; }
    }

    public class ClientCaretakerMap : WorkShiftRates
    {
        public int CaretakerId { get; set; }
        public string CareTakerName { get; set; }
        public string CategoryName { get; set; }
    }

    public class RejectedCaretaker
    {
        public int ClientId { get; set; }
        public int CareTakerId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }

        public string ClientName { get; set; }
        public string CaretakerName { get; set; }
        public int Workshift { get; set; }
        public string WorkshiftName { get; set; }
        public DateTime ScheduleDate { get; set; }

    }
    public class ClientShiftDetails
    {
        public int ClientId { get; set; }
        public int TimeShiftId { get; set; }
        public string TimeShiftName { get; set; }
    }

    public class ClientCategoryRate
    {

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public float Rate { get; set; }
        public bool IsTaxApplicable { get; set; }
        public int ClientId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
    }



    public class ScheduledData : ClientAddressModel
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
        public System.DateTime End { get; set; }
        public string Description { get; set; }

        public int CareTaker { get; set; }
        public string CareTakerName { get; set; }

        public string WorkTimeDetails { get; set; }
        public string WorkShifDetails { get; set; }
        public string ContactPerson { get; set; }

        public bool CustomTiming { get; set; }
        public int ClientEmailStatus { get; set; }
     
        public string FromTime { get; set; }
        public string EndTime { get; set; }


        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
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
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string MessageContent { get; set; }

        public int BranchID { get; set; }
    }
    public class TempScheduledData : ClientAddressModel
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


        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
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
        public DateTime UpdatedDate { get; set; }
        public string MessageContent { get; set; }
    }
    public class SchedulingDate
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
    }

    public class CalenderEventInput
    {
        public int ScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CaretakerId { get; set; }
    }
}
