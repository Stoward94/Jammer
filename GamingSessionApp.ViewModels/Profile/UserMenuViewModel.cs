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
        public string ThumbnailUrl { get; set; }

        public string KudosPoints { get; set; }

        public int UnreadMessages { get; set; }

        public int UnseenNotifications { get; set; }
    }
}
