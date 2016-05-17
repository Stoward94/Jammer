using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.Profile
{
    public class EditProfileViewModel
    {
        [Required, Display(Name ="Display Name")]
        public string DisplayName { get; set; }
        
        public string ImageUrl { get; set; }

        [AllowHtml]
        public string About { get; set; }

        public string Website { get; set; }
        
        [Display(Name = "Xbox GamerTag")]
        public string XboxUsername { get; set; }

        [Display(Name = "PSN Username")]
        public string PsnUsername { get; set; }

        [Display(Name = "Steam Username")]
        public string SteamUsername { get; set; }

        [Display(Name = "Twitch")]
        public string Twitch { get; set; }

        [Display(Name = "Twitter")]
        public string Twitter { get; set; }

        [Display(Name = "Facebook")]
        public string Facebook { get; set; }

    }
}
