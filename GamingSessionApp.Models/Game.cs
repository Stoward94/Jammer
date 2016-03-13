using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        //If the game has been selected from IGDB, then store the id
        public int? IgdbGameId { get; set; }

        //Store the game title
        [Required]
        public string GameTitle { get; set; }
    }
}
