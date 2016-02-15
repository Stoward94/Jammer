using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class UserPreferences
    {
        public UserPreferences()
        {
            ReceiveEmail = true;
            ReceiveNotifications = true;
            ReminderTimeId = 6; //60 minutes
        }

        [Key, ForeignKey("Profile")]
        public string ProfileId { get; set; }
        public UserProfile Profile { get; set; }

        public bool ReceiveEmail { get; set; }

        public bool ReceiveNotifications { get; set; }

        //Email reminders time period
        public int ReminderTimeId { get; set; }
        public EmailReminderTime ReminderTime { get; set; }
    }
}
