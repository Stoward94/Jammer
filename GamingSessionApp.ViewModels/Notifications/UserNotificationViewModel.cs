using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Notifications
{
    public class UserNotificationViewModel
    {
        public Guid Id { get; set; }

        public string SenderThumbnailUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DisplayDate { get; set; }

        public string Body { get; set; }
        
        public int TypeId { get; set; }
        
        public Guid SessionId { get; set; }

        //If the notification is referring to a comment on a session
        public int? CommentId { get; set; }

        public bool Read { get; set; }
    }
}
