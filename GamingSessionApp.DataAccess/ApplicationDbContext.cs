using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using GamingSessionApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamingSessionApp.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //one-to-many for Session => SessionMessage
            modelBuilder.Entity<SessionMessage>()
                .HasRequired(s => s.Session)
                .WithMany(s => s.Messages)
                .HasForeignKey(s => s.SessionId)
                .WillCascadeOnDelete(false);

            //one-to-many for UserProfile => UserFriends
            modelBuilder.Entity<UserFriend>()
                .HasRequired(s => s.Profile)
                .WithMany(s => s.Friends)
                .HasForeignKey(s => s.ProfileId)
                .WillCascadeOnDelete(false);

            //Many-to-many for sessions and users
            modelBuilder.Entity<Session>()
                   .HasMany<UserProfile>(s => s.Members)
                   .WithMany(u => u.Sessions)
                   .Map(cs =>
                   {
                       cs.MapLeftKey("SessionId");
                       cs.MapRightKey("UserId");
                       cs.ToTable("SessionMembers");
                   });

            //Fix for 2 navigation properties
            modelBuilder.Entity<UserMessage>()
                .HasRequired(p => p.Recipient)
                .WithMany(m => m.Messages)
                .HasForeignKey(f => f.RecipientId)
                .WillCascadeOnDelete(false);

            //Fix for 2 navigation properties
            modelBuilder.Entity<SessionFeedback>()
                .HasRequired(x => x.User)
                .WithMany(x => x.Feedback)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            //Overrider the cascade delete issue.
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<SessionStatus> SessionStatuses { get; set; }

        public DbSet<SessionSettings> SessionSettings { get; set; }

        public DbSet<SessionMessage> SessionMessages { get; set; }

        public DbSet<SessionMessageType> SessionMessageTypes { get; set; }

        public DbSet<SessionDuration> SessionDurations { get; set; }

        public DbSet<Platform> Platforms { get; set; }

        public DbSet<SessionType> SessionTypes { get; set; }

        public DbSet<Kudos> Kudos { get; set; }

        public DbSet<KudosHistory> KudosHistory { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<UserNotificationType> UserNotificationTypes { get; set; }

        public DbSet<UserMessage> UserMessages { get; set; }

        public DbSet<UserPreferences> UserPreferences { get; set; }

        public DbSet<EmailReminderTime> EmailReminderTimes { get; set; }

        public DbSet<SessionFeedback> SessionFeedback { get; set; }
    }
}
