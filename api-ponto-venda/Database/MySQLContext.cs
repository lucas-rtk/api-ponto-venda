using MySql.Data.EntityFramework;
using System.Data.Entity;

namespace api_ponto_venda.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MySQLContext : DbContext
    {
        public MySQLContext() : base("ConexaoMySQL")
        {
            Configuration.LazyLoadingEnabled = true;
        }
    }
}