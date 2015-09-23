using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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

        public DbSet<Session> Sessions { get; set; }

        public DbSet<SessionSettings> SessionSettings { get; set; }

        public DbSet<SessionDuration> SessionDurations { get; set; }

        public DbSet<Platform> Platforms { get; set; }

        public DbSet<SessionType> SessionTypes { get; set; }
    }
}
