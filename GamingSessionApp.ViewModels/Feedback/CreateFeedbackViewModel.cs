using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.Feedback
{
    public class CreateFeedbackViewModel
    {
        public CreateFeedbackViewModel()
        {
            CanSubmitFeedback = false;
            UsersFeeback = new List<UserFeedbackViewModel>();
        }

        [Required, HiddenInput]
        public Guid SessionId { get; set; }

        public bool CanSubmitFeedback { get; set; }

        public string SessionEndDate { get; set; }
        public string SessionEndTime { get; set; }

        [Required]
        public List<UserFeedbackViewModel> UsersFeeback { get; set; }
    }
}
