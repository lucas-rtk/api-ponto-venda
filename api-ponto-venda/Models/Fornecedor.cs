﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
            return (!String.IsNullOrWhiteSpace(RazaoSocial)) && (!String.IsNullOrWhiteSpace(CNPJ));
        }
    }
}