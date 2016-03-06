using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class SessionFeedback
    {
        public SessionFeedback()
        {
            Comments = string.Empty;
            CreatedDate = DateTime.UtcNow;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, ForeignKey("Session")]
        public Guid SessionId { get; set; }

        public Session Session { get; set; }

        //Who the feedback is for
        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        public UserProfile User { get; set; }
        
        [Required]
        public int Rating { get; set; }

        public string Comments { get; set; }

        //Who provided the feedback
        //Nullable to allow for anonymous
        [ForeignKey("Owner")]
        public string OwnerId { get; set; }
        public UserProfile Owner { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
