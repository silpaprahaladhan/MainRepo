using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Nirast.Pcms.Web.Helpers
{
    public class NotificationHub : Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        public void NotifyAll(string title, string message, string alertType)
        {
            Clients.All.displayNotification(title, message, alertType);
        }

        // Call this from C#: NewsFeedHub.Static_Send(channel, content)
        public static void Static_Send(string title, string message, string alertType)
        {
            //Clients.All.displayNotification(title, message, alertType);
            // hubContext.Clients.Group(channel).addMessage(content);
            hubContext.Clients.All.displayNotification(title, message, alertType);
        }
    }
}