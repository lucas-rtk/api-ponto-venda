using api_ponto_venda.Models;
using MySql.Data.EntityFramework;
using System.Data.Entity;

namespace api_ponto_venda.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MySQLContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        public MySQLContext() : base("ConexaoMySQL")
        {
            Configuration.LazyLoadingEnabled = true;
        }
    }
}