using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GamingSessionApp.Models
{
    public class Session
    {
        public Session()
        {
            CreatedDate = DateTime.Now;
            SignedGamersCount = 0;
        }

        public int Id { get; set; }

        //Who is the creator of this session
        [Required]
        public string CreatorId { get; set; }
        //public virtual ApplicationUser Creator { get; set; }

        //When the Session was created
        [Required]
        public DateTime CreatedDate { get; set; }

        //Date of when the session is scheduled for
        [Required]
        public DateTime ScheduledDate { get; set; }

        //Which platform is this session for?
        [Required]
        public int PlatformId { get; set; }
        public virtual Platform Platform { get; set; }
        
        //Type of session this session is 'Boosting/Co-op etc.
        [Required]
        public int TypeId { get; set; }
        public virtual SessionType Type { get; set; }

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
        public int DurationId { get; set; }
        public virtual SessionDuration Duration { get; set; }

        //Is the session publicly available
        [Required]
        public bool IsPublic { get; set; }

        //A collection of the gamers signed to the session
        public virtual ICollection<ApplicationUser> SignedGamers { get; set; }
    }
}