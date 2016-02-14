using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class OutboxMessageViewModel
    {
        public Guid Id { get; set; }

        public string RecipientImageUrl { get; set; }

        [Display(Name = "To")]
        public string RecipientName { get; set; }

        [Display(Name = "Sent")]
        public DateTime SentDate { get; set; }

        public string Subject { get; set; }
    }
}
