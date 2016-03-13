using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
//using GamingSessionApp.Helpers;

namespace GamingSessionApp.ViewModels.Session
{
    public class CreateSessionVM
    {
        public CreateSessionVM()
        {
            IsPublic = true;
            NotifyOnJoin = true;
            NotifyOnLeave = true;
            MinRatingScore = 3;
            Goals = new List<string>();
        }

        [Required(ErrorMessage = "You need to choose a game"), DisplayName("Game")]
        public string GameTitle { get; set; }

        public int? IgdbGameId { get; set; }

        [Required, DataType(DataType.Date), DisplayName("Schedule Date"), /*PresentFutureDate*/]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ScheduledDate { get; set; }
        
        [Required, DataType(DataType.Time), DisplayName("Start Time")]
        public DateTime ScheduledTime { get; set; }

        [Required, DisplayName("Target Platform"), Range(1, 10, ErrorMessage = "Pick you gaming platform")]
        public int PlatformId { get; set; }

        [Required, DisplayName("Session Type"), Range(1, 6, ErrorMessage = "Choose the type of session you want to create")]
        public int TypeId { get; set; }

        //Goals and objectives
        public List<string> Goals { get; set; }

        [Required, DisplayName("Gamers Needed (including you)")]
        public int GamersRequired { get; set; }
        public SelectList GamersRequiredList { get; set; }

        [Required(ErrorMessage = "Please provide details about the session"), StringLength(5000), AllowHtml]
        [Display(Name = "Session Details")]
        public string Information { get; set; }
        
        [Required, DisplayName("Expected Duration")]
        public int DurationId { get; set; }
        public SelectList DurationList { get; set; }

        [Required, DisplayName("Public Session?")]
        public bool IsPublic { get; set; }

        [Required, DisplayName("Manually Approve Joining Members")]
        public bool ApproveJoinees { get; set; }

        [Required, DisplayName("Notify me when users join")]
        public bool NotifyOnJoin { get; set; }

        [Required, DisplayName("Notify me when users leave")]
        public bool NotifyOnLeave { get; set; }

        [Required, DisplayName("Minimum User Rating")]
        public int MinRatingScore { get; set; }
        
        [DisplayName("Invite User(s)")]
        public string InviteRecipients { get; set; }
    }
}
