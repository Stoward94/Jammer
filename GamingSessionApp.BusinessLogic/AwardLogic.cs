using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Awards;

namespace GamingSessionApp.BusinessLogic
{
    public class AwardLogic : BaseLogic, IAwardLogic
    {
        private readonly GenericRepository<Award> _awardsRepo;

        public AwardLogic(UnitOfWork uow)
        {
            UoW = uow;
            _awardsRepo = UoW.Repository<Award>();
        }

        public async Task<UserAwardsViewModel> GetUserAwards(string userId)
        {
            try
            {
                UserId = userId;

                var userRepo = UoW.Repository<UserProfile>();

                //Load the awards the user has obtained
                var userAwards = await userRepo.Get(u => u.UserId == userId)
                    .Select(x => new UserAwardsViewModel
                    {
                        BeginnerAwardsCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Beginner),
                        NoviceAwardsCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Novice),
                        IntermediateAwardsCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Intermediate),
                        AdvancedAwardsCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Advanced),
                        ExpertAwardsCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Expert),
                        
                        TotalAwards = x.Awards.Count,

                        Kudos = x.Kudos.Points,
                        Completed = x.Statistics.SessionsCompleted,
                        Created = x.Statistics.SessionsCreated,
                        Rating = x.Rating,
                        Awards = x.Awards.Select(a => new AwardViewModel
                        {
                            Id = a.Award.Id,
                            Title = a.Award.Title,
                            Description = a.Award.Description,
                            DateObtained = a.DateAwarded,
                            Obtained = true,
                            Requirement = a.Award.Requirement,
                            GroupId = a.Award.GroupId,
                            Level = a.Award.Level,
                            Slug = a.Award.Slug
                        }).ToList()
                    }).FirstOrDefaultAsync();

                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //Convert to timezone time
                foreach (var a in userAwards.Awards)
                {
                    a.DateObtained = a.DateObtained.ToTimeZoneTime(GetUserTimeZone());
                    a.DisplayDateObtained = a.DateObtained.ToMinsAgoTime(now);
                }

                //Now get all the awards
                var allAwards = await _awardsRepo.Get()
                    .Select(a => new AwardViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        Requirement = a.Requirement,
                        GroupId = a.GroupId,
                        Level = a.Level,
                        Slug = a.Slug
                    }).ToListAsync();

                foreach (var award in userAwards.Awards)
                {
                    allAwards.Remove(allAwards.FirstOrDefault(x => x.Id == award.Id));
                }
                
                //Combine the awards to build the complete list
                userAwards.Awards.AddRange(allAwards);

                //Order by the level
                userAwards.Awards = userAwards.Awards.OrderBy(x => x.Level.Id).ToList();

                //If the user has no awards just return an empty (new) model
                if (!userAwards.Awards.Any())
                    return new UserAwardsViewModel();
                
               return userAwards;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get awards for user : " + userId);
                throw;
            }
        }
    }
}