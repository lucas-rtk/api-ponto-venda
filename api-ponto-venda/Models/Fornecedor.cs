using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_ponto_venda.Models
{
    [Table("Fornecedores")]
    public class Fornecedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string RazaoSocial { get; set; }
        [Required]
        [MaxLength(14)]
        public string CNPJ { get; set; }
        [MaxLength(20)]
        public string Telefone { get; set; }
        [MaxLength(60)]
        public string Email { get; set; }

        public bool ValidarCamposObrigatorios()
        {
            return (!string.IsNullOrWhiteSpace(RazaoSocial)) && (!string.IsNullOrWhiteSpace(CNPJ));
        }
    }
}