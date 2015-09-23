using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionLogic : BaseLogic, IBusinessLogic<Session>
    {
        #region Variables

        //Session Repository
        private readonly GenericRepository<Session> _sessionRepo;

        //Business Logics
        private readonly SessionDurationLogic _durationLogic;
        private readonly SessionTypeLogic _typeLogic;
        private readonly PlatformLogic _platformLogic;

        #endregion

        #region Constructor

        public SessionLogic()
        {
            _sessionRepo = UoW.Repository<Session>();

            _durationLogic = new SessionDurationLogic();
            _typeLogic = new SessionTypeLogic();
            _platformLogic = new PlatformLogic();
        }

        #endregion

        #region CRUD Operations

        public async Task<List<Session>> GetAll()
        {
            return await _sessionRepo.Get().Where(s => s.Settings.IsPublic).ToListAsync();
        }

        public Session GetById(object id)
        {
            return _sessionRepo.GetById(id);
        }
        
        public async Task<Session> GetByIdAsync(object id)
        {
            return await _sessionRepo.GetByIdAsync(id);
        }

        public async Task<bool> CreateSession(CreateSessionVM viewModel)
        {
            try
            {
                //Map the properties from view model to model
                Session model = Mapper.Map<CreateSessionVM, Session>(viewModel);
                model.Settings = Mapper.Map<CreateSessionVM, SessionSettings>(viewModel);

                //Combine both date and time fields
                model.ScheduledDate = CombineDateAndTime(model.ScheduledDate, viewModel.ScheduledTime);

                //Insert the new session into the db
                _sessionRepo.Insert(model);
                await SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to create session");   
                return false;
            }
        }

        public async Task<bool> EditSession(EditSessionVM viewModel)
        {
            try
            {
                //Load the session from the db
                Session model = await GetByIdAsync(viewModel.SessionId);

                //Map the changes (top-tier & 2nd level)
                Mapper.Map(viewModel, model);
                Mapper.Map(viewModel, model.Settings);

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
        /// <returns></returns>
        public async Task<CreateSessionVM> PrepareCreateSessionVM(CreateSessionVM viewModel)
        {
            //Set the default time if we don't already have one
            if (viewModel.ScheduledTime == new DateTime())
                viewModel.ScheduledTime = SetDefaultSessionTime();

            //Add the select lists options
            viewModel.DurationList = await _durationLogic.GetDurationSelectList();
            viewModel.SessionTypeList = await _typeLogic.GetTypeSelectList();
            viewModel.PlatformList = await _platformLogic.GetPlatformSelectList();

            //Get the time slots list
            viewModel.ScheduledTimeList = GetTimeSlots();

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
        /// Prepares the view model used to view a session ready to be passed the view
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<SessionDetailsVM> PrepareViewSessionVM(Guid sessionId)
        {
            try
            {
                //Load the session from the db
                Session model = await GetByIdAsync(sessionId);

                if (model == null) return null;

                //Map the properties to the view model
                var viewModel = Mapper.Map<Session, SessionDetailsVM>(model);


                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
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
                        ScheduledTime = x.ScheduledDate.ToString(),
                        PlatformId = x.PlatformId,
                        TypeId = x.TypeId,
                        GamersRequired = x.GamersRequired.ToString(),
                        Information = x.Information,
                        DurationId = x.DurationId,
                        IsPublic = x.Settings.IsPublic,
                        ApproveJoinees = x.Settings.ApproveJoinees

                    }).FirstOrDefaultAsync();

                //Make sure we have found something
                if(viewModel == null) return null;

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
        /// Returns 24 hours worth of times slots rounded to the nearest 15 mins
        /// </summary>
        /// <returns>Select list of times</returns>
        private SelectList GetTimeSlots()
        {
            List<string> times = new List<string>();

            for (int i = 0; i < 24; i++)
            {
                times.Add(i + ":00");
                times.Add(i + ":15");
                times.Add(i + ":30");
                times.Add(i + ":45");
            }

            return new SelectList(times);
        }

        /// <summary>
        /// Returns a select list of options for the required amount of gamers
        /// </summary>
        /// <returns></returns>
        private SelectList GetGamersRequiredOptions()
        {
            //Anon object used to hold id and value (for the unlimited option)
            var options = new List<object>();

            //Add options 1 - 24
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
        /// Sets the default value for the session time (+1 hour, rounded to the nearest 15 mins)
        /// </summary>
        /// <returns></returns>
        private DateTime SetDefaultSessionTime()
        {
            var time = DateTime.Now.AddHours(1);
            var dif = TimeSpan.FromMinutes(15);
            return new DateTime(((time.Ticks + dif.Ticks - 1)/dif.Ticks)*dif.Ticks);
        }

        /// <summary>
        /// Combines the date and time inputs entered when creating a session to form a single DateTime object
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private DateTime CombineDateAndTime(DateTime date, DateTime time)
        {
            //Convert time string to timespan
            //TimeSpan ts = DateTime.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            TimeSpan ts = time.TimeOfDay;

            DateTime newDateTime = date + ts;

            return newDateTime;
        }

        #endregion
    }
}