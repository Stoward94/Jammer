using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamingSessionApp.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
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
                    UserName = "luke.stoward",
                    Email = "luke@email.com",
                };
                um.Create(user, "Password");
                context.SaveChanges();
            }

            //Seed the SessionDuration Values
            var durationValues = new List<string>
            {
                "0-30 mins",
                "30-60 mins",
                "1-2 hours",
                "2-3 hours",
                "3-4 hours",
                "4-5 hours",
                "5+ hours"
            };

            durationValues.ForEach(c => context.SessionDurations.AddOrUpdate(x => x.Duration, new SessionDuration { Duration = c }));

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

            context.SaveChanges();

            //Seed sample sessions
            if (!context.Sessions.Any())
            {
                context.Sessions.AddOrUpdate(x => x.PlatformId,
                    new Session
                    {
                        CreatedDate = DateTime.Now,
                        PlatformId = 2,
                        CreatorId = user.Id,
                        DurationId = 3,
                        GamersRequired = 4,
                        Information = "This is the first session",
                        ScheduledDate = DateTime.Now.AddDays(12),
                        TypeId = 1,
                        Settings = new SessionSettings
                        {
                            IsPublic = true,
                            ApproveJoinees = false
                        }
                    });
            }
        }
    }
}
