namespace Swiftie.Server.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_info : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "Avatar", c => c.String());
            AddColumn("dbo.UserProfiles", "DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "DisplayName");
            DropColumn("dbo.UserProfiles", "Avatar");
        }
    }
}
