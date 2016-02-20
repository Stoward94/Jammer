using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.BusinessLogic
{
    public static class AutoMapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.Initialize(config =>
            {
                #region Session Maps

                

                config.CreateMap<SessionSettings, CreateSessionVM>()
                    .ReverseMap();
            
                //View Session
                config.CreateMap<Session, SessionDetailsVM>()
                    .ForMember(dest => dest.Platform, opt => opt.MapFrom(src => src.Platform.Name)) //Map platform name
                    .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.DisplayName)) //Map the creator username
                    .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration.Duration)) //Map the duration value
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name)) //Map the type name
                    .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Settings.IsPublic)) //Map the settings
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name)) //Map the settings
                    .ReverseMap();

                //Edit Session (Used to map the changes from the view model to the model when editing a session)
                config.CreateMap<EditSessionVM, Session>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SessionId));

                //Edit Session Settings (Used to map the changes from the view model to the model when editing a session)
                config.CreateMap<EditSessionVM, SessionSettings>();



                //Mapper.AssertConfigurationIsValid();

                #endregion
            });
        }
    }
}