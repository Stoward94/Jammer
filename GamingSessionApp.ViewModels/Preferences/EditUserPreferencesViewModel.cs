using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.Preferences
{
    public class EditUserPreferencesViewModel
    {
        public bool ReceiveEmail { get; set; }

        public bool ReceiveNotifications { get; set; }

        
        public int ReminderTimeId { get; set; }

        public SelectList ReminderTimes { get; set; }
    }
}
