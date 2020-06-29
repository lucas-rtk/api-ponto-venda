using api_ponto_venda.Database;
using api_ponto_venda.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;

namespace api_ponto_venda.Controllers
{
    [Authorize]
    public class FornecedoresController : BaseController
    {
        public HttpResponseMessage Post(Fornecedor fornecedor)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para cadastrar fornecedores!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            if (!fornecedor.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "A razão social e CNPJ do fornecedor não podem estar em branco!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                contexto.Fornecedores.Add(fornecedor);
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
                ReasonPhrase = "Fornecedor criado com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Headers = { { "FornecedorId", fornecedor.Id.ToString() } }
            };
        }

        public Fornecedor Get(int Id)
        {
            return new MySQLContext().Fornecedores
                        .Where(f => f.Id == Id)
                        .SingleOrDefault();
        }

        public HttpResponseMessage Put(int Id, [FromBody] Fornecedor fornecedor)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para alterar fornecedores!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            if (!fornecedor.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "A razão social e CNPJ do fornecedor não podem estar em branco!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Fornecedor oldofnr = contexto.Fornecedores
                                   .Where(f => f.Id == Id)
                                   .SingleOrDefault();

                if (oldofnr == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Fornecedor não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                oldofnr.Id = Id;
                contexto.Entry(oldofnr).CurrentValues.SetValues(fornecedor);
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
                ReasonPhrase = "Fornecedor alterado com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        public HttpResponseMessage Delete(int Id)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para cadastrar fornecedores!",
                    StatusCode = HttpStatusCode.Forbidden
                };

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
                    Content = new StringContent(e.Message + "\n\n" + e.StackTrace),
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