using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_ponto_venda.Models
{
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
        public string EAN { get; set; }
        public double Preco { get; set; }
        public double IPI { get; set; }
        public double ICMS { get; set; }
        public double Saldo { get; set; }

        public bool ValidarCamposObrigatorios()
        {
            return (!string.IsNullOrWhiteSpace(Nome));
        }
    }
}