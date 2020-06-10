using api_ponto_venda.Business;
using api_ponto_venda.Database;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace api_ponto_venda.Models
{
    public enum PerfilUsuario
    {
        Gerente = 0,
        BackOffice = 1,
        OperadorCaixa = 2
    }

    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(120)]
        public string Email { get; set; }
        [Required]
        [JsonIgnore]
        [Column("Senha")]
        public byte[] SenhaCrypto { get; set; }
        [Required]
        public PerfilUsuario Perfil { get; set; }
        [NotMapped]
        public string Senha { get; set; } //Campo utilizado apenas para receber a senha durante o login

        public bool ValidarLogin()
        {
            if (String.IsNullOrWhiteSpace(Email) || String.IsNullOrWhiteSpace(Senha))
                return false;

            Usuario usuario = new MySQLContext().Usuarios
                                    .Where(u => u.Email == Email)
                                    .FirstOrDefault();

            if (usuario == null)
                return false;

            return Crypto.CompararHashes(usuario.SenhaCrypto, Crypto.CalcularHash(Senha));
        }
    }
}