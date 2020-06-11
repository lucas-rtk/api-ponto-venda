using api_ponto_venda.Database;
using api_ponto_venda.Models;
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
            HttpResponseMessage Retorno = new HttpResponseMessage();

            if (!produto.ValidarCamposObrigatorios())
            {
                Retorno.ReasonPhrase = "O nome do produto não pode estar em branco!";
                Retorno.StatusCode = HttpStatusCode.BadRequest;
                return Retorno;
            }

            try
            {
                MySQLContext contexto = new MySQLContext();

                contexto.Produtos.Add(produto);
                contexto.SaveChanges();
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;

                Retorno.Content = new StringContent(e.Message + "\n" + e.StackTrace);
                Retorno.StatusCode = HttpStatusCode.InternalServerError;
                return Retorno;
            }

            Retorno.ReasonPhrase = "Produto criado com sucesso!";
            Retorno.StatusCode = HttpStatusCode.OK;
            Retorno.Headers.Add("ProdutoId", produto.Id.ToString());
            return Retorno;
        }

        public Produto Get(int Id)
        {
            return new MySQLContext().Produtos
                        .Where(p => p.Id == Id)
                        .SingleOrDefault();
        }

        public HttpResponseMessage Put(int Id, [FromBody] Produto produto)
        {
            if (!produto.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "O nome do produto não pode estar em branco!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Produto tmp = contexto.Produtos
                                   .Where(p => p.Id == Id)
                                   .SingleOrDefault();

                if (tmp == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Produto não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                tmp.Nome = produto.Nome;
                tmp.EAN = produto.EAN;
                tmp.Preco = produto.Preco;
                tmp.IPI = produto.IPI;
                tmp.ICMS = produto.ICMS;
                tmp.Saldo = produto.Saldo;

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
                ReasonPhrase = "Produto alterado com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        public HttpResponseMessage Delete(int Id)
        {
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
                    Content = new StringContent(e.Message + "\n" + e.StackTrace),
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