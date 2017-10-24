namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Events", name: "Group_Id", newName: "GroupId");
            RenameIndex(table: "dbo.Events", name: "IX_Group_Id", newName: "IX_GroupId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Events", name: "IX_GroupId", newName: "IX_Group_Id");
            RenameColumn(table: "dbo.Events", name: "GroupId", newName: "Group_Id");
        }
    }
}
