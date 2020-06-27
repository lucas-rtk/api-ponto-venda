namespace api_ponto_venda.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fornecedores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RazaoSocial = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        CNPJ = c.String(nullable: false, maxLength: 14, storeType: "nvarchar"),
                        Telefone = c.String(maxLength: 20, storeType: "nvarchar"),
                        Email = c.String(maxLength: 60, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        Email = c.String(nullable: false, maxLength: 120, storeType: "nvarchar"),
                        Senha = c.Binary(nullable: false),
                        Perfil = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Usuarios");
            DropTable("dbo.Fornecedores");
        }
    }
}
