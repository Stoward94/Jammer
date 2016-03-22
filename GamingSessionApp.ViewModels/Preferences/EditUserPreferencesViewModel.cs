using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.Preferences
{
    public class EditUserPreferencesViewModel
    {
        [Display(Name = "Receive Emails")]
        public bool ReceiveEmail { get; set; }

        [Display(Name = "Receive Notifications")]
        public bool ReceiveNotifications { get; set; }

        
        [Display(Name = "Session Reminder Time")]
        public int ReminderTimeId { get; set; }

        public SelectList ReminderTimes { get; set; }
    }
}
