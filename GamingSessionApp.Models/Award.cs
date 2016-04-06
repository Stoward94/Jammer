using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class Award
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int Requirement { get; set; }

        [Required, ForeignKey("Level")]
        public int LevelId { get; set; }
        public AwardLevel Level { get; set; }

        [Required, ForeignKey("Group")]
        public int GroupId { get; set; }
        public AwardGroup Group { get; set; }

        [Required]
        public string Slug { get; set; }

        public ICollection<UserAward> Users { get; set; }
    }
}
