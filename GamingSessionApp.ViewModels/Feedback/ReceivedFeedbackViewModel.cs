using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Feedback
{
    public class ReceivedFeedbackViewModel
    {
        public Guid SessionId { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string Type { get; set; }

        public List<UserFeedbackViewModel> UserFeedback { get; set; }
    }
}
