using System.ComponentModel.DataAnnotations;

namespace GamingSessionApp.Models
{
    public class AwardGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Group { get; set; }
    }
}
