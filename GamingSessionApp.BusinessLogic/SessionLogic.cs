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
            return await _sessionRepo.Get().Where(s => s.IsPublic).ToListAsync();
        }

        public Session GetById(int id)
        {
            return _sessionRepo.GetById(id);
        }

        public async Task<Session> GetByIdAsync(int id)
        {
            return await _sessionRepo.GetByIdAsync(id);
        }

        public async Task<bool> CreateSession(CreateSessionViewModel viewModel)
        {
            try
            {
                //Map the properties from view model to model
                Session model = Mapper.Map<CreateSessionViewModel, Session>(viewModel);

                //Combine both date and time fields
                model.ScheduledDate = CombineDateAndTime(model.ScheduledDate, viewModel.ScheduledTime);

                //Insert the new session into the db
                _sessionRepo.Insert(model);
                await SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log exception    
                return false;
            }
        }

        #endregion

        #region Prepare View Models

        public async Task<CreateSessionViewModel> PrepareCreateSessionViewModel(CreateSessionViewModel viewModel)
        {
            //Set the default time if we don't already have one
            if(String.IsNullOrEmpty(viewModel.ScheduledTime))
                viewModel.ScheduledTime = SetDefaultSessionTime();

            //Add the select lists options
            List<SessionDuration> durationList = await _durationLogic.GetAll();
            List<SessionType> typeList = await _typeLogic.GetAll();
            List<Platform> platformList = await _platformLogic.GetAll();


            viewModel.DurationList = new SelectList(durationList, "Id", "Duration");
            viewModel.SessionTypeList = new SelectList(typeList, "Id", "Name");
            viewModel.PlatformList = new SelectList(platformList, "Id", "Name");

            //Get the time slots list
            viewModel.ScheduledTimeList = GetTimeSlots();

            return viewModel;
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
        /// Sets the default value for the session time
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private string SetDefaultSessionTime()
        {
            var time = DateTime.Now.AddHours(1);
            var dif = TimeSpan.FromMinutes(15);
            return new DateTime(((time.Ticks + dif.Ticks - 1) / dif.Ticks) * dif.Ticks).ToShortTimeString();
        }

        private DateTime CombineDateAndTime(DateTime date, string time )
        {
            //Convert time string to timespan
            TimeSpan ts = DateTime.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

            DateTime newDateTime = date + ts;

            return newDateTime;
        }

        #endregion
    }
}
