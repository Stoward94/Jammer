using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Feedback
{
    public class SessionFeedbackViewModel
    {
        public SessionFeedbackViewModel()
        {
            Providers = new List<FeedbackProviderViewModel>();
        }

        public Guid SessionId { get; set; }

        public bool CanSubmit { get; set; }

        public bool SessionCompleted { get; set; }

        public List<FeedbackProviderViewModel> Providers { get; set; }
    }

    public class FeedbackProviderViewModel
    {
        public string Provider { get; set; }
        
        public List<UserFeedbackViewModel> Feedback { get; set; }
    }

    public class UserFeedbackViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        public string UserThumbnail { get; set; }

        public string Kudos { get; set; }

        [Required, Range(1, 10, ErrorMessage = "Please provide a star rating between {1} and {2}")]
        public int Rating { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm | dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime Submitted { get; set; }
    }
}
