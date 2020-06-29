namespace api_ponto_venda.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotasFiscais : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotasFiscais",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Numero = c.Int(nullable: false),
                    FornecedorId = c.Int(nullable: false),
                    DataEmissao = c.DateTime(nullable: false, precision: 0, null, "CURRENT_TIMESTAMP"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fornecedores", t => t.FornecedorId, cascadeDelete: true)
                .Index(t => t.FornecedorId);

            CreateTable(
                "dbo.NotaFiscalItems",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NotaFiscalId = c.Int(nullable: false),
                    ProdutoId = c.Int(nullable: false),
                    Preco = c.Double(nullable: false),
                    Quantidade = c.Double(nullable: false),
                    ICMS_Valor = c.Double(nullable: false),
                    ICMS_Aliquota = c.Double(nullable: false),
                    IPI_Valor = c.Double(nullable: false),
                    IPI_Aliquota = c.Double(nullable: false),
                    Total = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produtos", t => t.ProdutoId, cascadeDelete: true)
                .ForeignKey("dbo.NotasFiscais", t => t.NotaFiscalId, cascadeDelete: true)
                .Index(t => t.NotaFiscalId)
                .Index(t => t.ProdutoId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotaFiscalItems", "NotaFiscalId", "dbo.NotasFiscais");
            DropForeignKey("dbo.NotaFiscalItems", "ProdutoId", "dbo.Produtos");
            DropForeignKey("dbo.NotasFiscais", "FornecedorId", "dbo.Fornecedores");
            DropIndex("dbo.NotaFiscalItems", new[] { "ProdutoId" });
            DropIndex("dbo.NotaFiscalItems", new[] { "NotaFiscalId" });
            DropIndex("dbo.NotasFiscais", new[] { "FornecedorId" });
            DropTable("dbo.NotaFiscalItems");
            DropTable("dbo.NotasFiscais");
        }
    }
}
