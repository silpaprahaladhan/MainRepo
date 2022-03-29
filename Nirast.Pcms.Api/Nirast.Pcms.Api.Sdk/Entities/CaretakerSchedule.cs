using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
    public class CaretakerSchedule
    {
        public int ScheduleType { get; set; }
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int WorkmodeId { get; set; }
        public string WorkMode { get; set; }
        public int TimeShiftId { get; set; }
        public string TimeShiftName { get; set; }
        public DateTime BookedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BookingLocation { get; set; }
        public string Description { get; set; }
    }
}
