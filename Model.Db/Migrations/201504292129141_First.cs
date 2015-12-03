namespace Alfred.Model.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommandItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Term = c.String(),
                        CommandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Commands", t => t.CommandId, cascadeDelete: true)
                .Index(t => t.CommandId);
            
            CreateTable(
                "dbo.Commands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Ruleref = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Configurations",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Episodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShowName = c.String(nullable: false),
                        Path = c.String(nullable: false),
                        SeasonNumber = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false),
                        Genre = c.String(),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MusicPlaylists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Playlist_Id = c.Int(nullable: false),
                        Music_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Musics", t => t.Music_Id, cascadeDelete: true)
                .ForeignKey("dbo.Playlists", t => t.Playlist_Id, cascadeDelete: true)
                .Index(t => t.Playlist_Id)
                .Index(t => t.Music_Id);
            
            CreateTable(
                "dbo.Musics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false),
                        Genre = c.String(),
                        Artist = c.String(),
                        Album = c.String(),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Playlists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Scenarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Playlist_Id = c.Int(),
                        Genre = c.String(),
                        Artist = c.String(),
                        Album = c.String(),
                        Title = c.String(),
                        Lights = c.String(),
                        Radio = c.String(),
                        RadioUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Playlists", t => t.Playlist_Id)
                .Index(t => t.Playlist_Id);
            
            CreateTable(
                "dbo.ScenarioTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        Hours = c.Int(nullable: false),
                        Minutes = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Scenario_Id = c.Int(nullable: false),
                        Scenario_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Scenarios", t => t.Scenario_Id1)
                .Index(t => t.Scenario_Id1);
            
            CreateTable(
                "dbo.Paths",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Channel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShopItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Product = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShopItems", "User_Id", "dbo.Users");
            DropForeignKey("dbo.MusicPlaylists", "Playlist_Id", "dbo.Playlists");
            DropForeignKey("dbo.ScenarioTimes", "Scenario_Id1", "dbo.Scenarios");
            DropForeignKey("dbo.Scenarios", "Playlist_Id", "dbo.Playlists");
            DropForeignKey("dbo.MusicPlaylists", "Music_Id", "dbo.Musics");
            DropForeignKey("dbo.CommandItems", "CommandId", "dbo.Commands");
            DropIndex("dbo.ShopItems", new[] { "User_Id" });
            DropIndex("dbo.ScenarioTimes", new[] { "Scenario_Id1" });
            DropIndex("dbo.Scenarios", new[] { "Playlist_Id" });
            DropIndex("dbo.MusicPlaylists", new[] { "Music_Id" });
            DropIndex("dbo.MusicPlaylists", new[] { "Playlist_Id" });
            DropIndex("dbo.CommandItems", new[] { "CommandId" });
            DropTable("dbo.Users");
            DropTable("dbo.ShopItems");
            DropTable("dbo.Players");
            DropTable("dbo.Paths");
            DropTable("dbo.ScenarioTimes");
            DropTable("dbo.Scenarios");
            DropTable("dbo.Playlists");
            DropTable("dbo.Musics");
            DropTable("dbo.MusicPlaylists");
            DropTable("dbo.Movies");
            DropTable("dbo.Episodes");
            DropTable("dbo.Configurations");
            DropTable("dbo.Commands");
            DropTable("dbo.CommandItems");
        }
    }
}
