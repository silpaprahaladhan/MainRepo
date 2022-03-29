using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class UserStatusViewModel
    {
        public int BookingId { get; set; }

        public int BookedCaretakerId { get; set; }

        public string BookedCaretakerProfileId { get; set; }

        public int BookedUserId { get; set; }
        
        public DateTime ServiceBookingDate { get; set; }

        public DateTime BookingStatusChangedDate { get; set; }

        public int BookingStatusId { get; set; }

        public string BookingStatusReason { get; set; }

        public bool BookingStatusRecordActive { get; set; }
        public string StatusChangedDate { get; set; }
    }

    public class UserStatusListViewModel : UserStatusViewModel
    {
        public string BookedCaretakerName { get; set; }

        public string BookedUserName { get; set; }
        
        public string BookedService { get; set; }

        public string BookingStatus { get; set; }

        public string UserLocation { get; set; }

        public string CaretakerLocation { get; set; }

        public float ServiceRate { get; set; }
    }
}