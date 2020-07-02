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
    public class NotasFiscaisController : BaseController
    {
        [Route("api/NotasFiscais/NovaNota")]
        [HttpPost]
        public HttpResponseMessage NovaNota(NotaFiscal Nota)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para lançar notas de compra!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            if (Nota == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Objeto da nota fiscal mal formado ou não encontrado!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (!Nota.ValidarCamposObrigatorios(out string erro))
                return new HttpResponseMessage()
                {
                    ReasonPhrase = erro,
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Nota.Fornecedor = contexto.Fornecedores.Where(f => f.Id == Nota.Fornecedor.Id).FirstOrDefault();
                Nota.TotalizaNota();

                contexto.NotasFiscais.Add(Nota);
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
                ReasonPhrase = "Nota fiscal criada com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(Nota)),
                Headers = { { "NotaFiscalId", Nota.Id.ToString() } }
            };
        }
    
        [Route("api/NotasFiscais/{Id}/IncluirItem")]
        [HttpPost]
        public HttpResponseMessage IncluirItem(int Id, [FromBody] NotaFiscalItem Item)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para incluir produtos nas notas de compra!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            if (Item == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Objeto do item mal formado ou não encontrado!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (!Item.ValidarCamposObrigatorios(out string erro))
                return new HttpResponseMessage()
                {
                    ReasonPhrase = erro,
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Item.NotaFiscal = contexto.NotasFiscais
                                        .Where(n => n.Id == Id)
                                        .Include("Fornecedor")
                                        .Include("Itens.Produto")
                                        .FirstOrDefault();
                if (!Item.NotaFiscal.EmEdicao)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Não é possível incluir item em uma nota fiscal já lançada!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                Item.Produto = contexto.Produtos.Where(p => p.Id == Item.Produto.Id).FirstOrDefault();

                if (Item.Preco == 0)
                    Item.Preco = Item.Produto.Preco;

                Item.TotalizarItem();

                contexto.NotasFiscaisItens.Add(Item);
                contexto.SaveChanges();

                Item.NotaFiscal.TotalizaNota();
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
                ReasonPhrase = "Item incluído com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(Item.NotaFiscal)),
                Headers = { { "NotaFiscalItemId", Item.Id.ToString() } }
            };
        }
    
        [Route("api/NotasFiscais/{IdNota}/RemoverItem/{Id}")]
        [HttpDelete]
        public HttpResponseMessage RemoverItem(int IdNota, int Id)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para excluir produtos das notas fiscais!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            MySQLContext contexto = new MySQLContext();
            NotaFiscal nota = contexto.NotasFiscais
                                .Where(n => n.Id == IdNota)
                                .Include("Fornecedor")
                                .Include("Itens.Produto")
                                .FirstOrDefault();

            if (nota == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Nota fiscal não encontrada!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (!nota.EmEdicao)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "A nota fiscal indicada já está lançada!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            bool achou = false;
            foreach (NotaFiscalItem item in nota.Itens)
                if (item.Id == Id)
                    {
                        achou = true;
                        break;
                    }

            if (!achou)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "O item indicado não pertence a nota indicada!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            NotaFiscalItem tmp = contexto.NotasFiscaisItens.Where(i => i.Id == Id).FirstOrDefault();
            try
            {
                contexto.Entry(tmp).State = EntityState.Deleted;
                contexto.SaveChanges();

                nota.TotalizaNota();
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
                ReasonPhrase = "Item removido com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(nota))
            };
        }

        [Route("api/NotasFiscais/{Id}/DefinirValores")]
        [HttpPost]
        public HttpResponseMessage DefinirValores(int Id, double? ValorFrete = null, double? ValorSeguro = null)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para definir valores na nota fiscal!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            MySQLContext contexto = new MySQLContext();
            NotaFiscal nota = contexto.NotasFiscais
                                .Where(n => n.Id == Id)
                                .Include("Fornecedor")
                                .Include("Itens.Produto")
                                .FirstOrDefault();

            if (nota == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Nota fiscal não encontrada!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (!nota.EmEdicao)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "A nota fiscal indicada já está lançada!",
                    StatusCode = HttpStatusCode.BadRequest
                };
            
            try
            {
                if (ValorFrete != null)
                    nota.ValorFrete = ValorFrete.Value;

                if (ValorSeguro != null)
                    nota.ValorSeguro = ValorSeguro.Value;

                nota.TotalizaNota();
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
                ReasonPhrase = "Frete atualizado com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(nota))
            };
        }

        public HttpResponseMessage Delete(int Id)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para excluir notas fiscais!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            MySQLContext contexto = new MySQLContext();
            NotaFiscal tmp = new NotaFiscal { Id = Id };

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
                ReasonPhrase = "Nota fiscal removida com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        [Route("api/NotasFiscais/{Id}/LancarNota")]
        [HttpPost]
        public HttpResponseMessage LancarNota(int Id)
        {
            if (ObterUsuario(RequestContext.Principal.Identity.Name).Perfil == PerfilUsuario.OperadorCaixa)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Usuário sem permissão para lançar notas fiscais!",
                    StatusCode = HttpStatusCode.Forbidden
                };

            MySQLContext contexto = new MySQLContext();
            NotaFiscal nota = contexto.NotasFiscais
                                .Where(n => n.Id == Id)
                                .Include("Fornecedor")
                                .Include("Itens.Produto")
                                .FirstOrDefault();

            if (nota == null)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Nota fiscal não encontrada!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (!nota.EmEdicao)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "A nota fiscal indicada já está lançada!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                nota.EmEdicao = false;
                nota.TotalizaNota();
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
                ReasonPhrase = "Nota lançada com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(nota))
            };
        }

        [Route("api/NotasFiscais/Listar")]
        [HttpGet]
        public List<NotaFiscal> Listar(int IdFornecedor = -1, bool ApenasLancadas = false)
        {
            var notas = new MySQLContext().NotasFiscais.AsQueryable();

            if (IdFornecedor >= 0)
                notas = notas.Where(n => n.Fornecedor.Id == IdFornecedor);

            if (ApenasLancadas)
                notas = notas.Where(n => n.EmEdicao == false);

            notas = notas
                        .Include("Fornecedor")
                        .Include("Itens.Produto");

            return notas.ToList();
        }

        public NotaFiscal Get(int Id)
        {
            return new MySQLContext().NotasFiscais
                            .Where(n => n.Id == Id)
                            .Include("Fornecedor")
                            .Include("Itens.Produto")
                            .FirstOrDefault();
        }
    }
}