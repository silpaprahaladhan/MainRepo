using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Enums
    {
        [Flags, Serializable]
        public enum CardType
        {
            MasterCard = 0x0001,
            VISA = 0x0002,
            Amex = 0x0004,
            DinersClub = 0x0008,
            enRoute = 0x0010,
            Discover = 0x0020,
            JCB = 0x0040,
            Unknown = 0x0080,
            All = CardType.Amex | CardType.DinersClub |
                             CardType.Discover | CardType.Discover |
                             CardType.enRoute | CardType.JCB |
                             CardType.MasterCard | CardType.VISA
        }

        public enum Gender : int
        {
            Male = 1,
            Female = 2,
            Others = 3
        }

        public enum DocumentType : int
        {
            Certificate = 1,
            SINFile = 2,
            Others = 3
        }

        public enum CaretakerStatus : int
        {
            Applied = 1,
            Approved = 2,
            Rejected = 3,
        }

        public enum BookingStatus : int
        {
            [Description("Pending Booking Acceptance")]
            PendingBookingAcceptance = 1,
            [Description("Completed")]
            Completed = 2,
            [Description("Rejected")]
            Rejected = 3,
            [Description("Cancelled")]
            Cancelled =4,
            [Description("Pending Payment")]
            PendingPayment,
        }

        public enum UserCardType : int
        {
            Debit = 1,
            Credit = 2,
        }

        public enum UserStatus : int
        {
            Active = 1,
            InActive = 2,
            Deleted = 3
        }

        public enum ClientStatus: int
        {
            Active = 1,
            InActive = 2
        }
        public enum ClientEmailStatus : int
        {
            Active = 1,
            InActive = 0
        }

        public enum EmailType : int
        {
            Registration = 1,
            Booking = 2,
            Scheduling = 3,
            UserPayment = 4,
            Invoice = 5
        }
        public enum AuditLogType : int
        {
             Scheduling = 1
        }
        public enum AuditLogActionType : int
        {
            Add = 1,
            Edit = 2,
            Delete = 3
        }

        public enum MapStatus : int
        {
            Map = 1,
            Unmap = 2
        }

        public enum TestRole : int
        {
            CountryStaff = 1,
            StateStaff = 2,
            CityStaff = 3,
            BranchStaff = 4
        }
    }
}