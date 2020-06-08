using api_ponto_venda.Business;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;

[assembly: OwinStartup(typeof(api_ponto_venda.OwinStartup))]

namespace api_ponto_venda
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = TokenManager.ObterParametrosValidacao()
            });
        }
    }
}