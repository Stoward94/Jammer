using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class OutboxMessageViewModel
    {
        public OutboxMessageViewModel()
        {
            Messages = new List<SentMessageViewModel>();
            Pagination = new Pagination
            {
                PageNo = 1,
                PageSize = 10,
                TotalCount = 0
            };
        }

        public Pagination Pagination { get; set; }

        public List<SentMessageViewModel> Messages { get; set; }
    }

    public class SentMessageViewModel
    {
        public Guid Id { get; set; }

        public string RecipientImageUrl { get; set; }

        [Display(Name = "To")]
        public string RecipientName { get; set; }

        [Display(Name = "Sent")]
        public DateTime SentDate { get; set; }

        public string SentDisplayDate { get; set; }

        public string Subject { get; set; }
    }
}
