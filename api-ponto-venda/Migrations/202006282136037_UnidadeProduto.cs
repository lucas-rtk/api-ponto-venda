namespace api_ponto_venda.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnidadeProduto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Produtos", "Unidade", c => c.String(maxLength: 3, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Produtos", "Unidade");
        }
    }
}
