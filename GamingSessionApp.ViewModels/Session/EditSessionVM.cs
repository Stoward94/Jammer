using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.Session
{
    public class EditSessionVM
    {
        [HiddenInput, Required]
        public Guid SessionId { get; set; }

        [HiddenInput, Required]
        public string CreatorId { get; set; }

        [Required, DataType(DataType.Date), DisplayName("Scheduled Date")]
        public DateTime ScheduledDate { get; set; }

        [Required, DisplayName("Start Time")]
        public string ScheduledTime { get; set; }
        public SelectList ScheduledTimeList { get; set; }

        [Required, DisplayName("Target Platform")]
        public int PlatformId { get; set; }
        public SelectList PlatformList { get; set; }

        [Required, DisplayName("Session Type")]
        public int TypeId { get; set; }
        public SelectList SessionTypeList { get; set; }

        [Required, DisplayName("Gamers Needed")]
        public string GamersRequired { get; set; }
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
    }
}
