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
                config.CreateMap<Session, CreateSessionViewModel>()
                    .ReverseMap();

                //config.CreateMap<Project, ProjectDetailsViewModel>()
                //    .ForMember(dest => dest.ClientName,
                //        opt => opt.MapFrom(src => src.Client.Name))
                //    .ReverseMap();

                //config.CreateMap<IncidentReport, CreateEditRequestViewModel>()
                //    .ReverseMap();
            });
        }
    }
}
