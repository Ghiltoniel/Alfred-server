namespace Alfred.Model.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRadio2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RadioDbs", "ThumbnailUrl", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RadioDbs", "ThumbnailUrl");
        }
    }
}
