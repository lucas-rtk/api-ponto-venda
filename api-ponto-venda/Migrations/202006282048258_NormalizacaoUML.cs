namespace api_ponto_venda.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NormalizacaoUML : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotasFiscais", "Serie", c => c.String(nullable: false, maxLength: 6, storeType: "nvarchar"));
            AddColumn("dbo.NotasFiscais", "ValorFrete", c => c.Double(nullable: false));
            AddColumn("dbo.NotasFiscais", "ValorSeguro", c => c.Double(nullable: false));
            AddColumn("dbo.NotasFiscais", "ValorImpostos", c => c.Double(nullable: false));
            AddColumn("dbo.NotasFiscais", "ValorTotal", c => c.Double(nullable: false));
            AddColumn("dbo.NotasFiscais", "EmEdicao", c => c.Boolean(nullable: false));
            AddColumn("dbo.NotaFiscalItems", "ValorImpostos", c => c.Double(nullable: false));
            AddColumn("dbo.Produtos", "SKU", c => c.String(maxLength: 13, storeType: "nvarchar"));
            AddColumn("dbo.Produtos", "NCM", c => c.String(maxLength: 8, storeType: "nvarchar"));
            AddColumn("dbo.Produtos", "Origem", c => c.Int(nullable: false));
            DropColumn("dbo.NotaFiscalItems", "ICMS_Valor");
            DropColumn("dbo.NotaFiscalItems", "ICMS_Aliquota");
            DropColumn("dbo.NotaFiscalItems", "IPI_Valor");
            DropColumn("dbo.NotaFiscalItems", "IPI_Aliquota");
            DropColumn("dbo.Produtos", "EAN");
            DropColumn("dbo.Produtos", "IPI");
            DropColumn("dbo.Produtos", "ICMS");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Produtos", "ICMS", c => c.Double(nullable: false));
            AddColumn("dbo.Produtos", "IPI", c => c.Double(nullable: false));
            AddColumn("dbo.Produtos", "EAN", c => c.String(maxLength: 13, storeType: "nvarchar"));
            AddColumn("dbo.NotaFiscalItems", "IPI_Aliquota", c => c.Double(nullable: false));
            AddColumn("dbo.NotaFiscalItems", "IPI_Valor", c => c.Double(nullable: false));
            AddColumn("dbo.NotaFiscalItems", "ICMS_Aliquota", c => c.Double(nullable: false));
            AddColumn("dbo.NotaFiscalItems", "ICMS_Valor", c => c.Double(nullable: false));
            DropColumn("dbo.Produtos", "Origem");
            DropColumn("dbo.Produtos", "NCM");
            DropColumn("dbo.Produtos", "SKU");
            DropColumn("dbo.NotaFiscalItems", "ValorImpostos");
            DropColumn("dbo.NotasFiscais", "EmEdicao");
            DropColumn("dbo.NotasFiscais", "ValorTotal");
            DropColumn("dbo.NotasFiscais", "ValorImpostos");
            DropColumn("dbo.NotasFiscais", "ValorSeguro");
            DropColumn("dbo.NotasFiscais", "ValorFrete");
            DropColumn("dbo.NotasFiscais", "Serie");
        }
    }
}
