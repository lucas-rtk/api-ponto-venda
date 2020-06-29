using api_ponto_venda.Database;
using api_ponto_venda.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api_ponto_venda.Controllers
{
    [Authorize]
    public class ProdutosController : BaseController
    {
        public HttpResponseMessage Post(Produto produto)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para cadastrar produtos!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            if (!produto.ValidarCamposObrigatorios(out string erro))
                return new HttpResponseMessage()
                {
                    ReasonPhrase = erro,
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                contexto.Produtos.Add(produto);
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
                ReasonPhrase = "Produto criado com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Headers = { { "ProdutoId", produto.Id.ToString() } },
                Content = new StringContent(JsonConvert.SerializeObject(produto))
            };
        }

        public Produto Get(int Id)
        {
            return new MySQLContext().Produtos
                        .Where(p => p.Id == Id)
                        .SingleOrDefault();
        }

        public HttpResponseMessage Put(int Id, [FromBody] Produto produto)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para alterar produtos!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            if (!produto.ValidarCamposObrigatorios(out string erro))
                return new HttpResponseMessage()
                {
                    ReasonPhrase = erro,
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Produto oldprod = contexto.Produtos
                                   .Where(p => p.Id == Id)
                                   .SingleOrDefault();

                if (oldprod == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Produto não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                produto.Id = Id;
                contexto.Entry(oldprod).CurrentValues.SetValues(produto);
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
                ReasonPhrase = "Produto alterado com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(produto))
            };
        }

        public HttpResponseMessage Delete(int Id)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para excluir produtos!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            MySQLContext contexto = new MySQLContext();
            Produto tmp = new Produto { Id = Id };

            try
            {
                contexto.Entry(tmp).State = EntityState.Deleted;
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
                ReasonPhrase = "Produto removido com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        [Route("api/Produtos/Listar")]
        [HttpGet]
        public List<Produto> Listar(string Nome = "")
        {
            return new MySQLContext().Produtos
                        .Where(p => p.Nome.Contains(Nome))
                        .ToList();
        }
    }
}