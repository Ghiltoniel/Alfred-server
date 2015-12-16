namespace Alfred.Model.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRadio1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RadioDbs", "DisplayName", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RadioDbs", "DisplayName", c => c.String(nullable: false, maxLength: 256));
        }
    }
}
