using System;
namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class Holidays
    {
        public int HolidayId { get; set; }
        public string HolidayName { get; set; }
        public DateTime? HolidayDate { get; set; }
        public float HolidayPayTimes { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int HolidayYear { get; set; }
    }
}
