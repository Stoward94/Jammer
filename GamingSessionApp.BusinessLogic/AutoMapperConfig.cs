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

                config.CreateMap<Session, CreateSessionViewModel>()
                    .ReverseMap();

                config.CreateMap<Session, SessionDetailsViewModel>()
                    .ForMember(dest => dest.Platform, opt => opt.MapFrom(src => src.Platform.Name)) //Map platform name
                    .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.UserName)) //Map the creator username
                    .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration.Duration)) //Map the duration value
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name)) //Map the type name
                    .ReverseMap();

                #endregion

            });
        }
    }
}