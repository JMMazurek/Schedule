namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes");
            DropForeignKey("dbo.EventTypes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Events", new[] { "EventTypeId" });
            DropIndex("dbo.EventTypes", new[] { "UserId" });
            DropTable("dbo.EventTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.EventTypes", "UserId");
            CreateIndex("dbo.Events", "EventTypeId");
            AddForeignKey("dbo.EventTypes", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes", "Id");
        }
    }
}
