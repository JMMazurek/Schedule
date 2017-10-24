namespace Schedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Starts = c.DateTime(nullable: false),
                        Ends = c.DateTime(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 300),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        EventTypeId = c.Int(nullable: false),
                        Group_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventTypes", t => t.EventTypeId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .ForeignKey("dbo.Groups", t => t.Group_Id)
                .Index(t => t.OwnerId)
                .Index(t => t.EventTypeId)
                .Index(t => t.Group_Id);
            
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
                        Discriminator = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvitedUserId = c.String(nullable: false, maxLength: 128),
                        InvitingUserId = c.String(nullable: false, maxLength: 128),
                        GroupId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Accepted = c.Boolean(nullable: false),
                        Rejected = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitedUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitingUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.InvitedUserId)
                .Index(t => t.InvitingUserId)
                .Index(t => t.GroupId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Searchable = c.Boolean(nullable: false),
                        NeedConfirmation = c.Boolean(nullable: false),
                        Public = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Memberships",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        GroupId = c.Int(nullable: false),
                        GroupRoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.GroupId })
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.GroupRoles", t => t.GroupRoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.GroupId)
                .Index(t => t.GroupRoleId);
            
            CreateTable(
                "dbo.GroupRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestingUserId = c.String(nullable: false, maxLength: 128),
                        AcceptingUserId = c.String(maxLength: 128),
                        GroupId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Accepted = c.Boolean(nullable: false),
                        Rejected = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AcceptingUserId)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.AspNetUsers", t => t.RequestingUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.RequestingUserId)
                .Index(t => t.AcceptingUserId)
                .Index(t => t.GroupId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Requests", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Invitations", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Invitations", "InvitingUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Invitations", "InvitedUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "RequestingUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Requests", "AcceptingUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Memberships", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Memberships", "GroupRoleId", "dbo.GroupRoles");
            DropForeignKey("dbo.Memberships", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Invitations", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Events", "Group_Id", "dbo.Groups");
            DropForeignKey("dbo.EventTypes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Requests", new[] { "User_Id" });
            DropIndex("dbo.Requests", new[] { "GroupId" });
            DropIndex("dbo.Requests", new[] { "AcceptingUserId" });
            DropIndex("dbo.Requests", new[] { "RequestingUserId" });
            DropIndex("dbo.Memberships", new[] { "GroupRoleId" });
            DropIndex("dbo.Memberships", new[] { "GroupId" });
            DropIndex("dbo.Memberships", new[] { "UserId" });
            DropIndex("dbo.Invitations", new[] { "User_Id" });
            DropIndex("dbo.Invitations", new[] { "GroupId" });
            DropIndex("dbo.Invitations", new[] { "InvitingUserId" });
            DropIndex("dbo.Invitations", new[] { "InvitedUserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.EventTypes", new[] { "UserId" });
            DropIndex("dbo.Events", new[] { "Group_Id" });
            DropIndex("dbo.Events", new[] { "EventTypeId" });
            DropIndex("dbo.Events", new[] { "OwnerId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Requests");
            DropTable("dbo.GroupRoles");
            DropTable("dbo.Memberships");
            DropTable("dbo.Groups");
            DropTable("dbo.Invitations");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.EventTypes");
            DropTable("dbo.Events");
        }
    }
}
