using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpeechRecg.Web.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await Task.FromResult(new AuthenticationState(GetClaimsPrincipal()));
        }
        private IHttpContextAccessor Context { get; set; }

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContext)
        {
            this.Context = httpContext;
        }
        public void SetToken(string token)
        {
            Context.HttpContext.Session.SetString("Jwt", token);
        }
        public string GetToken()
        {
            return Context.HttpContext.Session.GetString("Jwt");
        }
        private ClaimsPrincipal GetClaimsPrincipal()
        {
            ClaimsIdentity identity;
            var userFromToken = BLL.Services.JWT.GetUser(GetToken());
            if (userFromToken != null)
            {
                var Claims = new List<Claim>();
                Claims.Add(new Claim(ClaimTypes.Name, userFromToken.Email));
                identity = new ClaimsIdentity(Claims, "apiauth_type");
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            return new ClaimsPrincipal(identity);
        }
        public void MarkUserAsAuthenticated()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(GetClaimsPrincipal())));
        }

        public void MarkUserAsLoggedOut()
        {
            this.SetToken(string.Empty);

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

    }
}
