namespace Alfred.Model.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMaxLengthLights : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Scenarios", "Lights", c => c.String(maxLength: 3000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Scenarios", "Lights", c => c.String(maxLength: 256));
        }
    }
}
