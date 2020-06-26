﻿using api_ponto_venda.Database;
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
    public class CaixasController : ApiController
    {
        public HttpResponseMessage Post(Caixa caixa)
        {
            if (!caixa.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "O nome do caixa não pode estar em branco!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                caixa.Saldo = 0; //Sempre cadastra um novo caixa com saldo zerado
                contexto.Caixas.Add(caixa);
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
                ReasonPhrase = "Caixa criado com sucesso!",
                StatusCode = HttpStatusCode.OK,
                Headers = { { "CaixaId", caixa.Id.ToString() } }
            };
        }

        public Caixa Get(int Id)
        {
            return new MySQLContext().Caixas
                        .Where(c => c.Id == Id)
                        .SingleOrDefault();
        }

        public HttpResponseMessage Put(int Id, [FromBody] Caixa caixa)
        {
            if (!caixa.ValidarCamposObrigatorios())
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "O nome do caixa não pode estar em branco!",
                    StatusCode = HttpStatusCode.BadRequest
                };

            try
            {
                MySQLContext contexto = new MySQLContext();

                Caixa oldCaixa = contexto.Caixas
                                   .Where(c => c.Id == Id)
                                   .SingleOrDefault();

                if (oldCaixa == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Caixa não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                oldCaixa.Nome = caixa.Nome;
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
                ReasonPhrase = "Caixa alterado com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        public HttpResponseMessage Delete(int Id)
        {
            MySQLContext contexto = new MySQLContext();
            Caixa tmp = new Caixa { Id = Id };

            if (tmp.Saldo > 0)
                return new HttpResponseMessage()
                {
                    ReasonPhrase = "Não é permitido excluir um caixa com saldo positivo, realize uma transferência de valores!",
                    StatusCode = HttpStatusCode.BadRequest
                };

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
                ReasonPhrase = "Caixa removido com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }

        [Route("api/Caixas/Listar")]
        [HttpGet]
        public List<Caixa> Listar(string Nome = "")
        {
            return new MySQLContext().Caixas
                        .Where(c => c.Nome.Contains(Nome))
                        .ToList();
        }
        
        [Route("api/Caixas/AbrirCaixa")]
        [HttpPost]
        public HttpResponseMessage AbrirCaixa(int Id, double Valor)
        {
            try
            {
                MySQLContext contexto = new MySQLContext();
                Caixa caixa = contexto.Caixas
                                   .Where(c => c.Id == Id)
                                   .SingleOrDefault();

                if (caixa == null)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "Caixa não encontrado com o ID informado!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                if (caixa.Saldo > 0)
                    return new HttpResponseMessage()
                    {
                        ReasonPhrase = "O caixa já está aberto!",
                        StatusCode = HttpStatusCode.BadRequest
                    };

                caixa.Saldo = Valor;
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
                ReasonPhrase = "Caixa aberto com sucesso!",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}