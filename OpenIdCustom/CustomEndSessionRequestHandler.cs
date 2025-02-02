using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Microsoft.Extensions.Logging;
using static OpenIddict.Server.OpenIddictServerEvents;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Server.AspNetCore;

namespace OpenIdAuthServer.OpenIdCustom;

public class CustomEndSessionRequestHandler : IOpenIddictServerHandler<HandleEndSessionRequestContext>
{
    private readonly ILogger<CustomEndSessionRequestHandler> _logger;

    public CustomEndSessionRequestHandler(ILogger<CustomEndSessionRequestHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(HandleEndSessionRequestContext context)
    {
        _logger.LogInformation("Handling end session request for client: {ClientId}", context.Request.ClientId);

        // If a post_logout_redirect_uri is specified, redirect the user to that URI.
        if (!string.IsNullOrEmpty(context.Request.PostLogoutRedirectUri))
        {
            var redirectUri = context.Request.PostLogoutRedirectUri;

            var parameters = new Dictionary<string, OpenIddictParameter>();
            parameters.Add("post_logout_redirect_uri", new OpenIddictParameter(redirectUri));

            context.SignOut(parameters);
        }
        else
        {
            // If no post-logout URI was provided, you can return a simple status.
            //context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            context.SignOut();
        }

        // Mark the request as handled so that OpenIddict doesn't attempt further processing.
        context.HandleRequest();

        return default;
    }
}
