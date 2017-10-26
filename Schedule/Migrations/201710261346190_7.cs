namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupRoles", "CanManageUsers", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupRoles", "CanManageUsersEvents", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupRoles", "CanManageGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupRoles", "CanManageUsersRoles", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupRoles", "CanManageUsersRoles");
            DropColumn("dbo.GroupRoles", "CanManageGroup");
            DropColumn("dbo.GroupRoles", "CanManageUsersEvents");
            DropColumn("dbo.GroupRoles", "CanManageUsers");
        }
    }
}
