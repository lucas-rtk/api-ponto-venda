using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace api_ponto_venda.Business
{
    public class TokenManager
    {
        private const string segredo = "52fd40c1cfc30998c5ee2a567c856c26c2a2d6c5a0984b04";
        private static readonly SymmetricSecurityKey chave = new SymmetricSecurityKey(Convert.FromBase64String(segredo));
        private const string emissor = "API-PontoVenda";

        private static ClaimsPrincipal ObterPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (jwtToken == null)
                    return null;

                return tokenHandler.ValidateToken(token, ObterParametrosValidacao(), out SecurityToken securityToken);
            }
            catch
            {
                return null;
            }
        }

        public static TokenValidationParameters ObterParametrosValidacao()
        {
            return new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidIssuer = emissor,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = chave
            };
        }

        public static string GerarToken(string email)
        {
            SecurityTokenDescriptor descritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256Signature),
                Issuer = emissor
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descritor);

            return handler.WriteToken(token);
        }

        public static string ValidarToken(string token)
        {
            ClaimsPrincipal principal = ObterPrincipal(token);

            if (principal == null)
                return null;

            ClaimsIdentity identidade;
            try
            {
                identidade = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }

            return identidade.FindFirst(ClaimTypes.Name).Value;
        }
    }
}