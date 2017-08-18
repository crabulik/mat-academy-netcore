using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace MatOrderingService.Services.Auth
{
    public class MatOsAuthHandler : AuthenticationHandler<MatOsAuthOptions>
    {
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var keyword = Options.AuthenticationScheme + " ";
            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorization))
            {
                return await Task.FromResult(AuthenticateResult.Skip());
            }

            if (!authorization.StartsWith(keyword))
            {
                return await Task.FromResult(AuthenticateResult.Skip());
            }

            var authValue = authorization.Substring(keyword.Length).Trim();

            if (string.IsNullOrEmpty(authValue))
            {
                return await Task.FromResult(AuthenticateResult.Skip());
            }

            if (authValue.Equals(Options.SecurityKey))
            {
                return await Task.FromResult(AuthenticateResult.Success(
                    new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimsIdentity.DefaultRoleClaimType, Roles.MatRegisteredUser, ClaimValueTypes.String),
                        }, Options.AuthenticationScheme)),
                        new AuthenticationProperties(),
                        Options.AuthenticationScheme)));
            }
            else
            {
                return await Task.FromResult(AuthenticateResult.Skip());
            }
        }
    }
}