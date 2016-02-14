using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class CreateMessageViewModel
    {
        [Required]
        public string Recipients { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Your subject can be a maximum of {1} characters")]
        public string Subject { get; set; }

        [Required, AllowHtml]
        public string Body { get; set; }
    }
}
