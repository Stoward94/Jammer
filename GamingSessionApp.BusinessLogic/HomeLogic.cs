using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.ViewModels.Home;
using static GamingSessionApp.BusinessLogic.SystemEnums;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;

namespace GamingSessionApp.BusinessLogic
{
    public class HomeLogic : BaseLogic, IHomeLogic, IDisposable
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

                viewModel.NewSessions = await GetNewSessions();
                viewModel.OpenSessions = await GetOpenSessions();

                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error preparing home view model");
                throw;
            }
        }

        private async Task<List<SessionListItem>> GetOpenSessions()
        {
            try
            {
                var openSessions = await _sessionRepo.Get(s => s.Settings.IsPublic && s.Active) //Only public and active sessions
                        .Where(x => x.StatusId != (int)SessionStatusEnum.FullyLoaded) //Sessions that are not full
                        .OrderBy(x => x.ScheduledDate)
                        .Select(s => new SessionListItem
                        {
                            ScheduledDate = s.ScheduledDate,
                            SessionId = s.Id,
                            Platform = s.Platform.Name,
                            TypeId = s.TypeId,
                            GamerCount = s.Members.Count + "/" + s.MembersRequired,
                            Summary = s.Information
                        })
                        .Take(15)
                        .ToListAsync();

                ConvertTimesToTimeZone(openSessions);


                return openSessions;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetOpenSessions() threw an error!");
                throw;
            }
        }
        private async Task<List<SessionListItem>> GetNewSessions()
        {
            try
            {
                var newSessions = await _sessionRepo.Get(s => s.Settings.IsPublic && s.Active) //Only public and active sessions
                        .OrderByDescending(x => x.ScheduledDate)
                        .Select(s => new SessionListItem
                        {
                            ScheduledDate = s.ScheduledDate,
                            SessionId = s.Id,
                            Platform = s.Platform.Name,
                            TypeId = s.TypeId,
                            GamerCount = s.Members.Count + "/" + s.MembersRequired,
                            Summary = s.Information
                        }).Take(15).ToListAsync();

                ConvertTimesToTimeZone(newSessions);

                return newSessions;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetNewSessions() threw an error!");
                throw;
            }
        }

        private void ConvertTimesToTimeZone(List<SessionListItem> model)
        {
            //Convert the DateTimes to the users time zone
            foreach (var s in model)
            {
                s.ScheduledDate = s.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
            }
        }

        
    }
}
