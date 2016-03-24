using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.ViewModels.Inbox
{
    public class UserMessagesViewModel
    {
        public UserMessagesViewModel()
        {
            Messages = new List<MessageViewModel>();
            Pagination = new Pagination
            {
                PageNo = 1,
                PageSize = 10,
                TotalCount = 0
            };
        }

        public Pagination Pagination { get; set; }

        public List<MessageViewModel> Messages { get; set; }
    }

    public class MessageViewModel
    {
        public Guid Id { get; set; }

        public string SenderImageUrl { get; set; }

        [Display(Name = "From")]
        public string SenderName { get; set; }

        public DateTime SentDate { get; set; }

        [Display(Name = "Sent")]
        public string SentDisplayDate { get; set; }

        public string Subject { get; set; }

        public bool Read { get; set; }
    }
}