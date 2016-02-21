using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Feedback
{
    public class UserFeedback
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public int Rating { get; set; }

        public string Comments { get; set; }
    }
}
