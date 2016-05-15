using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GamingSessionApp.Models;

namespace GamingSessionApp.ViewModels.Session
{
    public class SessionDetailsVM
    {
        [HiddenInput]
        public Guid Id { get; set; }

        public string Status { get; set; }
        public string StatusDescription { get; set; }

        public string CreatorId { get; set; }

        public string GameTitle { get; set; }

        //Who is the creator of this session
        [Required]
        public string CreatorName { get; set; }
        
        //Date of when the session is scheduled for
        [Required]
        public DateTime ScheduledDate { get; set; }
        public string ScheduledDisplayDate { get; set; }

        //Which platform is this session for?
        [Required]
        public int PlatformId { get; set; }
        public string Platform { get; set; }

        //Type of session this session is 'Boosting/Co-op etc.
        [Required]
        public int TypeId { get; set; }

        public string TypeDescription { get; set; }

        //# of gamers needed for the session
        [Required]
        public int MembersRequired { get; set; }

        //# of gamers registered for session
        public int MembersCount { get; set; }

        //Description of what the session is about
        [Required, StringLength(5000)]
        public string Information { get; set; }

        //List of the sessions goals
        public List<string> Goals { get; set; }
        
        //Expected duration of the session
        [Required]
        public string Duration { get; set; }

        //Whats the minimum user rating
        [Display(Name = "Minimum User Rating")]
        public int MinUserRating { get; set; }

        //Is the session publicly available
        [Required]
        public bool IsPublic { get; set; }

        public bool Active { get; set; }

        public SessionCommentsViewModel Comments { get; set; }

        //A collection of the gamers signed to the session
        public virtual ICollection<SessionMemberViewModel> Members { get; set; }


        // ---- Display Option Variables ---- //
        public bool CanJoin { get; set; }
        public bool CanLeave { get; set; }
        public bool CanEdit { get; set; }

    }
}
