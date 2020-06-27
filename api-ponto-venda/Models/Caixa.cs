using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_ponto_venda.Models
{
    [Table("Caixas")]
    public class Caixa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Nome { get; set; }
        public double Saldo { get; set; }

        public Caixa()
        {
            Saldo = 0;
        }

        public bool ValidarCamposObrigatorios()
        {
            return (!string.IsNullOrWhiteSpace(Nome));
        }
    }
}