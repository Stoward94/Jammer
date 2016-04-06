using System;
using System.Collections.Generic;
using GamingSessionApp.Models;

namespace GamingSessionApp.ViewModels.Awards
{
    public class UserAwardsViewModel
    {
        public UserAwardsViewModel()
        {
            Awards = new List<AwardViewModel>();
        }

        public int TotalAwards { get; set; }

        public int BeginnerAwardsCount { get; set; }
        public int NoviceAwardsCount { get; set; }
        public int IntermediateAwardsCount { get; set; }
        public int AdvancedAwardsCount { get; set; }
        public int ExpertAwardsCount { get; set; }

        public int Created { get; set; }
        public int Completed { get; set; }
        public int Kudos { get; set; }
        public double Rating { get; set; }

        public List<AwardViewModel> Awards { get; set; }
    }
    
    public class AwardViewModel
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        //Class name to use
        public string Slug { get; set; }

        public bool Obtained { get; set; }

        public DateTime DateObtained { get; set; }
        public string DisplayDateObtained { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Requirement { get; set; }

        public AwardLevel Level { get; set; }
    }
}
