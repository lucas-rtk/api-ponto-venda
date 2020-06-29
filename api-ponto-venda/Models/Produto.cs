using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_ponto_venda.Models
{
    public enum OrigemProduto { Nacional, ImportacaoDireta, AdquiridaInterno }

    [Table("Produtos")]
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Nome { get; set; }
        [MaxLength(13)]
        public string SKU { get; set; }
        public double Preco { get; set; }
        [MaxLength(8)]
        public string NCM { get; set; }
        public double Saldo { get; set; }
        public OrigemProduto Origem { get; set; }
        [MaxLength(3)]
        public string Unidade { get; set; }

        public Produto()
        {
            Unidade = "UN";
        }

        public bool ValidarCamposObrigatorios(out string erro)
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                erro = "O nome do produto não pode estar em branco!";
                return false;
            }

            if (Preco < 0)
            {
                erro = "Não é possível cadastrar um produto com preço negativo!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NCM))
            {
                erro = "É necessário informar o NCM do produto!";
                return false;
            }

            erro = "";
            return true;
        }
    }
}