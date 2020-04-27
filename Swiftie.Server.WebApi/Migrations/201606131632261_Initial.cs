namespace Swiftie.Server.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.Stories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(),
                    Title = c.String(),
                    Description = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    IsFeatured = c.Boolean(nullable: false),
                    FeatureLink = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                    Audio_Id = c.Int(),
                    Image_Id = c.Int(),
                    Video_Id = c.Int(),
                    UserProfile_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Audios", t => t.Audio_Id)
                .ForeignKey("dbo.Images", t => t.Image_Id)
                .ForeignKey("dbo.Videos", t => t.Video_Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .Index(t => t.Audio_Id)
                .Index(t => t.Image_Id)
                .Index(t => t.Video_Id)
                .Index(t => t.UserProfile_Id);

            CreateTable(
                "dbo.Audios",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Duration = c.Double(nullable: false),
                    Url = c.String(),
                    Type = c.Int(nullable: false),
                    Title = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Comments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(),
                    StoryId = c.Int(nullable: false),
                    Title = c.String(),
                    Description = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DateCreated = c.DateTime(nullable: false),
                    Audio_Id = c.Int(),
                    Image_Id = c.Int(),
                    Video_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Audios", t => t.Audio_Id)
                .ForeignKey("dbo.Images", t => t.Image_Id)
                .ForeignKey("dbo.Videos", t => t.Video_Id)
                .ForeignKey("dbo.Stories", t => t.StoryId, cascadeDelete: true)
                .Index(t => t.StoryId)
                .Index(t => t.Audio_Id)
                .Index(t => t.Image_Id)
                .Index(t => t.Video_Id);

            CreateTable(
                "dbo.Images",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Width = c.Int(nullable: false),
                    Height = c.Int(nullable: false),
                    Aspect = c.Double(nullable: false),
                    ImageType = c.String(),
                    Url = c.String(),
                    Type = c.Int(nullable: false),
                    Title = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Videos",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Duration = c.Double(nullable: false),
                    Width = c.Int(nullable: false),
                    Height = c.Int(nullable: false),
                    Aspect = c.Double(nullable: false),
                    VideoType = c.String(),
                    Url = c.String(),
                    Type = c.Int(nullable: false),
                    Title = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);

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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.UserProfiles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(),
                    RemainingFeatureStars = c.Int(nullable: false),
                    UserProfile_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .Index(t => t.UserProfile_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Stories", "UserProfile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "UserProfile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stories", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.Stories", "Image_Id", "dbo.Images");
            DropForeignKey("dbo.Comments", "StoryId", "dbo.Stories");
            DropForeignKey("dbo.Comments", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.Comments", "Image_Id", "dbo.Images");
            DropForeignKey("dbo.Comments", "Audio_Id", "dbo.Audios");
            DropForeignKey("dbo.Stories", "Audio_Id", "dbo.Audios");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.UserProfiles", new[] { "UserProfile_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Comments", new[] { "Video_Id" });
            DropIndex("dbo.Comments", new[] { "Image_Id" });
            DropIndex("dbo.Comments", new[] { "Audio_Id" });
            DropIndex("dbo.Comments", new[] { "StoryId" });
            DropIndex("dbo.Stories", new[] { "UserProfile_Id" });
            DropIndex("dbo.Stories", new[] { "Video_Id" });
            DropIndex("dbo.Stories", new[] { "Image_Id" });
            DropIndex("dbo.Stories", new[] { "Audio_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Videos");
            DropTable("dbo.Images");
            DropTable("dbo.Comments");
            DropTable("dbo.Audios");
            DropTable("dbo.Stories");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
