using api_ponto_venda.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace api_ponto_venda.Models
{
    [Table("NotasFiscais")]
    public class NotaFiscal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(6)]
        public string Serie { get; set; }
        [Required]
        public int Numero { get; set; }
        [JsonIgnore]
        public int FornecedorId { get; set; }
        [Required]
        [ForeignKey("FornecedorId")]        
        public Fornecedor Fornecedor { get; set; }
        [Required]
        public DateTime DataEmissao { get; set; }
        public double ValorFrete { get; set; }
        public double ValorSeguro { get; set; }
        public double ValorImpostos { get; set; }
        public double ValorTotal { get; set; }
        [InverseProperty("NotaFiscal")]
        public virtual List<NotaFiscalItem> Itens { get; set; }
        public bool EmEdicao { get; set; }

        public NotaFiscal()
        {
            DataEmissao = DateTime.Now;
            Itens = new List<NotaFiscalItem>();
            EmEdicao = true;
        }

        private bool FornecedorExiste()
        {
            if (Fornecedor == null)
                return false;

            Fornecedor tmp = new MySQLContext().Fornecedores
                                    .Where(f => f.Id == Fornecedor.Id)
                                    .FirstOrDefault();

            return tmp != null;
        }

        public bool ValidarCamposObrigatorios(out string erro)
        {           
            if (!FornecedorExiste())
            {
                erro = "Fornecedor não informado ou não encontrado!";
                return false;
            }

            if (Numero <= 0)
            {
                erro = "Número da nota fiscal não informado ou inválido!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Serie))
            {
                erro = "Série da nota fiscal não informada ou inválida!";
                return false;
            }

            erro = "";
            return true;
        }

        public void TotalizaNota()
        {
            for (int i = 0; i < Itens.Count; i++)
                Itens[i].TotalizarItem();

            ValorImpostos = (from item in Itens select item.ValorImpostos).Sum();
            ValorTotal = ValorFrete + ValorSeguro + ValorImpostos + (from item in Itens select item.Total).Sum();
        }
    }
}