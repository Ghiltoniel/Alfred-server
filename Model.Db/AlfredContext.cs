using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db
{
    internal class AlfredContext : DbContext
    {
        public AlfredContext() : base("AlfredDbModel.AlfredContext") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<CommandItem> CommandItems { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<MusicPlaylist> MusicPlaylists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<ScenarioTime> ScenarioTimes { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Path> Paths { get; set; }
        public DbSet<RadioDb> RadioDbs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
