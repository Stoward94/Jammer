using System;
using System.ComponentModel.DataAnnotations;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class ViewMessageViewModel
    {
        public string ImageUrl { get; set; }

        public string SenderName { get; set; }

        public string Kudos { get; set; }

        public DateTime SentDate { get; set; }
        public string SentDisplayDate { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "You need to type your reply first")] //Required for the reply message
        public string Body { get; set; }

        public bool Read { get; set; }

        public bool CanReply { get; set; }
    }
}
