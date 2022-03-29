using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Entities
{
   public class UpcomingAppointment
    {

        /// <summary>
        /// Get or Setbooking/scheduling Id
        /// </summary>
        public int AppointmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SiteURL { get; set; }

        /// <summary>
        /// Get or Set Appointment Date
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Get or Set User
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Get or Set the Appointment Time
        /// </summary>
        public string AppointmentTime { get; set; }

        /// <summary>
        /// Get or Set the location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Get or Set the type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Get or Set User
        /// </summary>
        public string CareTaker { get; set; }

        /// <summary>
        /// Get or Set User
        /// </summary>
        public string AdminEmail { get; set; }
    }
}
