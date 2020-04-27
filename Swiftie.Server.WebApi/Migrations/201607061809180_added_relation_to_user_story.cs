namespace Swiftie.Server.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_relation_to_user_story : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stories", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Stories", "User_Id");
            AddForeignKey("dbo.Stories", "User_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Stories", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stories", "UserId", c => c.String());
            DropForeignKey("dbo.Stories", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Stories", new[] { "User_Id" });
            DropColumn("dbo.Stories", "User_Id");
        }
    }
}
