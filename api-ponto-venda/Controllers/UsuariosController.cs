using api_ponto_venda.Models;
using api_ponto_venda.Business;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api_ponto_venda.Database;
using System.Net.Http.Headers;

namespace api_ponto_venda.Controllers
{
    public class UsuariosController : BaseController
    {
        [Authorize]
        public Usuario Get()
        {
            return ObterUsuario(RequestContext.Principal.Identity.Name);
        }

        [Authorize]
        public HttpResponseMessage Put([FromBody] Usuario usuario)
        {
            try
            {
                MySQLContext contexto = new MySQLContext();

                Usuario oldusr = ObterUsuario(RequestContext.Principal.Identity.Name, contexto);

                if (oldusr == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Usuário não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                oldusr.Nome = usuario.Nome;
                oldusr.Perfil = usuario.Perfil;

                contexto.SaveChanges();
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                return new HttpResponseMessage()
                {
                    Content = new StringContent(e.Message + "\n\n" + e.StackTrace),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new HttpResponseMessage()
            {
                ReasonPhrase = "Usuário alterado com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        [AllowAnonymous]
        [Route("api/Usuarios/Login")]
        [HttpPost]
        public HttpResponseMessage Login(Usuario usuario)
        {
            if (!usuario.ValidarLogin())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "O e-mail ou senha informados estão incorretos!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            string token = TokenManager.GerarToken(usuario.Email);

            return new HttpResponseMessage()
            {
                ReasonPhrase = "Login efetuado!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(token),
                Headers = { { "token", token } }
            };
        }

        [AllowAnonymous]
        [Route("api/Usuarios/ValidarToken")]
        [HttpGet]
        public HttpResponseMessage ValidarToken(string token, string email)
        {
            Usuario usuario = ObterUsuario(email);

            if (usuario == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "O e-mail indicado não pertence a nenhum usuário cadastrado!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (email.Equals(TokenManager.ValidarToken(token)))
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Token valido",
                    StatusCode = HttpStatusCode.OK
                };
            else
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Token inválido",
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        [AllowAnonymous]
        [Route("api/Usuarios/Registrar")]
        [HttpPost]
        public HttpResponseMessage Registrar(Usuario usuario)
        {
            if (usuario == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Você deve informar um objeto de usuário para criar a sua conta!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (!usuario.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Você deve informar um usuário e senha para criar o seu usuário!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (ObterUsuario(usuario.Email) != null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Já existe um usuário cadastrado para este e-mail!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                usuario.SenhaCrypto = Crypto.CalcularHash(usuario.Senha);

                contexto.Usuarios.Add(usuario);
                contexto.SaveChanges();
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;

                return new HttpResponseMessage()
                {
                    Content = new StringContent(e.Message + "\n" + e.StackTrace),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new HttpResponseMessage()
            {
                ReasonPhrase = "Usuário criado com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Headers = { { "UsuarioId", usuario.Id.ToString() } }
            };
        }
    }
}