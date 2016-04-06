using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class UserStatistics
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        public UserProfile User { get; set; }

        public int SessionsCreated { get; set; }

        public int SessionsCompleted { get; set; }
    }
}
