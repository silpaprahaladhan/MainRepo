using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nirast.Pcms.Web.Models
{
    public class Constants
    {
        public const string NoViewPrivilege = "You are not authorized to view this page.";

        public const string NoActionPrivilege = "You are not authorized to perform this action.";
        
        public const string NotLoggedIn = "Your session expired. Please login to continue.";
        public const string PublicUserNotLoggedIn = "You Need to Login to Continue Booking";
    }
}
