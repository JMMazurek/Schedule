namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Schedule.Models.SheduleDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Schedule.Models.SheduleDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            SeedGroupRoles(context);
        }

        private void SeedGroupRoles(Schedule.Models.SheduleDbContext context)
        {
            context.GroupRoles.AddOrUpdate(
                gr => gr.Name,
                new Models.GroupRole
                {
                    Name = "User",
                    CanManageGroup = false,
                    CanManageUsers = false,
                    CanManageUsersEvents = false,
                    CanManageUsersRoles = false
                },
                new Models.GroupRole
                {
                    Name = "Moderator",
                    CanManageGroup = false,
                    CanManageUsers = false,
                    CanManageUsersEvents = true,
                    CanManageUsersRoles = false
                },
                new Models.GroupRole
                {
                    Name = "Administrator",
                    CanManageGroup = true,
                    CanManageUsers = true,
                    CanManageUsersEvents = true,
                    CanManageUsersRoles = true
                }
                );
        }
    }
}
