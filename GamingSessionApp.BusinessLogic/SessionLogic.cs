using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;
using GamingSessionApp.ViewModels.SessionComments;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionLogic : BaseLogic
    {

    #region Variables

        //Session Repository
        private readonly GenericRepository<Session> _sessionRepo;

        //Business Logics
        private readonly SessionCommentLogic _commentLogic;
        private readonly SessionDurationLogic _durationLogic;
        private readonly SessionTypeLogic _typeLogic;
        private readonly PlatformLogic _platformLogic;

    #endregion

    #region Constructor

        public SessionLogic()
        {
            _sessionRepo = UoW.Repository<Session>();

            _commentLogic = new SessionCommentLogic();
            _durationLogic = new SessionDurationLogic();
            _typeLogic = new SessionTypeLogic();
            _platformLogic = new PlatformLogic();
        }

        #endregion

    #region CRUD Operations
        
        public async Task<List<SessionListItemViewModel>> GetSessions(string userId)
        {
            UserId = userId;

            //Get all the sessions that are public and active
            List<SessionListItemViewModel> sessions = await _sessionRepo.Get().Where(s => s.Settings.IsPublic && s.Active)
                .Select(x => new SessionListItemViewModel
                {
                    Id = x.Id,
                    Creator = x.Creator.DisplayName,
                    ScheduledDate = x.ScheduledDate.ToString(),
                    Status = x.Status.Name,
                    Platform = x.Platform.Name,
                    Type = x.Type.Name,
                    RequiredCount = x.MembersRequired,
                    MembersCount = x.Members.Count,
                    Duration = x.Duration.Duration
                })
                .OrderBy(x => x.ScheduledDate)
                .Take(20)
                .ToListAsync();

            //Convert the DateTimes to the users time zone
            foreach (var s in sessions)
            {
                //Parse string then convert to user timezone
                DateTime scheduledDate = DateTime.Parse(s.ScheduledDate);
                scheduledDate = scheduledDate.ToTimeZoneTime(GetUserTimeZone());

                //Create user friendly display time
                s.ScheduledDate = DateToUserFriendlyString(scheduledDate);
            }

            return sessions;
        }

        public IQueryable<Session> GetAllQueryable()
        {
            return _sessionRepo.Get();
        }

        public async Task<ValidationResult> CreateSession(CreateSessionVM model, string userId)
        {
            try
            {
                UserId = userId;

                //Map the properties from the model to the session
                Session session = new Session
                {
                    CreatorId = userId,
                    PlatformId = model.PlatformId,
                    TypeId = model.TypeId,
                    MembersRequired = model.GamersRequired,
                    Information = model.Information,
                    DurationId = model.DurationId,
                    ScheduledDate = CombineDateAndTime(model.ScheduledDate, model.ScheduledTime),
                    Settings = new SessionSettings()
                    {
                        IsPublic = true,
                        ApproveJoinees = model.ApproveJoinees
                    }
                };

                //Convert all dates to UTC format
                ConvertSessionTimesToUtc(session);

                //Future date check
                if (session.ScheduledDate < DateTime.UtcNow)
                    return VResult.AddError("You must schedule the session to be a future date and time.");

               Session conflictSession = await CheckForSessionConflict(session, userId);

                if (conflictSession != null)
                    return VResult.AddError("This sessions time conflicts with an existing session. " +
                                            "Please pick another date and time.");

                

                //Add the intial message to the session messages feed
                _commentLogic.AddSessionCreatedComment(session);

                //Load the UserProfile of the creator to add as a member
                UserProfile creator = await UoW.Repository<UserProfile>().GetByIdAsync(userId);

                //Add the creator as a member of the session
                session.Members.Add(creator);

                //Insert the new session into the db
                _sessionRepo.Insert(session);
                await SaveChangesAsync();

                //Send out invites to users if applicable
                if (!string.IsNullOrEmpty(model.InviteRecipients))
                {
                    List<string> usernames = model.InviteRecipients.Split(',').ToList();

                    List<UserProfile> recipients = await UoW.Repository<UserProfile>()
                        .Get(x => usernames.Contains(x.DisplayName))
                        .Include(x => x.User)
                        .Include(x => x.Preferences)
                        .ToListAsync();

                    //Send Email
                    EmailLogic email = new EmailLogic();
                    await email.SessionInviteEmail(session, creator.DisplayName, recipients);

                    //Send Notification
                    NotificationLogic notif = new NotificationLogic();
                    await notif.SessionInviteNotification(session, creator.UserId, recipients);
                }

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to create session");   
                return VResult.AddError("Unable to create the session at this time. " +
                                        "Please try again later.");
            }
        }

        public async Task<bool> EditSession(EditSessionVM viewModel)
        {
            try
            {
                throw new NotImplementedException();
                //Load the session from the db
                Session model = await _sessionRepo.Get(x => x.Id == viewModel.SessionId)
                    .Include(x => x.Platform)
                    .Include(x => x.Status)
                    .Include(x => x.Type)
                    .Include(x => x.Duration)
                    .Include(x => x.Settings)
                    .FirstOrDefaultAsync();

                //Map the changes (top-tier & 2nd level)
                //Mapper.Map(viewModel, model);
                //Mapper.Map(viewModel, model.Settings);

                //Convert all dates to UTC format
                ConvertSessionTimesToUtc(model);

                //Update the db
                _sessionRepo.Update(model);
                await SaveChangesAsync();

                //TODO: Email linked members about changes

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        #endregion

    #region View Model Preperation

        /// <summary>
        /// Prepares the view model used to create a session ready to be passed the view
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CreateSessionVM> CreateSessionViewModel(CreateSessionVM viewModel, string userId)
        {
            UserId = userId;

            //Set start time default if null
            if (viewModel.ScheduledTime == new DateTime())
            {
                //Adds 30 mins from now and rounds up to nearest 30
                DateTime dt = DateTime.UtcNow.AddMinutes(30).ToTimeZoneTime(GetUserTimeZone());
                TimeSpan d = TimeSpan.FromMinutes(30);

                viewModel.ScheduledTime = new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
            }

            //Set start date default if null
            if (viewModel.ScheduledDate == new DateTime())
            {
                viewModel.ScheduledDate = DateTime.UtcNow.AddHours(1).ToTimeZoneTime(GetUserTimeZone());
            }

            //Add the select lists options
            viewModel.DurationList = await _durationLogic.GetDurationSelectList();
            viewModel.SessionTypeList = await _typeLogic.GetTypeSelectList();
            viewModel.PlatformList = await _platformLogic.GetPlatformSelectList();
            
            //Get the 'how many gamers needed' list options
            viewModel.GamersRequiredList = GetGamersRequiredOptions();

            return viewModel;
        }

        /// <summary>
        /// Re-binds the select list options for the Edit Session view model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<EditSessionVM> PrepareEditSessionVM(EditSessionVM viewModel)
        {
            //Add the select lists options
            viewModel.DurationList = await _durationLogic.GetDurationSelectList();
            viewModel.SessionTypeList = await _typeLogic.GetTypeSelectList();
            viewModel.PlatformList = await _platformLogic.GetPlatformSelectList();

            return viewModel;
        } 

        /// <summary>
        /// Get the details of a session and set up the edit view model
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<EditSessionVM> EditSessionVM(Guid sessionId)
        {
            try
            {
                //Load and project the session from the db
                EditSessionVM viewModel = await _sessionRepo.Get(x => x.Id == sessionId)
                    .Select(x => new EditSessionVM
                    {
                        SessionId = x.Id,
                        CreatorId = x.CreatorId,
                        ScheduledDate = x.ScheduledDate,
                        ScheduledTime = x.ScheduledDate,
                        Status = x.Status.Name,
                        PlatformId = x.PlatformId,
                        TypeId = x.TypeId,
                        GamersRequired = x.MembersRequired.ToString(),
                        Information = x.Information,
                        DurationId = x.DurationId,
                        IsPublic = x.Settings.IsPublic,
                        ApproveJoinees = x.Settings.ApproveJoinees

                    }).FirstOrDefaultAsync();

                //Make sure we have found something
                if(viewModel == null) return null;

                //Convert the DateTimes to the users time zone
                viewModel.ScheduledDate = viewModel.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                viewModel.ScheduledTime = viewModel.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());

                //Add the select lists options
                viewModel.DurationList = await _durationLogic.GetDurationSelectList();
                viewModel.SessionTypeList = await _typeLogic.GetTypeSelectList();
                viewModel.PlatformList = await _platformLogic.GetPlatformSelectList();


                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        } 

    #endregion

    #region Helpers
        
        /// <summary>
        /// Returns a select list of options for the required amount of gamers
        /// </summary>
        /// <returns></returns>
        private SelectList GetGamersRequiredOptions()
        {
            //Anon object used to hold id and value (for the unlimited option)
            var options = new List<object>();

            //Add options 2 - 24
            for (int i = 2; i < 25; i++)
            {
                options.Add(new { id = i, value = i });
            }

            options.Add(new { id = 32, value = 32 });
            options.Add(new { id = 64, value = 64 });
            options.Add(new { id = -1, value = "Unlimited" });

            return new SelectList(options, "id", "value");
        }
        

        /// <summary>
        /// Combines the date and time inputs entered when creating a session to form a single DateTime object
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private DateTime CombineDateAndTime(DateTime date, DateTime time)
        {
            TimeSpan ts = time.TimeOfDay;

            //Quick rounding to nearest 15 mins
            var totalMinutes = (int)(ts + new TimeSpan(0, 15 / 2, 0)).TotalMinutes;
            TimeSpan roundedTime = new TimeSpan(0, totalMinutes - totalMinutes % 15, 0);

            DateTime newDateTime = date + roundedTime;

            return newDateTime;
        }

        protected void ConvertSessionTimesToUtc(Session model)
        {
            //Convert all dates to UTC format
            model.ScheduledDate = TimeZoneInfo.ConvertTimeToUtc(model.ScheduledDate, GetUserTimeZone());
        }

        protected void ConvertSessionTimesToTimeZone(Session model)
        {
            TimeZoneInfo timeZone = GetUserTimeZone();

            //Convert the DateTimes to the users time zone
            model.CreatedDate = model.CreatedDate.ToTimeZoneTime(timeZone);
            model.ScheduledDate = model.ScheduledDate.ToTimeZoneTime(timeZone);

            if (model.Comments != null)
            {
                foreach (var c in model.Comments)
                {
                    c.CreatedDate = c.CreatedDate.ToTimeZoneTime(timeZone);
                }
            }
        }

        /// <summary>
        /// Checks through the users session to see if the new 
        /// session conflicts with any of them.
        /// </summary>
        /// <param name="tSession"></param>
        /// <returns></returns>
        private async Task<Session> CheckForSessionConflict(Session tSession, string userId)
        {
            //Load user
            List<Session> userSessions = await _sessionRepo.Get(x => x.CreatorId == userId)
                .Include(x => x.Duration)
                .Where(x => x.Active)
                .ToListAsync();


            foreach (var s in userSessions)
            {
                if (tSession.ScheduledDate >= s.ScheduledDate &&
                    tSession.ScheduledDate < s.ScheduledDate.AddMinutes(s.Duration.Minutes))
                {
                    //Return conflicting session
                    return tSession;
                }
            }

            //If no conflicts return null
            return null;
        }

        /// <summary>
        /// Evaluates the session and updates the sessions status if needed
        /// </summary>
        /// <param name="session"></param>
        private void CheckSessionStatus(Session session)
        {
            //Is the session now full?
            if (session.MembersCount == session.MembersRequired)
            {
                session.StatusId = (int) SessionStatusEnum.FullyLoaded;
            }
            else
            {
                session.StatusId = (int)SessionStatusEnum.Recruiting;
            }
        }

        private string DateToUserFriendlyString(DateTime date)
        {
            string friendlyDate;

            //DateTime.Now in users time zone
            DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

            if (date.Day == now.Day)
            {
                friendlyDate = "Today";
            }
            else if (date.Day == now.Day + 1)
            {
                friendlyDate = "Tomorrow";
            }
            else
            {
                //Date
                string dayOfWeek = date.ToString("ddd");
                int dayNumber = date.Day;
                string month = date.ToString("MMM");
                string daySuffix;

                //Add day suffix
                switch (dayNumber)
                {
                    case 1:
                    case 21:
                    case 31:
                        daySuffix = "st";
                        break;

                    case 2:
                    case 22:
                        daySuffix = "nd";
                        break;

                    case 3:
                    case 23:
                        daySuffix = "rd";
                        break;

                    default:
                        daySuffix = "th";
                        break;

                }

                friendlyDate = $"{dayOfWeek} {dayNumber}{daySuffix} {month}";
            }

            //Time
            string friendlyTime = date.ToString("HH:mm");

            return friendlyDate + " @ " + friendlyTime;
        }

        #endregion

        public async Task<ValidationResult> AddUserToSession(string userId, Guid sessionId)
        {
            try
            {
                UserId = userId;

                Session targetSession = await _sessionRepo.Get(x => x.Id == sessionId)
                    .Include(x => x.Members)
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync();

                if (targetSession == null)
                    return VResult.AddError("Unable to join this session as this time");

                //Check the session is active and that there is room to join
                if (!targetSession.Active)
                    return VResult.AddError("This session is no longer active");

                 if(targetSession.MembersCount >= targetSession.MembersRequired)
                    return VResult.AddError("You are unable to join this session as it's full. Why not create a new session");

                //Check this user isn't already a member of the session.
                bool existingUser = targetSession.Members.Any(x => x.UserId == userId);

                //If we found a matching user return error;
                if (existingUser)
                    return VResult.AddError("You are already a member of this session");

                //Check that this session doesn't conflict with the users existing sessions
                Session conflictSession = await CheckForSessionConflict(targetSession, userId);

                if(conflictSession != null)
                    return VResult.AddError("This sessions time conflicts with another session you have already joined. " +
                                            "You are unable to join this session as a result.");


                //Load the UserProfile of the creator to add as a member
                UserProfile user = await UoW.Repository<UserProfile>().GetByIdAsync(userId);

                targetSession.Members.Add(user);

                //Check if the status needs updating
                CheckSessionStatus(targetSession);

                //Add 'User joined' message to session feed
                //_commentLogic.UserId = UserId;
                //_commentLogic.AddUserJoinedComment(targetSession, user.DisplayName);

                //Send notification to the session owner
                NotificationLogic nLogic = new NotificationLogic();
                await nLogic.AddUserJoinedNotification(targetSession, user.UserId);

                _sessionRepo.Update(targetSession);
                await SaveChangesAsync();

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to add user to session");
                throw;
            }
        }

        public async Task<bool> RemoveUserFromSession(string userId, Guid sessionId)
        {
            try
            {
                Session targetSession = await _sessionRepo.Get(x => x.Id == sessionId)
                    .Include(x => x.Members)
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync();

                if (targetSession == null) return false;

                if (!targetSession.Active) return false;

                UserProfile user = targetSession.Members.First(x => x.UserId == userId);

                //Remove the user
                targetSession.Members.Remove(user);

                //Check if the status needs updating
                CheckSessionStatus(targetSession);

                //Add 'User left' message to session feed
                //_commentLogic.UserId = UserId;
                //_commentLogic.AddUserLeftComment(targetSession, user.DisplayName);

                _sessionRepo.Update(targetSession);
                await SaveChangesAsync();

                //Send email
                //TODO: Send email
                //Send notification
                //TODO: Send notification

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }
}