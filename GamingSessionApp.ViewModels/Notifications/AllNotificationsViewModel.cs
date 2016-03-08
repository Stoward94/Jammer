using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Notifications
{
    public class AllNotificationsViewModel
    {
        public int TotalCount { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public string ShowingText { get; set; }

        public List<UserNotificationViewModel> Notifications { get; set; }
    }
}
