using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class SessionSettings
    {
        public SessionSettings()
        {
            IsPublic = true;
            ApproveJoinees = false;
        }

        public int Id { get; set; }

        [Key, ForeignKey("Session")]
        public Guid SessionId { get; set; }
        public  Session Session { get; set; }

        //Is the session publicly available
        [Required]
        public bool IsPublic { get; set; }

        //Will the creator manually approve people wanting to join?
        [Required]
        public bool ApproveJoinees { get; set; }

    }
}
