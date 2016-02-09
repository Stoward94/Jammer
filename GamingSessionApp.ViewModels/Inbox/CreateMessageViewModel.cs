using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class CreateMessageViewModel
    {
        [Required]
        public List<Guid> RecipientIds { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Your can be a maximum of {0} characters")]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
