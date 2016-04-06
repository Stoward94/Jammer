namespace GamingSessionApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AwardLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Awards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        LevelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAwards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        AwardId = c.Int(nullable: false),
                        DateAwarded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Awards", t => t.AwardId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AwardId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(nullable: false, maxLength: 256),
                        ThumbnailUrl = c.String(nullable: false),
                        About = c.String(),
                        Rating = c.Double(nullable: false),
                        Website = c.String(),
                        XboxGamertag = c.String(),
                        XboxUrl = c.String(),
                        PlayStationNetwork = c.String(),
                        PlayStationUrl = c.String(),
                        SteamName = c.String(),
                        SteamUrl = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DisplayName, unique: true);
            
            CreateTable(
                "dbo.SessionFeedbacks",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SessionId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Rating = c.Int(nullable: false),
                        Comments = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.OwnerId)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserId)
                .Index(t => t.SessionId)
                .Index(t => t.UserId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CreatorId = c.String(nullable: false, maxLength: 128),
                        GameId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ScheduledDate = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                        PlatformId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        MembersRequired = c.Int(nullable: false),
                        Information = c.String(nullable: false),
                        DurationId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.CreatorId, cascadeDelete: true)
                .ForeignKey("dbo.SessionDurations", t => t.DurationId, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Platforms", t => t.PlatformId, cascadeDelete: true)
                .ForeignKey("dbo.SessionStatus", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.SessionTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.CreatorId)
                .Index(t => t.GameId)
                .Index(t => t.StatusId)
                .Index(t => t.PlatformId)
                .Index(t => t.TypeId)
                .Index(t => t.DurationId);

            CreateTable(
                "dbo.SessionComments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    SessionId = c.Guid(nullable: false),
                    CommentTypeId = c.Int(nullable: false),
                    AuthorId = c.String(nullable: false, maxLength: 128),
                    CreatedDate = c.DateTime(nullable: false),
                    Body = c.String(nullable: false, maxLength: 2000),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.SessionCommentTypes", t => t.CommentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .Index(t => t.SessionId)
                .Index(t => t.CommentTypeId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.SessionCommentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SessionDurations",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Duration = c.String(nullable: false),
                        Minutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IgdbGameId = c.Int(),
                        GameTitle = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SessionGoals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.Guid(nullable: false),
                        Goal = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.Platforms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SessionSettings",
                c => new
                    {
                        SessionId = c.Guid(nullable: false),
                        Id = c.Int(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        ApproveJoinees = c.Boolean(nullable: false),
                        MinUserRating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SessionId)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.SessionStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SessionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserFriends",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.String(nullable: false, maxLength: 128),
                        FriendId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.FriendId)
                .ForeignKey("dbo.UserProfiles", t => t.ProfileId)
                .Index(t => t.ProfileId)
                .Index(t => t.FriendId);
            
            CreateTable(
                "dbo.Kudos",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Points = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.UserProfiles", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.KudosHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KudosId = c.String(maxLength: 128),
                        Points = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kudos", t => t.KudosId)
                .Index(t => t.KudosId);
            
            CreateTable(
                "dbo.UserMessages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        RecipientId = c.String(nullable: false, maxLength: 128),
                        SenderId = c.String(nullable: false, maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                        Subject = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        Read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.RecipientId)
                .ForeignKey("dbo.UserProfiles", t => t.SenderId, cascadeDelete: true)
                .Index(t => t.RecipientId)
                .Index(t => t.SenderId);
            
            CreateTable(
                "dbo.UserNotifications",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        RecipientId = c.String(nullable: false, maxLength: 128),
                        RefereeId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                        Body = c.String(),
                        TypeId = c.Int(nullable: false),
                        SessionId = c.Guid(nullable: false),
                        CommentId = c.Int(),
                        Read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.RecipientId)
                .ForeignKey("dbo.UserProfiles", t => t.RefereeId)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.UserNotificationTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.RecipientId)
                .Index(t => t.RefereeId)
                .Index(t => t.TypeId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.UserNotificationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPreferences",
                c => new
                    {
                        ProfileId = c.String(nullable: false, maxLength: 128),
                        ReceiveEmail = c.Boolean(nullable: false),
                        ReceiveNotifications = c.Boolean(nullable: false),
                        ReminderTimeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProfileId)
                .ForeignKey("dbo.UserProfiles", t => t.ProfileId)
                .ForeignKey("dbo.EmailReminderTimes", t => t.ReminderTimeId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.ReminderTimeId);
            
            CreateTable(
                "dbo.EmailReminderTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Duration = c.String(),
                        Minutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TimeZoneId = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.SessionMembers",
                c => new
                    {
                        SessionId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SessionId, t.UserId })
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.UserProfiles", t => t.UserId)
                .Index(t => t.SessionId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.UserAwards", "UserId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPreferences", "ReminderTimeId", "dbo.EmailReminderTimes");
            DropForeignKey("dbo.UserPreferences", "ProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserNotifications", "TypeId", "dbo.UserNotificationTypes");
            DropForeignKey("dbo.UserNotifications", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.UserNotifications", "RefereeId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserNotifications", "RecipientId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserMessages", "SenderId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserMessages", "RecipientId", "dbo.UserProfiles");
            DropForeignKey("dbo.Kudos", "UserId", "dbo.UserProfiles");
            DropForeignKey("dbo.KudosHistories", "KudosId", "dbo.Kudos");
            DropForeignKey("dbo.UserFriends", "ProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserFriends", "FriendId", "dbo.UserProfiles");
            DropForeignKey("dbo.SessionFeedbacks", "UserId", "dbo.UserProfiles");
            DropForeignKey("dbo.SessionFeedbacks", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Sessions", "TypeId", "dbo.SessionTypes");
            DropForeignKey("dbo.Sessions", "StatusId", "dbo.SessionStatus");
            DropForeignKey("dbo.SessionSettings", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Sessions", "PlatformId", "dbo.Platforms");
            DropForeignKey("dbo.SessionMembers", "UserId", "dbo.UserProfiles");
            DropForeignKey("dbo.SessionMembers", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.SessionGoals", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Sessions", "GameId", "dbo.Games");
            DropForeignKey("dbo.Sessions", "DurationId", "dbo.SessionDurations");
            DropForeignKey("dbo.Sessions", "CreatorId", "dbo.UserProfiles");
            DropForeignKey("dbo.SessionComments", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.SessionComments", "CommentTypeId", "dbo.SessionCommentTypes");
            DropForeignKey("dbo.SessionComments", "AuthorId", "dbo.UserProfiles");
            DropForeignKey("dbo.SessionFeedbacks", "OwnerId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserAwards", "AwardId", "dbo.Awards");
            DropIndex("dbo.SessionMembers", new[] { "UserId" });
            DropIndex("dbo.SessionMembers", new[] { "SessionId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.UserPreferences", new[] { "ReminderTimeId" });
            DropIndex("dbo.UserPreferences", new[] { "ProfileId" });
            DropIndex("dbo.UserNotifications", new[] { "SessionId" });
            DropIndex("dbo.UserNotifications", new[] { "TypeId" });
            DropIndex("dbo.UserNotifications", new[] { "RefereeId" });
            DropIndex("dbo.UserNotifications", new[] { "RecipientId" });
            DropIndex("dbo.UserMessages", new[] { "SenderId" });
            DropIndex("dbo.UserMessages", new[] { "RecipientId" });
            DropIndex("dbo.KudosHistories", new[] { "KudosId" });
            DropIndex("dbo.Kudos", new[] { "UserId" });
            DropIndex("dbo.UserFriends", new[] { "FriendId" });
            DropIndex("dbo.UserFriends", new[] { "ProfileId" });
            DropIndex("dbo.SessionSettings", new[] { "SessionId" });
            DropIndex("dbo.SessionGoals", new[] { "SessionId" });
            DropIndex("dbo.SessionComments", new[] { "AuthorId" });
            DropIndex("dbo.SessionComments", new[] { "CommentTypeId" });
            DropIndex("dbo.SessionComments", new[] { "SessionId" });
            DropIndex("dbo.Sessions", new[] { "DurationId" });
            DropIndex("dbo.Sessions", new[] { "TypeId" });
            DropIndex("dbo.Sessions", new[] { "PlatformId" });
            DropIndex("dbo.Sessions", new[] { "StatusId" });
            DropIndex("dbo.Sessions", new[] { "GameId" });
            DropIndex("dbo.Sessions", new[] { "CreatorId" });
            DropIndex("dbo.SessionFeedbacks", new[] { "OwnerId" });
            DropIndex("dbo.SessionFeedbacks", new[] { "UserId" });
            DropIndex("dbo.SessionFeedbacks", new[] { "SessionId" });
            DropIndex("dbo.UserProfiles", new[] { "DisplayName" });
            DropIndex("dbo.UserProfiles", new[] { "UserId" });
            DropIndex("dbo.UserAwards", new[] { "AwardId" });
            DropIndex("dbo.UserAwards", new[] { "UserId" });
            DropTable("dbo.SessionMembers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.EmailReminderTimes");
            DropTable("dbo.UserPreferences");
            DropTable("dbo.UserNotificationTypes");
            DropTable("dbo.UserNotifications");
            DropTable("dbo.UserMessages");
            DropTable("dbo.KudosHistories");
            DropTable("dbo.Kudos");
            DropTable("dbo.UserFriends");
            DropTable("dbo.SessionTypes");
            DropTable("dbo.SessionStatus");
            DropTable("dbo.SessionSettings");
            DropTable("dbo.Platforms");
            DropTable("dbo.SessionGoals");
            DropTable("dbo.Games");
            DropTable("dbo.SessionDurations");
            DropTable("dbo.SessionCommentTypes");
            DropTable("dbo.SessionComments");
            DropTable("dbo.Sessions");
            DropTable("dbo.SessionFeedbacks");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.UserAwards");
            DropTable("dbo.Awards");
            DropTable("dbo.AwardLevels");
        }
    }
}
