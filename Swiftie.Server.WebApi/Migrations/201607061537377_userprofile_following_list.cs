namespace Swiftie.Server.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_following_list : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.UserProfiles", new[] { "UserProfile_Id" });
            CreateTable(
                "dbo.UserProfileUserProfiles",
                c => new
                    {
                        UserProfile_Id = c.Int(nullable: false),
                        UserProfile_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserProfile_Id, t.UserProfile_Id1 })
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id1)
                .Index(t => t.UserProfile_Id)
                .Index(t => t.UserProfile_Id1);
            
            DropColumn("dbo.UserProfiles", "UserProfile_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "UserProfile_Id", c => c.Int());
            DropForeignKey("dbo.UserProfileUserProfiles", "UserProfile_Id1", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfileUserProfiles", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.UserProfileUserProfiles", new[] { "UserProfile_Id1" });
            DropIndex("dbo.UserProfileUserProfiles", new[] { "UserProfile_Id" });
            DropTable("dbo.UserProfileUserProfiles");
            CreateIndex("dbo.UserProfiles", "UserProfile_Id");
            AddForeignKey("dbo.UserProfiles", "UserProfile_Id", "dbo.UserProfiles", "Id");
        }
    }
}
