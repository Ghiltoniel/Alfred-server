namespace Alfred.Model.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRadio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RadioDbs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseUrl = c.String(nullable: false, maxLength: 256),
                        BaseName = c.String(nullable: false, maxLength: 256),
                        DisplayName = c.String(nullable: false, maxLength: 256),
                        HasSubsetRadios = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RadioDbs");
        }
    }
}
