using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class ViewMessageViewModel
    {
        public string ImageUrl { get; set; }

        public string SenderName { get; set; }

        public DateTime SentDate { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
