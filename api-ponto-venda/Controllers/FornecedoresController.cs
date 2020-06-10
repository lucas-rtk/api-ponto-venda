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
    public class FornecedoresController : BaseController
    {
        public HttpResponseMessage Post(Fornecedor fornecedor)
        {
            HttpResponseMessage Retorno = new HttpResponseMessage();

            if (!fornecedor.ValidarCamposObrigatorios())
            {
                Retorno.ReasonPhrase = "A razão social e CNPJ do fornecedor não podem estar em branco!";
                Retorno.StatusCode = HttpStatusCode.BadRequest;
                return Retorno;
            }

            try
            {
                MySQLContext contexto = new MySQLContext();

                contexto.Fornecedores.Add(fornecedor);
                contexto.SaveChanges();
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;

                Retorno.Content = new StringContent(e.Message + "\n" + e.StackTrace);
                Retorno.StatusCode = HttpStatusCode.InternalServerError;
                return Retorno;
            }

            Retorno.ReasonPhrase = "Fornecedor criado com sucesso!";
            Retorno.StatusCode = HttpStatusCode.OK;
            Retorno.Headers.Add("FornecedorId", fornecedor.Id.ToString());
            return Retorno;
        }

        public Fornecedor Get(int Id)
        {
            return new MySQLContext().Fornecedores
                        .Where(f => f.Id == Id)
                        .SingleOrDefault();
        }

        public HttpResponseMessage Put(int Id, [FromBody] Fornecedor fornecedor)
        {
            if (!fornecedor.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "A razão social e CNPJ do fornecedor não podem estar em branco!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Fornecedor tmp = contexto.Fornecedores
                                   .Where(f => f.Id == Id)
                                   .SingleOrDefault();

                if (tmp == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Fornecedor não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                tmp.RazaoSocial = fornecedor.RazaoSocial;
                tmp.CNPJ = fornecedor.CNPJ;
                tmp.Telefone = fornecedor.Telefone;
                tmp.Email = fornecedor.Email;

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
                ReasonPhrase = "Fornecedor alterado com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        public HttpResponseMessage Delete(int Id)
        {
            MySQLContext contexto = new MySQLContext();
            Fornecedor tmp = new Fornecedor { Id = Id };
            
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
                ReasonPhrase = "Fornecedor removido com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        [Route("api/Fornecedores/Listar")]
        [HttpGet]
        public List<Fornecedor> Listar(string RazaoSocial = "")
        {
            return new MySQLContext().Fornecedores
                        .Where(f => f.RazaoSocial.Contains(RazaoSocial))
                        .ToList();
        }
    }
}