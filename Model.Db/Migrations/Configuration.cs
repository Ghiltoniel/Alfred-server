namespace Alfred.Model.Db.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AlfredContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AlfredContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.Configurations.AddOrUpdate(
              p => p.Name,
                new Alfred.Model.Db.Configuration() { Name = "Alfred_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Common_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Interface_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Device_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Mail_Enabled", Value = "0" },
                new Alfred.Model.Db.Configuration() { Name = "MediaManager_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Meteo_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Multiroom_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "ParisBouge_Enabled", Value = "0" },
                new Alfred.Model.Db.Configuration() { Name = "Playlist_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Rappel_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Scenario_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Showcase_Enabled", Value = "0" },
                new Alfred.Model.Db.Configuration() { Name = "Wiki_Enabled", Value = "0" },
                new Alfred.Model.Db.Configuration() { Name = "Device_HueEnabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Device_HueBridgeIp", Value = "192.168.0.13" },
                new Alfred.Model.Db.Configuration() { Name = "Device_HueBridgeUser", Value = "developper" },
                new Alfred.Model.Db.Configuration() { Name = "Device_TelldusEnabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Device_TelldusConsumerKey", Value = "FEHUVEW84RAFR5SP22RABURUPHAFRUNU" },
                new Alfred.Model.Db.Configuration() { Name = "Device_TelldusConsumerSecret", Value = "ZUXEVEGA9USTAZEWRETHAQUBUR69U6EF" },
                new Alfred.Model.Db.Configuration() { Name = "Device_TelldusToken", Value = "e1da4e4f8f2d705c23f24032bc984c9d050423c7a" },
                new Alfred.Model.Db.Configuration() { Name = "Device_TelldusTokenSecret", Value = "c8cd435512952def2c5d5b321a2f01f0" },
                new Alfred.Model.Db.Configuration() { Name = "TestPlugin_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Sensor_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "User_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Player_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Chat_Enabled", Value = "1" },
                new Alfred.Model.Db.Configuration() { Name = "Velib_Enabled", Value = "1" }
            );

            context.Paths.AddOrUpdate(
                p => p.Name,
                new Path() { Name = "Movies", Value = @"\\192.168.1.32\Films" },
                new Path() { Name = "Music", Value = @"\\192.168.1.32\Musiques" },
                new Path() { Name = "Series", Value = @"\\192.168.1.32\Séries" },
                new Path() { Name = "WebsiteRessources", Value = @"C:\inetpub\wwwroot\Home\Ressources\Text" }
            );

            context.RadioDbs.AddOrUpdate(
                p => p.BaseName,
                new RadioDb() { BaseName = "JazzRadio", BaseUrl = "http://www.jazzradio.fr/radio/webradio/jazz", DisplayName = "Jazz Radio", ThumbnailUrl = "http://www.jazzradio.fr/assets/logo.jpg", HasSubsetRadios = true },
                new RadioDb() { BaseName = "DjamRadio", BaseUrl = "http://www.ledjamradio.com/playlist.m3u", DisplayName = "Djam Radio", ThumbnailUrl = "http://www.ledjamradio.com/gfx/logo.png", HasSubsetRadios = false },
                new RadioDb() { BaseName = "DIRadio", BaseUrl = "http://www.di.fm/", DisplayName = "Digitally Imported", ThumbnailUrl = "http://www.di.fm/assets/flux/branding/logo-di-e9580dd4ef317e3a977539651162ff5f.png", HasSubsetRadios = true }
            );

            context.Users.AddOrUpdate(
                p => p.Id,
                new User() { Id = 1, Username = "Guigui", Password = "1000:fb4+tVr04V579XOz7k+7pIzf5B0tUC61:Y/vgDsSL4bpRLAdcVHjHR8SrsL8vCSzM" },
                new User() { Id = 2, Username = "Vince", Password = "1000:tVWAx67KUT+MDwq0XgFWhIPQb3Y3Ax0R:REXI+HGxF0oC85YNzPWZv2M4lDgbsAEy" },
                new User() { Id = 3, Username = "Chloé", Password = "1000:g8fVqlnM2npPPz9ymq1i5YFfe1Lo1syW:Bi9kyt9Aj/r1fEVXpoqKiS8bCu6opHo5" }
            );

            context.Commands.AddOrUpdate(
                c => c.Name,
                new Command() { Name = "Alfred_Start", Ruleref = "" },
                new Command() { Name = "Common_Insult", Ruleref = "" },
                new Command() { Name = "Common_Dialog", Ruleref = "" },
                new Command() { Name = "Media_StartStopKinect", Ruleref = "" },
                new Command() { Name = "Alfred_StopListening", Ruleref = "" },
                new Command() { Name = "Alfred_Again", Ruleref = "" },
                new Command() { Name = "Common_Time", Ruleref = "" },
                new Command() { Name = "Common_VolumeDown", Ruleref = "" },
                new Command() { Name = "MediaManager_PlayPause", Ruleref = "" },
                new Command() { Name = "MediaManager_Tournedisque", Ruleref = "" },
                new Command() { Name = "MediaManager_Next", Ruleref = "" },
                new Command() { Name = "MediaManager_Previous", Ruleref = "" },
                new Command() { Name = "Common_VolumeUp", Ruleref = "" },
                new Command() { Name = "Meteo_MeteoToday", Ruleref = "" },
                new Command() { Name = "Device_AllumeTout", Ruleref = "" },
                new Command() { Name = "Device_EteinsTout", Ruleref = "" },
                new Command() { Name = "Device_Allume", Ruleref = "piece" },
                new Command() { Name = "Device_Eteins", Ruleref = "piece" },
                new Command() { Name = "MediaManager_PlaylistGenre", Ruleref = "genre" },
                new Command() { Name = "MediaManager_PlaylistArtist", Ruleref = "artist" },
                new Command() { Name = "Scenario_LaunchScenario", Ruleref = "mode" },
                new Command() { Name = "Device_TurnUp", Ruleref = "piece" },
                new Command() { Name = "Device_TurnDown", Ruleref = "piece" },
                new Command() { Name = "Alfred_RestartTimer", Ruleref = "" },
                new Command() { Name = "Interface_StartStopKinect", Ruleref = "" },
                new Command() { Name = "Velib_GetNearestAvailabilities", Ruleref = "" });

            context.CommandItems.AddOrUpdate(
                new CommandItem() { CommandId = 1, Term = "Pouchmina" },
                new CommandItem() { CommandId = 2, Term = "Pouchmina Ta Gueule" },
                new CommandItem() { CommandId = 2, Term = "Pouchmina Tais-toi" },
                new CommandItem() { CommandId = 2, Term = "Ta Gueule" },
                new CommandItem() { CommandId = 3, Term = "Comment ça va" },
                new CommandItem() { CommandId = 3, Term = "ça va pas mal" },
                new CommandItem() { CommandId = 3, Term = "Tu as quel âge" },
                new CommandItem() { CommandId = 4, Term = "Démarre la kinect" },
                new CommandItem() { CommandId = 4, Term = "Arrête la kinect" },
                new CommandItem() { CommandId = 5, Term = "Merci Pouchmina" },
                new CommandItem() { CommandId = 5, Term = "C'est parfait" },
                new CommandItem() { CommandId = 5, Term = "Merci" },
                new CommandItem() { CommandId = 5, Term = "C'est parfait, Merci" },
                new CommandItem() { CommandId = 6, Term = "Encore" },
                new CommandItem() { CommandId = 6, Term = "Encore un peu" },
                new CommandItem() { CommandId = 7, Term = "Il est quelle heure" },
                new CommandItem() { CommandId = 7, Term = "Quelle heure est-il" },
                new CommandItem() { CommandId = 8, Term = "Baisse le son" },
                new CommandItem() { CommandId = 9, Term = "Arrête la musique" },
                new CommandItem() { CommandId = 9, Term = "Démarre la musique" },
                new CommandItem() { CommandId = 10, Term = "Démarre le tournedisque" },
                new CommandItem() { CommandId = 10, Term = "Lance le tournedisque" },
                new CommandItem() { CommandId = 11, Term = "Musique suivante" },
                new CommandItem() { CommandId = 12, Term = "Musique précédente" },
                new CommandItem() { CommandId = 13, Term = "Monte le son" },
                new CommandItem() { CommandId = 14, Term = "Quel temps il fait dehors" },
                new CommandItem() { CommandId = 14, Term = "Quel temps fait-il dehors" },
                new CommandItem() { CommandId = 14, Term = "Il fait chaud dehors" },
                new CommandItem() { CommandId = 14, Term = "Il fait beau dehors" },
                new CommandItem() { CommandId = 15, Term = "Allume toutes les lumières" },
                new CommandItem() { CommandId = 16, Term = "Eteins toutes les lumières" },
                new CommandItem() { CommandId = 17, Term = "Allume le" },
                new CommandItem() { CommandId = 17, Term = "Allume la" },
                new CommandItem() { CommandId = 17, Term = "Allume la lumière de" },
                new CommandItem() { CommandId = 18, Term = "Eteins le" },
                new CommandItem() { CommandId = 18, Term = "Eteins la lumière de" },
                new CommandItem() { CommandId = 19, Term = "Mets nous un peu de " },
                new CommandItem() { CommandId = 19, Term = "Lance une playlist de " },
                new CommandItem() { CommandId = 20, Term = "Mets nous du " },
                new CommandItem() { CommandId = 21, Term = "Mets toi en mode " },
                new CommandItem() { CommandId = 22, Term = "Augmente la luminosité du" },
                new CommandItem() { CommandId = 22, Term = "Augmente la luminosité de l'" },
                new CommandItem() { CommandId = 23, Term = "Baisse la luminosité du" },
                new CommandItem() { CommandId = 23, Term = "Diminue la luminosité du" },
                new CommandItem() { CommandId = 23, Term = "Baisse la luminosité de l'" },
                new CommandItem() { CommandId = 26, Term = "Combien de vélib sont disponibles" });

            context.Scenarios.AddOrUpdate(
                s => s.Name,
                new Scenario() { Name = "Romantique", Artist = "", Lights = @"[{""Key"":""1"",""Name"":""Salle de bain"",""On"":true,""Bri"":154,""Hue"":56739,""Sat"":187,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""2"",""Name"":""Salon"",""On"":true,""Bri"":138,""Hue"":47213,""Sat"":172,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""3"",""Name"":""Entrée"",""On"":true,""Bri"":158,""Hue"":53832,""Sat"":226,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""4"",""Name"":""Unused"",""On"":true,""Bri"":204,""Hue"":0,""Sat"":0,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""5"",""Name"":""Chambre Vincent"",""On"":true,""Bri"":0,""Hue"":0,""Sat"":0,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""6"",""Name"":""Spot 1"",""On"":true,""Bri"":255,""Hue"":54613,""Sat"":255,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""7"",""Name"":""Spot 2"",""On"":true,""Bri"":255,""Hue"":43690,""Sat"":255,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""8"",""Name"":""Vestiaire"",""On"":true,""Bri"":207,""Hue"":47542,""Sat"":255,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""748506"",""Name"":""Chambre Guillaume"",""On"":true,""Bri"":0,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":true,""ColorEnabled"":false},{""Key"":""346051"",""Name"":""Cuisine"",""On"":true,""Bri"":0,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":true,""ColorEnabled"":false},{""Key"":""346053"",""Name"":""Feu rouge"",""On"":true,""Bri"":0,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":true,""ColorEnabled"":false},{""Key"":""346052"",""Name"":""Led"",""On"":false,""Bri"":2,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":false,""ColorEnabled"":false}]" },
                new Scenario() { Name = "Matin week-end", Radio = "DjamRadio", RadioUrl = "", Lights = @"[{""Key"":""1"",""Name"":""Salle de bain"",""On"":true,""Bri"":254,""Hue"":14922,""Sat"":144,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""2"",""Name"":""Salon"",""On"":true,""Bri"":254,""Hue"":14922,""Sat"":144,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""3"",""Name"":""Entrée"",""On"":true,""Bri"":254,""Hue"":14922,""Sat"":144,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""4"",""Name"":""Unused"",""On"":true,""Bri"":204,""Hue"":0,""Sat"":0,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""5"",""Name"":""Chambre Vincent"",""On"":true,""Bri"":237,""Hue"":14922,""Sat"":144,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""6"",""Name"":""Spot 1"",""On"":true,""Bri"":255,""Hue"":54613,""Sat"":255,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""7"",""Name"":""Spot 2"",""On"":true,""Bri"":255,""Hue"":43690,""Sat"":255,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""8"",""Name"":""Vestiaire"",""On"":true,""Bri"":104,""Hue"":20667,""Sat"":250,""Type"":1,""DimEnabled"":true,""ColorEnabled"":true},{""Key"":""748506"",""Name"":""Chambre Guillaume"",""On"":true,""Bri"":1,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":true,""ColorEnabled"":false},{""Key"":""346051"",""Name"":""Cuisine"",""On"":true,""Bri"":16,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":true,""ColorEnabled"":false},{""Key"":""346053"",""Name"":""Feu rouge"",""On"":true,""Bri"":1,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":true,""ColorEnabled"":false},{""Key"":""346052"",""Name"":""Led"",""On"":false,""Bri"":2,""Hue"":null,""Sat"":null,""Type"":0,""DimEnabled"":false,""ColorEnabled"":false}]" });
        }
    }
}