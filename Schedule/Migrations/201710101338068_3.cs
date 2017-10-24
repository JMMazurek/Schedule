namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Events", "EventTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "EventTypeId", c => c.Int(nullable: false));
        }
    }
}
