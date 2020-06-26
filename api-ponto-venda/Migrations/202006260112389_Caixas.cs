namespace api_ponto_venda.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Caixas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Caixas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        Saldo = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Caixas");
        }
    }
}
