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

        //Who is the creator of this session
        [Required]
        public string CreatorName { get; set; }

        //When the Session was created
        [Required]
        public DateTime CreatedDate { get; set; }

        //Date of when the session is scheduled for
        [Required]
        public DateTime ScheduledDate { get; set; }

        //What's the status of the session
        [Required]
        public string Status { get; set; }

        //Which platform is this session for?
        [Required]
        public string Platform { get; set; }

        //Type of session this session is 'Boosting/Co-op etc.
        [Required]
        public string Type { get; set; }

        //# of gamers needed for the session
        [Required]
        public int GamersRequired { get; set; }

        //# of gamers registered for session
        public int SignedGamersCount { get; set; }

        //Description of what the session is about
        [Required, StringLength(5000)]
        public string Information { get; set; }

        //Expected duration of the session
        [Required]
        public string Duration { get; set; }

        //Is the session publicly available
        [Required]
        public bool IsPublic { get; set; }

        public List<SessionMessage> Messages { get; set; }

        //A collection of the gamers signed to the session
        public virtual ICollection<ApplicationUser> SignedGamers { get; set; }


        // ---- Display Option Variables ---- //
        public bool CanJoin { get; set; }
        public bool CanLeave { get; set; }
        public bool CanPost { get; set; }

    }
}
