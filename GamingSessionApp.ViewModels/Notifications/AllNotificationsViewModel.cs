using System.Collections.Generic;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.ViewModels.Notifications
{
    public class AllNotificationsViewModel
    {

        public string ShowingText { get; set; }

        public Pagination Pagination { get; set; }

        public List<UserNotificationViewModel> Notifications { get; set; }
    }
}
