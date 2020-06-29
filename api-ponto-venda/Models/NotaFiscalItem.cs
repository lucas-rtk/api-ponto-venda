using api_ponto_venda.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace api_ponto_venda.Models
{
    public class NotaFiscalItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [JsonIgnore]
        public int NotaFiscalId { get; set; }
        [Required]
        [ForeignKey("NotaFiscalId")]
        [JsonIgnore]
        public NotaFiscal NotaFiscal { get; set; }
        [JsonIgnore]
        public int ProdutoId { get; set; }
        [Required]
        [ForeignKey("ProdutoId")]
        public Produto Produto { get; set; }
        [Required]
        public double Preco { get; set; }
        [Required]
        public double Quantidade { get; set; }
        public double ValorImpostos { get; set; }
        [Required]
        public double Total { get; set; }

        private bool ProdutoExiste()
        {
            if (Produto == null)
                return false;

            Produto tmp = new MySQLContext().Produtos
                                    .Where(p => p.Id == Produto.Id)
                                    .FirstOrDefault();

            return tmp != null;
        }

        public bool ValidarCamposObrigatorios(out string erro)
        {
            if (!ProdutoExiste())
            {
                erro = "Produto não informado ou não encontrado!";
                return false;
            }

            if (Preco <= 0 && Produto.Preco == 0)
            {
                erro = "O produto não possui preço definido!";
                return false;
            }

            if (Quantidade == 0)
            {
                erro = "O produto não possui quantidade definida!";
                return false;
            }

            erro = "";
            return true;
        }
    
        public void TotalizarItem()
        {
            Total = (Quantidade * Preco) + ValorImpostos;
        }
    }
}