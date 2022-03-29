using System;
namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class ClientTimeShifts
    {
        public int TimeShiftId { get; set; }

        public string TimeShiftName { get; set; }

        public float WorkingHours { get; set; }

        public float PayingHours{ get; set; }

        public DateTime StartTime { get; set; }
        public float IntervalHours { get; set; }

    }
}
