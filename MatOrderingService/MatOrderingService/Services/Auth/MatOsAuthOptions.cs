using Microsoft.AspNetCore.Builder;

namespace MatOrderingService.Services.Auth
{
    public class MatOsAuthOptions: AuthenticationOptions
    {
        public string SecurityKey { get; set; }
    }
}