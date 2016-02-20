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
            //Seed the session email reminder durations
            var reminderTimes = new List<EmailReminderTime>
            {
                new EmailReminderTime {  Duration = "Session start", Minutes = 0 },
                new EmailReminderTime {  Duration = "1 mintute", Minutes = 1 },
                new EmailReminderTime {  Duration = "15 minutes", Minutes = 15 },
                new EmailReminderTime {  Duration = "30 minutes", Minutes = 30 },
                new EmailReminderTime {  Duration = "45 minutes", Minutes = 45 },
                new EmailReminderTime {  Duration = "1 hour", Minutes = 60 },
                new EmailReminderTime {  Duration = "2 hours", Minutes = 120 },
                new EmailReminderTime {  Duration = "3 hours", Minutes = 180 }
            };

            reminderTimes.ForEach(r => context.EmailReminderTimes.AddOrUpdate(m => m.Minutes, r));

            context.SaveChanges();

            var user = new ApplicationUser();

            //Seed the Master User if required
            if (!context.Users.Any())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                user = new ApplicationUser
                {
                    UserName = "Stoward94",
                    Email = "luke_stoward@hotmail.co.uk",
                    TimeZoneId = "GMT Standard Time",
                    Profile = new UserProfile
                    {
                        DisplayName = "Stoward94",
                        ThumbnailUrl = "/Images/thumbnails/default/001.png",
                        Kudos = new Kudos(),
                        Preferences = new UserPreferences()
                    }
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
            context.SessionStatuses.AddOrUpdate(x => x.Name, 
                new SessionStatus { Name = "Recruiting", Description = "This session is still in need of players to join and play" },
                new SessionStatus { Name = "Fully Loaded", Description = "This session is currently full, however this may change before the session begins" },
                new SessionStatus { Name = "Jamming", Description = "This session is actively jamming (reached the start time, but not yet exceeded the expected duration)" },
                new SessionStatus { Name = "Retired", Description = "This session has now been completed" }
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

            //Seed the User Notification Types
            var userNotificationTypes = new List<string>
            {
                "Player Joined",
                "Player Left",
                "Kudos Added",
                "Information",
                "Invitation"
            };

            userNotificationTypes.ForEach(t => context.UserNotificationTypes.AddOrUpdate(x => x.Name, new UserNotificationType { Name = t }));

            context.SaveChanges();

            //Seed sample sessions
            if (!context.Sessions.Any())
            {
                
                Session session = new Session
                {
                    PlatformId = 2,
                    CreatorId = user.Id,
                    DurationId = 3,
                    MembersRequired = 4,
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
                        new SessionMessage() { AuthorId = user.Id, Body = "Luke Joined", MessageNo = 2, MessageTypeId = 2 }
                    }
                };

                //Need to load user as profile is null
                UserProfile profile = context.UserProfiles.First(x => x.UserId == user.Id);

                session.Members.Add(profile);

                context.Sessions.AddOrUpdate(x => x.PlatformId, session);
            }
            context.SaveChanges();
        }
    }
}
