using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GestionEquipo.Client.Autorizacion
{
    public class ProveedorAutenticacion : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //await Task.Delay(2000);
            var anonimo = new ClaimsIdentity();
            var usuarioPepe = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Bruno Giovanni"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("DNI", "42.338.781")
                },
                authenticationType: "ok"
                );
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(usuarioPepe)));
        }
    }
}
