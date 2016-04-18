using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.ViewModels.Home;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.BusinessLogic
{
    public class HomeLogic : BaseLogic, IHomeLogic
    {
        //Business Logics
        private readonly GenericRepository<Session> _sessionRepo;

        public HomeLogic(UnitOfWork uow)
        {
            UoW = uow;
            _sessionRepo = UoW.Repository<Session>();
        }

        public async Task<HomeViewModel> GetHomeViewModel(string userId)
        {
            try
            {
                UserId = userId;

                var viewModel = new HomeViewModel();

                viewModel.KudosLeaderboard = await GetKudosLeaderBoard();
                viewModel.NewUsers = await GetNewestUsers();
                viewModel.NewSessions = await GetNewSessions();



                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error preparing home view model");
                throw;
            }
        }

        public async Task<SearchResultsViewModel> GetSearchResults(string term, string userId)
        {
            try
            {
                UserId = userId;

                var sessionResults = await _sessionRepo.Get(x => x.Settings.IsPublic && x.Game.GameTitle.Contains(term))
                    .Select(x => new SessionSearchResult
                    {
                        PlatformId = x.PlatformId,
                        Platform = x.Platform.Name,
                        ScheduledStart = x.ScheduledDate,
                        Id = x.Id,
                        TypeId = x.TypeId,
                        Type = x.Type.Name,
                        Game = x.Game.GameTitle
                    })
                    .Take(10)
                    .ToListAsync();

                var userResults = await UoW.Repository<UserProfile>().Get(x => x.DisplayName.Contains(term))
                    .Select(x => new UserSearchResult
                    {
                        Kudos = x.Kudos.Points.ToString(),
                        ThumbnailUrl = x.ThumbnailUrl,
                        Username = x.DisplayName
                    })
                    .Take(10)
                    .ToListAsync();

                //Set times to usertime zone
                foreach (var s in sessionResults)
                {
                    s.ScheduledStart = s.ScheduledStart.ToTimeZoneTime(GetUserTimeZone());
                    s.DisplayScheduledStart = s.ScheduledStart.ToFullDateTimeString();
                }

                //Update kudos score
                foreach (var u in userResults)
                {
                    u.Kudos = TrimKudos(u.Kudos);
                }

                var model = new SearchResultsViewModel
                {
                    Sessions = sessionResults,
                    Users = userResults,
                    IsUtcTime = UserId == null
                };

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error when searching for term: " + term);
                return null;
            }
        }

        private async Task<List<SessionListItemViewModel>> GetNewSessions()
        {
            try
            {
                var newSessions = await _sessionRepo.Get(s => s.Settings.IsPublic && s.Active) //Only public and active sessions
                        .OrderByDescending(x => x.ScheduledDate)
                        .Select(s => new SessionListItemViewModel
                        {
                            Id = s.Id,
                            Creator = s.Creator.DisplayName,
                            Duration = s.Duration.Duration,
                            Game = s.Game.GameTitle,
                            MembersCount = s.Members.Count,
                            PlatformId = s.PlatformId,
                            RequiredCount = s.MembersRequired,
                            ScheduledTime = s.ScheduledDate,
                            Status = s.Status.Name,
                            StatusDescription = s.Status.Description,
                            Type = s.Type.Name,
                            TypeId = s.TypeId,
                            Platform = s.Platform.Name,
                        }).Take(10).ToListAsync();

                foreach (var s in newSessions)
                {
                    s.ScheduledTime = s.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDisplayTime = s.ScheduledTime.ToFullDateTimeString();
                }

                return newSessions;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetNewSessions() threw an error!");
                throw;
            }
        }

        private async Task<KudosLeaderboard> GetKudosLeaderBoard()
        {
            try
            {
                KudosLeaderboard leaderboard = new KudosLeaderboard
                {
                    Users = await UoW.Repository<UserProfile>().Get()
                        .OrderByDescending(x => x.Kudos.Points)
                        .Select(x => new UserSearchResult
                        {
                            Kudos = x.Kudos.Points.ToString(),
                            ThumbnailUrl = x.ThumbnailUrl,
                            Username = x.DisplayName
                        })
                        .Take(10)
                        .ToListAsync()
                };

                foreach (var u in leaderboard.Users)
                {
                    u.Kudos = int.Parse(u.Kudos).ToString("n0");
                }

                return leaderboard;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting user kudos leaderboard");
                return null;
            }
        }

        private async Task<List<NewestUsers>> GetNewestUsers()
        {
            try
            {

                var users = await UoW.Repository<UserProfile>().Get()
                    .OrderByDescending(x => x.User.DateRegistered)
                    .Select(x => new NewestUsers
                    {
                        ThumbnailUrl = x.ThumbnailUrl,
                        Username = x.DisplayName,
                        Registered = x.User.DateRegistered

                    })
                    .Take(10)
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting user kudos leaderboard");
                return null;
            }
        }
    }
}
