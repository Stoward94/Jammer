using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.Models;

namespace GamingSessionApp.ViewModels.Profile
{
    public class UserMenuViewModel
    {
        public int KudosPoints { get; set; }

        public int UnseenNotifications { get; set; }
        //public List<UserNotification> Notifications { get; set; }
    }
}
