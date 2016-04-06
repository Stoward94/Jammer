using System.ComponentModel.DataAnnotations;

namespace GamingSessionApp.Models
{
    public class AwardLevel
    {
        [Key]
        public int Id { get; set; }

        public string Level { get; set; }
    }
}
