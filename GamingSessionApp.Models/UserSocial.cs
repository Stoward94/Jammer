using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class UserSocial
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        public UserProfile User { get; set; }

        public string Xbox { get; set; }

        public string PlayStation { get; set; }

        public string Steam { get; set; }

        public string Twitch { get; set; }

        public string Twitter { get; set; }

        public string Facebook { get; set; }

    }
}
