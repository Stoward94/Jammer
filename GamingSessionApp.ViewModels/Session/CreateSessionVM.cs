using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GamingSessionApp.Helpers;

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
        }


        [Required, DataType(DataType.Date), DisplayName("Schedule Date"), PresentFutureDate]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ScheduledDate { get; set; }
        
        [Required, DataType(DataType.Time), DisplayName("Start Time")]
        public DateTime ScheduledTime { get; set; }

        [Required, DisplayName("Target Platform")]
        public int PlatformId { get; set; }
        public SelectList PlatformList { get; set; }

        [Required, DisplayName("Session Type")]
        public int TypeId { get; set; }
        public SelectList SessionTypeList { get; set; }

        [Required, DisplayName("Gamers Needed")]
        public int GamersRequired { get; set; }
        public SelectList GamersRequiredList { get; set; }

        [Required, StringLength(5000)]
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
