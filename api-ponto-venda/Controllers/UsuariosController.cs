using api_ponto_venda.Models;
using api_ponto_venda.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api_ponto_venda.Database;

namespace api_ponto_venda.Controllers
{
    [AllowAnonymous]
    public class UsuariosController : BaseController
    {
        [Route("api/Usuarios/Login")]
        [HttpPost]
        public HttpResponseMessage Login(Usuario usuario)
        {
            HttpResponseMessage retorno = new HttpResponseMessage();

            if (!usuario.ValidarLogin())
            {
                retorno.ReasonPhrase = "O e-mail ou senha informados estão incorretos!";
                retorno.StatusCode = HttpStatusCode.BadRequest;
                return retorno;
            }

            string token = TokenManager.GerarToken(usuario.Email);

            retorno.ReasonPhrase = "Login efetuado!";
            retorno.StatusCode = HttpStatusCode.OK;
            retorno.Content = new StringContent(token);
            retorno.Headers.Add("token", token);

            return retorno;
        }

        [Route("api/Usuarios/ValidarToken")]
        [HttpGet]
        public HttpResponseMessage ValidarToken(string token, string email)
        {
            HttpResponseMessage retorno = new HttpResponseMessage();
            Usuario usuario = ObterUsuario(email);

            if (usuario == null)
            {
                retorno.ReasonPhrase = "O e-mail indicado não pertence a nenhum usuário cadastrado!";
                retorno.StatusCode = HttpStatusCode.BadRequest;
                return retorno;
            }

            string tokenUsername = TokenManager.ValidarToken(token);
            if (email.Equals(tokenUsername))
            {
                retorno.ReasonPhrase = "Token validado.";
                retorno.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                retorno.ReasonPhrase = "Token inválido.";
                retorno.StatusCode = HttpStatusCode.BadRequest;
            }

            return retorno;
        }

        [Route("api/Usuarios/Registrar")]
        [HttpPost]
        public HttpResponseMessage Registrar(Usuario usuario)
        {
            HttpResponseMessage retorno = new HttpResponseMessage();
            Usuario NovoUsuario;

            if (usuario == null)
            {
                retorno.ReasonPhrase = "Você deve informar um usuário e senha para criar o seu usuário!";
                retorno.StatusCode = HttpStatusCode.BadRequest;
                return retorno;
            }

            if (String.IsNullOrWhiteSpace(usuario.Email) || String.IsNullOrWhiteSpace(usuario.Senha))
            {
                retorno.ReasonPhrase = "Você deve informar um usuário e senha para criar o seu usuário!";
                retorno.StatusCode = HttpStatusCode.BadRequest;
                return retorno;
            }

            if (ObterUsuario(usuario.Email) != null)
            {
                retorno.ReasonPhrase = "Já existe um usuário cadastrado para este e-mail!";
                retorno.StatusCode = HttpStatusCode.BadRequest;
                return retorno;
            }

            try
            {
                MySQLContext contexto = new MySQLContext();

                NovoUsuario = new Usuario
                {
                    Email = usuario.Email,
                    SenhaCrypto = Crypto.CalcularHash(usuario.Senha),
                    Nome = usuario.Nome
                };

                contexto.Usuarios.Add(NovoUsuario);
                contexto.SaveChanges();
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;

                retorno.Content = new StringContent(e.Message + "\n" + e.StackTrace);
                retorno.StatusCode = HttpStatusCode.InternalServerError;
                return retorno;
            }

            retorno.ReasonPhrase = "Usuário criado com sucesso!";
            retorno.StatusCode = HttpStatusCode.OK;
            retorno.Headers.Add("UsuarioId", NovoUsuario.Id.ToString());
            return retorno;
        }
    }
}