using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using static Nirast.Pcms.Web.Models.Enums;

namespace Nirast.Pcms.Web.Models
{
    public class TermsAndPolicyModel
    {
        public TermsAndPolicyModel() { }
        public TermsAndPolicyModel(HttpPostedFileBase termsnCondition, HttpPostedFileBase privacyPolicy)
        {
            TermsnCondition = termsnCondition;
            PrivacyPolicy = privacyPolicy;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase TermsnCondition { get; set; }
        public HttpPostedFileBase PrivacyPolicy { get; set; }
        public HttpPostedFileBase ConsentForm { get; set; }
    }

    public class LoginModel
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
    }

    public class Years
    {
        public int Id { get; set; }
        public string Year { get; set; }
    }


    public class Months
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public SelectList GetMonths()
        {
            List<Months> months = new List<Months>();
            SelectList MonthList = null;
            Months month = new Months
            {
                Id = 0,
                Month = "--Select Month--"
            };
            months.Add(month);
            for (int i = 1; i <= 12; i++)
            {
                month = new Months
                {
                    Id = i,
                    Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                };
                months.Add(month);
            }
            MonthList = new SelectList(months, "Id", "Month", DateTime.Now.Month);
            return MonthList;
        }
    }

   
    public class LoggedInUser
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public byte[] ProfilePicByte { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public bool IsVerified { get; set; }
        public int UserStatus { get; set; }
        public string EmailAddress { get; set; }
        public string LoginName { get; set; }
        public string Location { get; set; }
        public string ProfilePicPath { get; set; }
        public int CityId { get; set; }
        public int WorkRoleId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int BranchId { get; set; }
        public int EmployeeNumber { get; set; }


        public string cityname { get; set; }
        public string statename { get; set; }
        public string countryname { get; set; }
    }
    public class CountryViewModel
    {
        public int CountryId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(50,ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only alphabets are allowed")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(10,ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string PhoneCode { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(10,ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string Currency { get; set; }

        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public string CurrencySymbol { get; set; }

        public bool Isdefault { get; set; }

    }

    public class StateViewModel
    {
        /// <summary>
        /// Get or Set state id
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Get or Set state code
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        //Get or Set state name 
        [Required(ErrorMessage = "* Required")]
        [StringLength(30, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        //[RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only alphabets are allowed")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Get or Set country id
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "CountryId")]
        public int CountryId { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string PhoneCode { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Get or Set country name
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Get or Set Tax Amount
        /// </summary>
        [Required(ErrorMessage = "* Required")]
        public float TaxPercent { get; set; }
	}

    public class CityViewModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int CityCreatedUserId { get; set; }
        public int CityUpdatedUserId { get; set; }
        public bool CityRecordActive { get; set; }
    }

    public class Privileges
    {
        public int PrivilegeId { get; set; }

        public int RoleId { get; set; }

        public int ModuleId { get; set; }

        public string ModuleName { get; set; }

        public bool AllowView { get; set; }

        public bool AllowEdit { get; set; }

        public bool AllowDelete { get; set; }
    }

    public class RolePrivileges
    {
        public int RoleId { get; set; }

        public List<Privileges> Privileges { get; set; }
    }

    public class WorkShiftViewModel
    {
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Shift")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        public string ShiftName { get; set; }

        public bool ShowResidentName { get; set; }
        public bool IsSeparateInvoice { get; set; }
    }

    public class TimeShiftViewModel
    {
        public int TimeShiftId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Time shift")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        public string TimeShiftName { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Working Hours")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public float WorkingHours { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Paying Hours")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        public float PayingHours { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Start Time")]
        [RegularExpression(@"^\b((1[0-2]|0?[1-9]):([0-5][0-9]) ([AaPp][Mm]))$", ErrorMessage = "Invalid time")]
        
        public string StartTime { get; set; }

        public float IntervalHours { get; set; }
    }

    public class HolidayViewModel
    {
        public int HolidayId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(50, ErrorMessage = "Maximum {1} characters allowed")]
        [RegularExpression(@"(?!^ +$)^.+$", ErrorMessage = " Blank Spaces are not allowed")]
        [Display(Name = "Holiday")]
        public string HolidayName { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Column(TypeName = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? HolidayDate { get; set; }

        public float HolidayPayTimes { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int HolidayYear { get; set; }
    }

    public class EmailInput
    {
        public int UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
        public string EmailId
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }
        public string Attachments
        {
            get;
            set;
        }
        public Attachment Attachment
        {
            get;
            set;
        }

        public EmailType EmailType
        {
            get;
            set;
        }
        public EmailConfiguration EmailConfig
        {
            get;
            set;
        }

        public EmailTypeConfiguration EmailIdConfig
        {
            get;
            set;
        }
    }

}