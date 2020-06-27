using api_ponto_venda.Database;
using api_ponto_venda.Models;
using System.Linq;
using System.Web.Http;

namespace api_ponto_venda.Controllers
{
    public class BaseController : ApiController
    {
        protected Usuario ObterUsuario(string email, MySQLContext contexto = null)
        {
            MySQLContext ctx;
            if (contexto == null)
                ctx = new MySQLContext();
            else
                ctx = contexto;

            return ctx.Usuarios
                    .Where(u => u.Email == email)
                    .FirstOrDefault();
        }
    }
}
