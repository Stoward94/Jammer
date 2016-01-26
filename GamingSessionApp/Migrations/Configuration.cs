using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var user = new ApplicationUser();

            //Seed the Master User if required
            if (!context.Users.Any())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                user = new ApplicationUser
                {
                    UserName = "luke@email.com",
                    Email = "luke@email.com",
                    TimeZoneId = "GMT Standard Time"
                };
                um.Create(user, "Password");
                context.SaveChanges();
            }

            //Seed the SessionDuration Values
            context.SessionDurations.AddOrUpdate(d => d.Minutes,
                new SessionDuration {Duration = "0 - 30 Minutes", Minutes = 30 },
                new SessionDuration {Duration = "30 - 60 Minutes", Minutes = 60 },
                new SessionDuration {Duration = "1 - 2 Hours", Minutes = 120 },
                new SessionDuration {Duration = "2 - 3 Hours", Minutes = 180 },
                new SessionDuration {Duration = "3 - 4 Hours", Minutes = 240 },
                new SessionDuration {Duration = "4 - 5 Hours", Minutes = 300 },
                new SessionDuration {Duration = "5+ hours", Minutes = 301 }
                );

            //Seed the SessionStatus Statuses
            context.SessionStatuses.AddOrUpdate(x => x.Status, 
                new SessionStatus { Status = "Recruiting", Description = "This session is still in need of players to join and play" },
                new SessionStatus { Status = "Fully Loaded", Description = "This session is currently full, however this may change before the session begins" },
                new SessionStatus { Status = "Jamming", Description = "This session is actively jamming (reached the start time, but not yet exceeded the expected duration)" },
                new SessionStatus { Status = "Retired", Description = "This session has now been completed" }
                );

            //Seed the SessionType Descriptions
            var sessionTypes = new List<string>
            {
                "Boosting",
                "Co-op",
                "Competitive",
                "Clan match",
                "Casual play",
                "Achievement hunting"
            };

            sessionTypes.ForEach(c => context.SessionTypes.AddOrUpdate(x => x.Name, new SessionType { Name = c }));

            //Seed the Platforms
            var platforms = new List<string>
            {
                "PC",
                "Xbox 360",
                "Xbox One",
                "Play Station 3",
                "Play Station 4",
                "Wii",
                "Wii U",
                "iOS",
                "Android"
            };

            platforms.ForEach(c => context.Platforms.AddOrUpdate(x => x.Name, new Platform { Name = c }));

            //Seed the Session Feed Message Types
            var feedMessageTypes = new List<string>
            {
                "System",
                "Player Joined",
                "Player Left",
                "Comment",
                "Invitation"
            };

            feedMessageTypes.ForEach(c => context.SessionMessageTypes.AddOrUpdate(x => x.Type, new SessionMessageType { Type = c }));

            context.SaveChanges();

            //Seed sample sessions
            if (!context.Sessions.Any())
            {
                context.Sessions.AddOrUpdate(x => x.PlatformId,
                    new Session
                    {
                        CreatedDate = DateTime.UtcNow,
                        PlatformId = 2,
                        CreatorId = user.Id,
                        DurationId = 3,
                        GamersRequired = 4,
                        Information = "This is the first session",
                        ScheduledDate = DateTime.UtcNow.AddDays(12),
                        TypeId = 1,
                        StatusId = (int)SessionStatusEnum.Recruiting,
                        Settings = new SessionSettings
                        {
                            IsPublic = true,
                            ApproveJoinees = false
                        },
                        Messages = new List<SessionMessage>()
                        {
                            new SessionMessage() { AuthorId = user.Id, Body = "Session Created", MessageNo = 1, MessageTypeId = 1 },
                            new SessionMessage() { AuthorId = user.Id, Body = "Luke Joined", MessageNo = 2, MessageTypeId = 2 },
                            new SessionMessage() { AuthorId = user.Id, Body = "Luke Left", MessageNo = 3, MessageTypeId = 3 },
                            
                        }
                    });
            }
            context.SaveChanges();
        }
    }
}
