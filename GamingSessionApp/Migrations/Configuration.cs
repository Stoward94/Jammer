using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
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

            var usernames = new List<string> {"Stoward94", "hazfraz007", "Sensei Neo", "8Thrills", "Straight8Shot"};
            var emails = new List<string> { "luke_stoward@hotmail.co.uk", "email1@email.com", "email2@email.com", "email3@email.com", "email4@email.com" };

            //Seed the users if required
            if (!context.Users.Any())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                for (int i = 0; i < usernames.Count; i++)
                {
                    var user = new ApplicationUser
                    {
                        UserName = usernames[i],
                        Email = emails[i],
                        TimeZoneId = "GMT Standard Time",
                        DateRegistered = DateTime.UtcNow,
                        LastSignIn = DateTime.UtcNow,
                        Profile = new UserProfile
                        {
                            DisplayName = usernames[i],
                            ThumbnailUrl = "/Images/thumbnails/default/00" + i + ".png",
                            Website = "https://www.triggerwars.com",
                            Kudos = new Kudos(),
                            Preferences = new UserPreferences(),
                            Statistics = new UserStatistics(),
                            Social = new UserSocial
                            {
                                Xbox = "British Legends",
                                XboxUrl = "account.xbox.com/en-GB/Profile?gamerTag=British+Legend",
                                Steam = "MonsterAchievements",
                                Facebook = "Luke Stoward",
                                PlayStation = "Stoward94",
                                Twitter = "TriggerWars",
                            }
                        }
                    };
                    var result = um.Create(user, "Password");


                    if (result.Succeeded)
                        context.SaveChanges();
                }
            }

            //Seed the SessionDuration Values
            context.SessionDurations.AddOrUpdate(d => d.Id,
                new SessionDuration { Id = 30, Duration = "30 minutes or less", Minutes = 30 },
                new SessionDuration { Id = 60, Duration = "30 - 60 minutes", Minutes = 60 },
                new SessionDuration { Id = 120, Duration = "1 - 2 hours", Minutes = 120 },
                new SessionDuration { Id = 180, Duration = "2 - 3 hours", Minutes = 180 },
                new SessionDuration { Id = 240, Duration = "3 - 4 hours", Minutes = 240 },
                new SessionDuration { Id = 300, Duration = "4 - 5 hours", Minutes = 300 },
                new SessionDuration { Id = 301, Duration = "5+ hours", Minutes = 301 }
                );

            //Seed the SessionStatus Statuses
            context.SessionStatuses.AddOrUpdate(x => x.Name, 
                new SessionStatus { Name = "Open", Description = "This session is still in need of players to join and play" },
                new SessionStatus { Name = "Full", Description = "This session is currently full, however this may change before the session begins" },
                new SessionStatus { Name = "In Progress", Description = "This session is currently in progress (reached the start time, but not yet exceeded the expected duration)" },
                new SessionStatus { Name = "Completed", Description = "This session has now been completed" }
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
                "PlayStation 2",
                "PlayStation 3",
                "PlayStation 4",
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

            feedMessageTypes.ForEach(c => context.SessionMessageTypes.AddOrUpdate(x => x.Type, new SessionCommentType { Type = c }));

            //Seed the User Notification Types
            var userNotificationTypes = new List<string>
            {
                "Player Joined",
                "Player Left",
                "Kudos Added",
                "Information",
                "Invitation",
                "Comment"
            };

            userNotificationTypes.ForEach(t => context.UserNotificationTypes.AddOrUpdate(x => x.Name, new UserNotificationType { Name = t }));

            context.SaveChanges();

            //Seed award levels
            var levels = new List<string>
            {
                "Beginner",
                "Novice",
                "Intermediate",
                "Advanced",
                "Expert"
            };

            levels.ForEach(l => context.AwardLevels.AddOrUpdate(x => x.Level, new AwardLevel {Level = l}));

            //Seed award groups
            var awardGroups = new List<string>
            {
                "Sessions Created",
                "Sessions Completed",
                "Donation",
                "Rating",
                "Kudos",
                "Unique"
            };

            awardGroups.ForEach(g => context.AwardGroups.AddOrUpdate(x => x.Group, new AwardGroup { Group = g }));

            context.SaveChanges();


            //Seed the user awards
            var awards = new List<Award>
            {
                new Award { Title = "1st Session Created", Description = "Awarded for creating your 1st session", LevelId = 1, GroupId = 1, Requirement = 1, Slug = "award-created-1" },
                new Award { Title = "10 Sessions Created", Description = "Awarded for creating your 10th session", LevelId = 2, Requirement = 10, GroupId = 1, Slug = "award-created-10" },
                new Award { Title = "100 Sessions Created", Description = "Awarded for creating your 100th session", LevelId = 3, Requirement = 100, GroupId = 1, Slug = "award-created-100" },
                new Award { Title = "250 Sessions Created", Description = "Awarded for creating your 250th session", LevelId = 4, Requirement = 250, GroupId = 1, Slug = "award-created-250" },
                new Award { Title = "1000 Sessions Created", Description = "Awarded for creating your 1000th session", LevelId = 5, Requirement = 1000, GroupId = 1, Slug = "award-created-1000" },

                new Award { Title = "1st Session Completed", Description = "Awarded for completing your 1st session", LevelId = 1, Requirement = 1, GroupId = 2, Slug = "award-completed-1" },
                new Award { Title = "10 Sessions Completed", Description = "Awarded for completing your 10th session", LevelId = 2, Requirement = 10, GroupId = 2, Slug = "award-completed-10" },
                new Award { Title = "100 Sessions Completed", Description = "Awarded for completing your 100th session", LevelId = 3, Requirement = 100, GroupId = 2, Slug = "award-completed-100" },
                new Award { Title = "250 Sessions Completed", Description = "Awarded for completing your 250th session", LevelId = 4, Requirement = 250, GroupId = 2, Slug = "award-completed-250" },
                new Award { Title = "1000 Sessions Completed", Description = "Awarded for completing your 1000th session", LevelId = 5, Requirement = 1000, GroupId = 2, Slug = "award-completed-1000" },

                //Donations
                new Award { Title = "Donation Supporter", Description = "Awarded for donating to help run TriggerWars", LevelId = 4, Requirement = 1, GroupId = 3, Slug = "award-donated" },
                new Award { Title = "Superior Donation Supporter", Description = "Awarded for a significant donation to help run TriggerWars", LevelId = 5, Requirement = 20, GroupId = 3, Slug = "award-donated-superior" },

                //Rating
                new Award { Title = "9 Star Average Feedback Rating", Description = "Awarded for having an average rating of 9+", LevelId = 5, Requirement = 9, GroupId = 4, Slug = "award-feedback-9" },
                new Award { Title = "8 Star Average Feedback Rating", Description = "Awarded for having an average rating of 8+", LevelId = 4, Requirement = 8, GroupId = 4, Slug = "award-feedback-8" },
                new Award { Title = "7 Star Average Feedback Rating", Description = "Awarded for having an average rating of 7+", LevelId = 3, Requirement = 7, GroupId = 4, Slug = "award-feedback-7" },
                new Award { Title = "6 Star Average Feedback Rating", Description = "Awarded for having an average rating of 6+", LevelId = 2, Requirement = 6, GroupId = 4, Slug = "award-feedback-6" },
                new Award { Title = "5 Star Average Feedback Rating", Description = "Awarded for having an average rating of 5+", LevelId = 1, Requirement = 5, GroupId = 4, Slug = "award-feedback-5" },

                //Kudos
                new Award { Title = "100 Kudos Earned", Description = "Awarded for achieving 100 total Kudos", LevelId = 1, Requirement = 100, GroupId = 5, Slug = "award-kudos-100" },
                new Award { Title = "1,000 Kudos Earned", Description = "Awarded for achieving 1,000 total Kudos", LevelId = 2, Requirement = 1000, GroupId = 5, Slug = "award-kudos-1000" },
                new Award { Title = "10,000 Kudos Earned", Description = "Awarded for achieving 10,000 total Kudos", LevelId = 3, Requirement = 10000, GroupId = 5, Slug = "award-kudos-10000" },
                new Award { Title = "50,000 Kudos Earned", Description = "Awarded for achieving 50,000 total Kudos", LevelId = 4, Requirement = 50000, GroupId = 5, Slug = "award-kudos-50000" },
                new Award { Title = "100,000 Kudos Earned!", Description = "Awarded for achieving a staggering 100,000 total Kudos", LevelId = 5, Requirement = 100000, GroupId = 5, Slug = "award-kudos-100000" },

                //Crowdfunder award
                new Award { Title = "I Crowdfunded TriggerWars!", Description = "A Unique award, awarded only to those that crowdfunded TriggerWars", LevelId = 5, GroupId = 6, Slug = "award-crowdfunded" }
            };

            awards.ForEach(award => context.Awards.AddOrUpdate(x => x.Title, award));

            //Seed Games
            context.Games.AddOrUpdate(x => x.GameTitle, 
                new Game {GameTitle = "Rise of the Tomb Raider", IgdbGameId = 7323},
                new Game {GameTitle = "Battlefield 2", IgdbGameId = 277},
                new Game {GameTitle = "The Walking Dead - 400 Days", IgdbGameId = 3015},
                new Game {GameTitle = "LOST", IgdbGameId = 10226},
                new Game {GameTitle = "Gears of War 4", IgdbGameId = 11186},
                new Game {GameTitle = "Rare Replay", IgdbGameId = 11147},
                new Game {GameTitle = "Dark Souls III", IgdbGameId = 11133});

            context.SaveChanges();

            //Seed sample sessions
            if (!context.Sessions.Any())
            {
                var users = context.UserProfiles.ToList();
                var count = users.Count;

                var index = 0;

                for (int i = 1; i < 20; i++)
                {
                    if (index == count)
                        index = 0;

                    var creator = users[index];

                    Session session = new Session
                    {
                        GameId = i < 8 ? i : 1 ,
                        PlatformId = index + 1,
                        CreatorId = creator.UserId,
                        DurationId = i <= count ? 60*i : 120,
                        MembersRequired = (i + 1),
                        Information =
                            "Looking to boost the multiplayer achievements. Will help anyone who needs any of them",
                        ScheduledDate = DateTime.UtcNow.AddDays(i),
                        EndTime = DateTime.UtcNow.AddDays(i).AddMinutes(120),
                        TypeId = index + 1,
                        StatusId = (int) SessionStatusEnum.Open,
                        Settings = new SessionSettings
                        {
                            IsPublic = true,
                            ApproveJoinees = false
                        },
                        Comments = new List<SessionComment>()
                        {
                            new SessionComment() {AuthorId = creator.UserId, Body = "Session Created", CommentTypeId = 1}
                        }
                    };
                    
                    session.Members.Add(creator);

                    context.Sessions.AddOrUpdate(x => x.PlatformId, session);

                    index++;
                }
            }
            context.SaveChanges();
        }
    }
}
