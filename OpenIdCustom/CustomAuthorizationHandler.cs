using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
//using OpenIddict.Server.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static OpenIddict.Server.OpenIddictServerEvents;
using System.Security.Claims;

public class CustomAuthorizationHandler : IOpenIddictServerHandler<HandleAuthorizationRequestContext>
{
    private readonly ILogger<CustomAuthorizationHandler> _logger;

    public CustomAuthorizationHandler(ILogger<CustomAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask HandleAsync(HandleAuthorizationRequestContext context)
    {
        _logger.LogInformation("Handling authorization request for client: {ClientId}", context.ClientId);

        // Example: Automatically approving requests for a specific client (Modify as needed)
        if (context.ClientId == "react-client")
        {
            context.Principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(OpenIddictConstants.Claims.Subject, "user-id"),
                    new Claim(OpenIddictConstants.Claims.Name, "John Doe"),
                }, OpenIddictConstants.Schemes.Bearer));

            context.Principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile);

            //context.HandleRequest();
        }
        else
        {
            context.Reject(
                error: OpenIddictConstants.Errors.InvalidClient,
                description: "Client is not authorized.");
        }
    }
}
