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
        [Required]
        public string DisplayName { get; set; }
        
        public string ImageUrl { get; set; }

        [AllowHtml]
        public string About { get; set; }

        public string Website { get; set; }


        [Display(Name = "Xbox GamerTag")]
        public string XboxUsername { get; set; }

        [Display(Name = "Xbox GamerTag Url")]
        public string XboxUrl { get; set; }

        [Display(Name = "PSN")]
        public string PsnUsername { get; set; }

        [Display(Name = "PSN Url")]
        public string PsnUrl { get; set; }

        [Display(Name = "Steam Username")]
        public string SteamUsername { get; set; }

        [Display(Name = "Steam Account Url")]
        public string SteamUrl { get; set; }


    }
}
