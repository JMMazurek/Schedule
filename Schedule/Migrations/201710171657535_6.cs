namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invitations", "Canceled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Requests", "Canceled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "Canceled");
            DropColumn("dbo.Invitations", "Canceled");
        }
    }
}
