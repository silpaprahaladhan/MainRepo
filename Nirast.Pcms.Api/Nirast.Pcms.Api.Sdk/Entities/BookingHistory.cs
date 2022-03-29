using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class BookingHistory
    {
        /// <summary>
        /// Get or Set the booking id
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Get or Set the booking date
        /// </summary>
        public DateTime BookingDate { get; set; }
        public DateTime StatusModifiedDate { get; set; }

        /// <summary>
        /// Get or Set the user name
        /// </summary>
        public string BookedUser { get; set; }

        /// <summary>
        /// Get or Set user location
        /// </summary>
        public string UserLocation { get; set; }

        /// <summary>
        /// Get or Set Care taker name
        /// </summary>
        public string Caretaker { get; set; }
        public int CaretakerId { get; set; }

        /// <summary>
        /// Get or Set the care taker location
        /// </summary>
        public string CaretakerLocation { get; set; }

        /// <summary>
        /// Get or Set the service name
        /// </summary>
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        
        /// <summary>
        /// Get or Set caretaker rate per hour
        /// </summary>
        public int ServiceRate { get; set; }

        public string CurrencySymbol { get; set; }

        public string Currency { get; set; }

        public string CareRecipient { get; set; }

        /// <summary>
        /// Get or Set BookedStatus
        /// </summary>
        public int BookingStatus { get; set; }
        public int InvoiceNo { get; set; }

        /// <summary>
        /// Get or Set InvoicePath
        /// </summary>
        public string InvoicePath { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoicePrefix { get; set; }

    }

    public class BookingHistoryDetail : BookingHistory
    {

        /// <summary>
        /// Get or Set from date
        /// </summary>
        public DateTime BookingStartTime { get; set; }

        /// <summary>
        /// Get or Set to date
        /// </summary>
        public DateTime BookingEndTime { get; set; }

        /// <summary>
        /// Get or Set the Details
        /// </summary>
        public string BookingPurpose { get; set; }

        public string PhoneNo1{ get; set; }

        public string EmailAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country{ get; set; }

        public string ZipCode{ get; set; }

        public float TotalPayingAmount { get; set; }

        public float TotalDisplayAmount { get; set; }
        public float DisplayRate { get; set; }
        public float TotalHours { get; set; }
        public float Tax { get; set; }

    }

    public class CaretakerBookingReport
    {
        public string Caretaker { get; set; }
        //public string Type { get; set; }
        public string BookingDate { get; set; }
        public string BookedBy { get; set; }
        public string Service { get; set; }
        public string UserLocation { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public float TotalHours { get; set; }

        public string FromDateTime { get; set; }
        public string ToDateTime { get; set; }
    }

    public class BookingHistorySearch
    {
        public int? PublicUserId { get; set; }
        public int? InvoiceNumber { get; set; }
        public int? InvoiceSearchInputId { get; set; }
        

        public string Caretaker { get; set; }
        public int? ServiceId { get; set; }
        public int? StatusId { get; set; }
        public int? DateSearchType { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class BookingSearchData
    {
        public int CaretakerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ScheduleDeleteData
    {
        public int ScheduleId { get; set; }
        public int ClientId { get; set; }
        public string SiteURL { get; set; }
        public int UserId { get; set; }
        public AuditLogType AuditLogType { get; set; }
        public AuditLogActionType AuditLogActionType { get; set; }
        public string OldData { get; set; }
        public string CareTakerName { get; set; }
    }


    public class CaretakerWiseSearchReport
    {

        public string CareTaker { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public int ServiceId { get; set; }

        public int? SearchType { get; set; }
    }

    public class CaretakerScheduleListSearch
    {
        public int CaretakerId { get; set; }
        public int? ClientId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }

    public class CaretakerScheduleList
    {
        public string ClientName { get; set; }
        public string WorkShiftName { get; set; }
        public string TimeShiftName { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public DateTime StartDateTime { get; set; }
    }
    public class CaretakerBookingReportModel
    {
        public int? CategoryId { get; set; }

        public int? ServiceId { get; set; }

        public int? CaretakerName { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }
    }
    public class UserBookingInvoiceReport
    {
        public int BookingId { get; set; }
        public string UserName { get; set; }
        public string CareRecipent { get; set; }
        public string UserLocation { get; set; }
        public string UserEmail { get; set; }
        public string CaretakerLocation { get; set; }
        public string HouseName { get; set; }
        public int? ServiceId { get; set; }
        public int? CaretakerId { get; set; }
        public string Service { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string FromTime { get; set; }
        public string EndTime { get; set; }
        public float TaxAmount { get; set; }
        public float EffTaxAmount { get; set; }
        public float ServiceRate { get; set; }
        public float DisplayRate { get; set; }
        public float PayingAmount { get; set; }
        public float DisplayAmount { get; set; }
        public string CareTakerName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoicePrefix { get; set; }
        public DateTime BookingDateTime { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string ZipCode { get; set; }
        public string Phone1 { get; set; }
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public float HolidayHours { get; set; }
        public float HolidayAmount { get; set; }
        public float Amount { get; set; }
        public float EffAmount { get; set; }
        public float TotalPayingAmount { get; set; }
        public float EffTotalPayingAmount { get; set; }
        public float TotalHours { get; set; }
        public float EFFTotalHours { get; set; }
        public int BookingStatus { get; set; }
        public float HST { get; set; }
        public string BookingPurpose { get; set; }
    }
    public class UserInvoiceParams
    {
        public string UserEmail { get; set; }
        public int InvoiceSearchInputId { get; set; }
        public int UserId { get; set; }

        public string UserName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string InvoicePath { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoicePrefix { get; set; }
        public int? Year { get; set; }

        public int? Month { get; set; }
        public int? Mode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}