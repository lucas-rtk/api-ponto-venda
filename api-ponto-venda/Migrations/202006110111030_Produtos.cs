namespace api_ponto_venda.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Produtos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Produtos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        EAN = c.String(maxLength: 13, storeType: "nvarchar"),
                        Preco = c.Double(nullable: false, 0f),
                        IPI = c.Double(nullable: false, 0f),
                        ICMS = c.Double(nullable: false, 0f),
                        Saldo = c.Double(nullable: false, 0f),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Produtos");
        }
    }
}
