using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class UserFriend
    {
        public int Id { get; set; }

        [ForeignKey("Profile")]
        public string ProfileId { get; set; }
        public UserProfile Profile { get; set; }

            
        [ForeignKey("Friend")]
        public string FriendId { get; set; }
        public UserProfile Friend { get; set; }
    }
}
