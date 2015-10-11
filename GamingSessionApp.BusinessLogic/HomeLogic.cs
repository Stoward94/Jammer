using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.ViewModels.Home;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class HomeLogic : BaseLogic
    {
        //Business Logics
        private readonly SessionLogic _sessionLogic;

        public HomeLogic()
        {
            _sessionLogic = new SessionLogic();
        }

        public async Task<List<SessionListItem>> GetOpenSessions()
        {
            try
            {
                var query = _sessionLogic.GetAllQueryable();

                var openSessions = await query
                        .Where(s => s.Settings.IsPublic && s.Active) //Only public and active sessions
                        .Where(x => x.StatusId != (int)SessionStatusEnum.FullyLoaded) //Sessions that are not full
                        .OrderBy(x => x.ScheduledDate)
                        .Select(s => new SessionListItem
                        {
                            ScheduledDate = s.ScheduledDate,
                            SessionId = s.Id,
                            Platform = s.Platform.Name,
                            Type = s.Type.Name,
                            GamerCount = s.SignedGamers.Count + "/" + s.GamersRequired,
                            Summary = s.Information
                        }).Take(15).ToListAsync();

                ConvertTimesToTimeZone(openSessions);


                return openSessions;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public async Task<List<SessionListItem>> GetNewSessions()
        {
            try
            {
                var query = _sessionLogic.GetAllQueryable();

                var newSessions = await query
                        .Where(s => s.Settings.IsPublic && s.Active) //Only public and active sessions
                        .OrderByDescending(x => x.CreatedDate)
                        .Select(s => new SessionListItem
                        {
                            ScheduledDate = s.ScheduledDate,
                            SessionId = s.Id,
                            Platform = s.Platform.Name,
                            Type = s.Type.Name,
                            GamerCount = s.SignedGamers.Count + "/" + s.GamersRequired,
                            Summary = s.Information
                        }).Take(15).ToListAsync();

                ConvertTimesToTimeZone(newSessions);

                return newSessions;
            }
            catch (Exception)
            {

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
