using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Schedule.Models
{
    public class SheduleDbContext : IdentityDbContext
    {
        public SheduleDbContext()
            : base("DefaultConnection")
        {
        }

        public static SheduleDbContext Create()
        {
            return new SheduleDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<User> Users { get; set; }

        public System.Data.Entity.DbSet<Schedule.Models.Notification> Notifications { get; set; }
    }
}