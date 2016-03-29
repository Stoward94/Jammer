using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionLogic : BaseLogic, ISessionLogic
    {

    #region Variables

        //Session Repository
        private readonly GenericRepository<Session> _sessionRepo;

        //Business Logics
        private SessionCommentLogic _commentLogic;
        private readonly SessionDurationLogic _durationLogic;

        #endregion

    #region Constructor

        public SessionLogic(UnitOfWork uow)
        {
            UoW = uow;
            _sessionRepo = UoW.Repository<Session>();
            
            _durationLogic = new SessionDurationLogic(uow);
        }

        #endregion

    #region CRUD Operations

        public async Task<AllSessionsViewModel> GetSessions(string userId, SessionsFilter filter = null)
        {
            try
            {
                UserId = userId;

                var query = _sessionRepo.Get();

                int numberOfDays = 7;
                int skip = 0;

                DateTime startDate = DateTime.UtcNow.Date;
                int timeZoneOffset = GetUserTimeZone().GetUtcOffset(DateTime.UtcNow).Hours;
                bool useStartDate = true;

                #region Filter

                if (filter != null)
                {
                    //Set the skip results value based on the page
                    skip = (filter.Page - 1)*numberOfDays;

                    //Filter Platform
                    if (filter.PlatformId != null)
                    {
                        query = query.Where(s => s.PlatformId == filter.PlatformId);
                    }

                    //Filter Game
                    if (!string.IsNullOrEmpty(filter.Game))
                    {
                        query = query.Where(s => s.Game.GameTitle.Contains(filter.Game));
                    }

                    //Filter Type
                    if (filter.TypeId != null)
                    {
                        query = query.Where(s => s.TypeId == filter.TypeId);
                    }

                    //Filter sessions im in
                    if (filter.SessionsImIn)
                    {
                        query = query.Where(s => s.Members.Any(m => m.UserId == userId));
                    }

                    //Filter sessions "i've" created (my sessions)
                    else if (filter.MySessions)
                    {
                        query = query.Where(s => s.CreatorId == userId);
                    }

                    //Filter sessions with free spaces
                    if (filter.FreeSpaces)
                    {
                        query = query.Where(s => s.Members.Count < s.MembersRequired);
                    }

                    //Filter by specific date
                    if (filter.SpecificDate != null)
                    {
                        DateTime specificDate;
                        if (DateTime.TryParseExact(filter.SpecificDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out specificDate))
                        {
                            query = query.Where(s => DbFunctions.TruncateTime(DbFunctions.AddHours(s.ScheduledDate, timeZoneOffset)) == specificDate);

                            useStartDate = false;
                        }
                    }

                    //Filter by specific time
                    if (filter.SpecificTime != new DateTime())
                    {
                        //Convert filter time to UTC
                        filter.SpecificTime = TimeZoneInfo.ConvertTimeToUtc(filter.SpecificTime, GetUserTimeZone());

                        query = query.Where(s => DbFunctions.CreateTime(s.ScheduledDate.Hour, s.ScheduledDate.Minute, 0) ==
                                             DbFunctions.CreateTime(filter.SpecificTime.Hour, filter.SpecificTime.Minute, 0));
                    }
                }

                #endregion

                //Use start date unless searching for specific time
                if (useStartDate)
                    query = query.Where(s => s.ScheduledDate >= startDate);

                //Get all the sessions that are public and active
                List<SessionDateGroup> groups = await query.Where(s => s.Settings.IsPublic)
                    .GroupBy(x => DbFunctions.TruncateTime(DbFunctions.AddHours(x.ScheduledDate, timeZoneOffset)))
                    .OrderBy(x => x.Key)
                    .Skip(skip)
                    .Take(numberOfDays)
                    .Select(x => new SessionDateGroup
                    {
                        ScheduledDate = x.Key.Value,
                        Sessions = x.Select(s => new SessionListItemViewModel
                        {
                            Id = s.Id,
                            Creator = s.Creator.DisplayName,
                            Game = s.Game.GameTitle,
                            Status = s.Status.Name,
                            StatusDescription = s.Status.Description,
                            ScheduledTime = s.ScheduledDate,
                            Platform = s.Platform.Name,
                            PlatformId = s.PlatformId,
                            Type = s.Type.Name,
                            TypeId = s.TypeId,
                            RequiredCount = s.MembersRequired,
                            MembersCount = s.Members.Count,
                            Duration = s.Duration.Duration
                        })
                            .OrderBy(s => s.ScheduledTime)
                            .ToList(),
                    })
                    .ToListAsync();

                DateTime userNow = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //Convert the DateTimes to the users time zone
                foreach (var g in groups)
                {
                   //If today
                    if (g.ScheduledDate.Date == userNow.Date)
                        g.ScheduledDisplayDate = "Today";
                    //If tomorrrow
                    else if (g.ScheduledDate.Date == userNow.Date.AddDays(1))
                        g.ScheduledDisplayDate = "Tomorrow";
                    //Else
                    else
                        g.ScheduledDisplayDate = g.ScheduledDate.ToFullDateString();

                    foreach (var s in g.Sessions)
                    {
                        s.ScheduledTime = s.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());
                        s.ScheduledDisplayTime = s.ScheduledTime.ToShortTimeString();
                    }
                }

                AllSessionsViewModel viewModel = new AllSessionsViewModel
                {
                    Groups = groups,
                    TotalSessions = groups.Sum(g => g.Sessions.Count)
                };

                if (groups.Any())
                {
                    //Set the start and end date labels
                    viewModel.StartDisplayDate = groups[0].ScheduledDisplayDate;
                    viewModel.EndDisplayDate = groups.LastOrDefault()?.ScheduledDisplayDate;
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting all sessions");
                throw;
            }
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
                        ApproveJoinees = model.ApproveJoinees,
                        MinUserRating = model.MinRatingScore
                    },
                    Goals = new List<SessionGoal>()
                };

                //Build the Goals list
                foreach (var g in model.Goals)
                {
                    //Skip blank/empty goals
                    if (string.IsNullOrWhiteSpace(g)) continue;

                    session.Goals.Add(new SessionGoal {Goal = g});
                }

                //Convert all dates to UTC format
                session.ScheduledDate = TimeZoneInfo.ConvertTimeToUtc(session.ScheduledDate, GetUserTimeZone());
                session.EndTime = session.ScheduledDate.AddMinutes(session.DurationId);

                //Future date check
                if (session.ScheduledDate < DateTime.UtcNow)
                    return VResult.AddError("You must schedule the session to be a future date and time.");

                //Check if this session conflicts with any of the users sessions.
                Session conflictSession = await CheckForSessionConflict(session, userId);

                if (conflictSession != null)
                {
                    DateTime conflictDate = conflictSession.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                    string time = conflictDate.ToString("HH:mm");
                    string date = conflictDate.ToShortDateString();
                    string warningMsg =
                        $"This sessions time overlaps with your session: {conflictSession.Game.GameTitle} ({conflictSession.Platform.Name}) at {time} on {date}. "
                         + "Please pick another date or time.";
                    return VResult.AddError(warningMsg);
                }
                    

                //Add the game to the session
                //Check to see if this game exists in my db
                GameLogic gLogic = new GameLogic(UoW);
                var existingGame = await gLogic.ExistingGame(model.IgdbGameId, model.GameTitle);

                //Either use existing gameId or create a new game.
                if (existingGame == null)
                {
                    session.Game = new Game
                    {
                        IgdbGameId = model.IgdbGameId,
                        GameTitle = model.GameTitle
                    };
                }
                else
                {
                    session.GameId = existingGame.Id;
                }


                //Add the intial message to the session messages feed
                _commentLogic = new SessionCommentLogic(UoW);
                _commentLogic.AddSessionCreatedComment(session);

                //Load the UserProfile of the creator to add as a member
                UserProfile creator = await UoW.Repository<UserProfile>().GetByIdAsync(userId);

                //Add the creator as a member of the session
                session.Members.Add(creator);

                //Insert the new session into the db
                _sessionRepo.Insert(session);
                await SaveChangesAsync();

                //Send out invites to users if any
                if (model.InviteRecipients != null && model.InviteRecipients.Any())
                {
                    List<UserProfile> recipients = await UoW.Repository<UserProfile>()
                        .Get(x => model.InviteRecipients.Contains(x.DisplayName))
                        .Include(x => x.User)
                        .Include(x => x.Preferences)
                        .ToListAsync();

                    //Send Email
                    EmailLogic email = new EmailLogic();
                    await email.SessionInviteEmail(session.Id, model.GameTitle, creator.DisplayName, recipients);

                    //Send Notification
                    NotificationLogic nLogic = new NotificationLogic(UoW);
                    await nLogic.SessionInviteNotification(session, creator.UserId, recipients);
                }

                //Add session Id to VResult data
                VResult.Data = session.Id;

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to create session");   
                return VResult.AddError("Unable to create the session at this time. " +
                                        "Please try again later.");
            }
        }

        public async Task<ValidationResult> EditSession(EditSessionVM viewModel, string userId)
        {
            try
            {
                UserId = userId;

                //Load the session from the db
                Session session = await _sessionRepo.Get(x => x.Id == viewModel.SessionId)
                    .Include(x => x.Members)
                    .Include(x => x.Game)
                    .Include(x => x.Goals)
                    .Include(x => x.Settings)
                    .FirstOrDefaultAsync();

                //Convert viewModel date to UTC
                var newUtcDate = CombineDateAndTime(viewModel.ScheduledDate, viewModel.ScheduledTime);
                newUtcDate = TimeZoneInfo.ConvertTimeToUtc(newUtcDate, GetUserTimeZone());

                bool gameChanged = session.Game.GameTitle != viewModel.GameTitle;
                bool dateChanged = session.ScheduledDate != newUtcDate;
                bool platformChanged = session.PlatformId != viewModel.PlatformId;
                bool typeChanged = session.TypeId != viewModel.TypeId;

                //If the game has changed look for an existing local record
                if (gameChanged)
                {
                    GameLogic gLogic = new GameLogic(UoW);
                    var existingGame = await gLogic.ExistingGame(viewModel.IgdbGameId, viewModel.GameTitle);

                    //Either use existing gameId or create a new game.
                    if (existingGame == null)
                    {
                        session.Game = new Game
                        {
                            IgdbGameId = viewModel.IgdbGameId,
                            GameTitle = viewModel.GameTitle
                        };
                    }
                    else
                    {
                        session.GameId = existingGame.Id;
                    }
                }

                //Map properties from view model
                if (dateChanged)
                {
                    session.ScheduledDate = newUtcDate;
                    session.EndTime = session.ScheduledDate.AddMinutes(viewModel.DurationId);
                }

                if(platformChanged)
                    session.PlatformId = viewModel.PlatformId;

                if (typeChanged)
                    session.TypeId = viewModel.TypeId;

                
                //Update goals
                var goalsRepo = UoW.Repository<SessionGoal>();
                foreach (var g in session.Goals.ToList())
                {
                    goalsRepo.Delete(g);
                }
                foreach (var g in viewModel.Goals)
                {
                    //Skip blank/empty goals
                    if (string.IsNullOrWhiteSpace(g)) continue;
                    session.Goals.Add(new SessionGoal { Goal = g });
                }

                //Update GamersRequired
                if (viewModel.GamersRequired < session.Members.Count)
                {
                    //We have more members that we need
                    return VResult.AddError("You already have more gamers than you have stated you need to complete this session. " +
                            $"You must select a minimum of {session.Members.Count} in gamers needed field.");
                }

                session.MembersRequired = viewModel.GamersRequired;
                session.Information = viewModel.Information;
                session.DurationId = viewModel.DurationId;
                session.Settings.IsPublic = viewModel.IsPublic;
                session.Settings.MinUserRating = viewModel.MinRatingScore;

                //Update the db
                _sessionRepo.Update(session);
                await SaveChangesAsync();

                //If anything important has changed
                if (gameChanged || dateChanged || platformChanged || typeChanged)
                {
                    //Email members about changes (ignore creator)
                    if (session.Members.Count > 1)
                    {
                        //Load the members this time with their email preferences
                        List<UserProfile> members = await _sessionRepo.Get(x => x.Id == viewModel.SessionId)
                            .SelectMany(x => x.Members)
                            .Include(x => x.Preferences)
                            .ToListAsync();

                        EmailLogic eLogic = new EmailLogic();
                        await eLogic.SessionEditedEmail(session, members, gameChanged, dateChanged, platformChanged, typeChanged);
                    }
                }

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating changes for session " + viewModel.SessionId);
                return VResult.AddError("Something went wrong when updating your session. Please try again later");
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
            viewModel.GamersRequiredList = GetGamersRequiredOptions();

            return viewModel;
        } 

        /// <summary>
        /// Get the details of a session and set up the edit view model
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<EditSessionVM> EditSessionVM(Guid sessionId, string userId)
        {
            try
            {
                UserId = userId;

                //Load and project the session from the db
                EditSessionVM viewModel = await _sessionRepo.Get(x => x.Id == sessionId)
                    .Where(x => x.CreatorId == userId) // Additional check so only owner can edit session
                    .Select(x => new EditSessionVM
                    {
                        Status = x.Status.Name,
                        StatusDescription = x.Status.Description,
                        SessionId = x.Id,
                        GameTitle = x.Game.GameTitle,
                        IgdbGameId = x.Game.IgdbGameId,
                        ScheduledDate = x.ScheduledDate,
                        ScheduledTime = x.ScheduledDate,
                        PlatformId = x.PlatformId,
                        TypeId = x.TypeId,
                        Goals = x.Goals.Select(g => g.Goal).ToList(),
                        GamersRequired = x.MembersRequired,
                        Information = x.Information,
                        DurationId = x.DurationId,
                        IsPublic = x.Settings.IsPublic,
                        ApproveJoinees = x.Settings.ApproveJoinees,
                        MinRatingScore = x.Settings.MinUserRating
                    }).FirstOrDefaultAsync();

                //Make sure we have found something
                if(viewModel == null) return null;

                //Convert the DateTimes to the users time zone
                viewModel.ScheduledDate = viewModel.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                viewModel.ScheduledTime = viewModel.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());

                //Add the select lists options
                viewModel.DurationList = await _durationLogic.GetDurationSelectList();
                viewModel.GamersRequiredList = GetGamersRequiredOptions();

                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get edit session vm for session : " + sessionId);
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
        /// session conflicts with the duration of them.
        /// </summary>
        /// <param name="newSession"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<Session> CheckForSessionConflict(Session newSession, string userId)
        {
            //Calculate the end date/time 
            DateTime startDate = newSession.ScheduledDate;
            DateTime endDate = newSession.EndTime;

            //Check for any user session that occurs between this new sessions start time
            Session conflictSession = await _sessionRepo.Get(x => x.CreatorId == userId)
                .Where(x => (x.ScheduledDate <= startDate && x.EndTime > startDate)//New session in the middle of existing
                || (x.ScheduledDate >= startDate && x.EndTime < endDate)//Existing session in the middle of start and end
                || (x.ScheduledDate >= startDate && x.ScheduledDate < endDate))//Existing session starts during
                .Include(x => x.Game)
                .Include(x => x.Platform)
                .FirstOrDefaultAsync();

            return conflictSession;
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

        public async Task<SelectList> GetPlatformsList()
        {
            try
            {
                var platforms = await UoW.Repository<Platform>().Get().ToListAsync();

                return new SelectList(platforms, "Id", "Name");
            }
            catch (Exception ex)
            {
                LogError(ex, "GetPlatformList method caused an error");
                return null;
            }
        }

        public async Task<SelectList> GetTypesList()
        {
            try
            {
                var types = await UoW.Repository<SessionType>().Get().ToListAsync();

                return new SelectList(types, "Id", "Name");
            }
            catch (Exception ex)
            {
                LogError(ex, "GetTypesList method caused an error");
                return null;
            }
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
                    .Include(x => x.Settings)
                    .FirstOrDefaultAsync();

                if (targetSession == null)
                    return VResult.AddError("Unable to join this session as this time");

                //Check the session is active and that there is room to join
                if (!targetSession.Active)
                    return VResult.AddError("This session is no longer active");

                 if(targetSession.MembersCount >= targetSession.MembersRequired)
                    return VResult.AddError("You are unable to join this session as it's full. Why not create a new session?");

                //Check this user isn't already a member of the session.
                bool existingUser = targetSession.Members.Any(x => x.UserId == userId);

                //If we found a matching user return error;
                if (existingUser)
                    return VResult.AddError("You are already a member of this session");

                //Load the UserProfile of the member wanting to join to add to the session
                UserProfile user = await UoW.Repository<UserProfile>().GetByIdAsync(userId);

                //Check the user has the minimum rating required
                if (user.Rating < targetSession.Settings.MinUserRating)
                    return VResult.AddError("Unfortunately your user rating is lower than the required minimum rating for this session. " +
                                            "Refer to the <b>\"User rating help\"</b> to learn how to improve your user rating.");
                    
                //Check that this session doesn't conflict with the users existing sessions
                Session conflictSession = await CheckForSessionConflict(targetSession, userId);

                if (conflictSession != null)
                {
                    DateTime conflictDate = conflictSession.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                    string time = conflictDate.ToString("HH:mm");
                    string date = conflictDate.ToShortDateString();
                    string warningMsg =
                        $"This sessions time overlaps with a session you have joined: {conflictSession.Game.GameTitle} ({conflictSession.Platform.Name}) at {time} on {date}. "
                         + "You are unable to join this session as a result.";
                    return VResult.AddError(warningMsg);
                }

                targetSession.Members.Add(user);

                //Check if the status needs updating
                CheckSessionStatus(targetSession);

                //Add 'User joined' message to session feed
                _commentLogic = new SessionCommentLogic(UoW);
                var comment = _commentLogic.AddUserJoinedComment(targetSession.Id, user.DisplayName, user.UserId);

                if(comment != null)
                    targetSession.Comments.Add(comment);

                //Send notification to the session owner
                NotificationLogic nLogic = new NotificationLogic(UoW);
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