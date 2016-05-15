using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Leaderboard;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.BusinessLogic
{
    public class LeaderboardLogic : BaseLogic, ILeaderboardLogic
    {
        public LeaderboardLogic(UnitOfWork uow)
        {
            UoW = uow;
        }

        public async Task<UserLeaderboardViewModel> GetUserLeaderboard(string userId, int page)
        {
            try
            {
                //Pagination calculation
                int pageSize = 10;
                int skip = (page - 1)*pageSize;

                //Base query
                var query = UoW.Repository<UserProfile>().Get().OrderByDescending(u => u.Kudos.Points);

                //Fetch the data including a groupby count
                var data = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(u => new UserListItem
                    {
                        Kudos = u.Kudos.Points.ToString(),
                        ThumbnailUrl = u.ThumbnailUrl,
                        Username = u.DisplayName,
                        Rank = query.Count(o => o.Kudos.Points > u.Kudos.Points) + 1 //Calculats the rank (index)
                    })
                    .GroupBy(u => new { Total = query.Count() })
                    .FirstOrDefaultAsync();
                
                //Build the view model
                var model = new UserLeaderboardViewModel
                {
                    Users = data.Select(x => x).ToList(),
                    Friends = new List<UserListItem>(),
                    Pagination = new Pagination
                    {
                        TotalCount = data.Key.Total,
                        PageNo = page,
                        PageSize = pageSize
                    }
                };

                foreach (var u in model.Users)
                {
                    u.Kudos = int.Parse(u.Kudos).ToString("n0");
                }

                //If we are logged in
                if (userId != null)
                {
                    //Get the rank of the current logged in user
                    model.CurrentUser = await query.Where(x => x.UserId == userId)
                        .Select(u => new UserListItem
                        {
                            Kudos = u.Kudos.Points.ToString(),
                            ThumbnailUrl = u.ThumbnailUrl,
                            Username = u.DisplayName,
                            Rank = query.Count(o => o.Kudos.Points >= u.Kudos.Points)
                        })
                        .FirstOrDefaultAsync();


                    model.Friends = await query.Where(x => x.UserId == userId)
                        .SelectMany(x => x.Friends)
                        .OrderByDescending(u => u.Friend.Kudos.Points)
                        .Select(x => new UserListItem
                        {
                            Kudos = x.Friend.Kudos.Points.ToString(),
                            ThumbnailUrl = x.Friend.ThumbnailUrl,
                            Username = x.Friend.DisplayName,
                            Rank = query.Count(o => o.Kudos.Points > x.Friend.Kudos.Points) + 1 //Calculates the rank (index)
                        })
                        .ToListAsync();

                    model.Friends.Add(model.CurrentUser);
                    model.Friends = model.Friends.OrderBy(x => x.Rank).ToList();
                }

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting the user leaderboard. userId :" + userId);
                throw;
            }
        }
    }
}