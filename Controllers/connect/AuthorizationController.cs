using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace OpenIddict.ConnectControllers;

[Route("connect")]
public class AuthorizationController : Controller
{
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthorizationController(IOpenIddictScopeManager scopeManager)
    {
        _scopeManager = scopeManager;
    }

    [HttpGet("authorize")]
    [Authorize]
    public async Task<IActionResult> AuthorizeAsync()
    {
        // Extract the OpenID Connect request from the HttpContext
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null)
        {
            return BadRequest("Invalid authorization request.");
        }

        // Custom logic: for example, ensure requested scopes are valid
        foreach (var scope in request.GetScopes())
        {
            if (await _scopeManager.FindByNameAsync(scope) is null)
            {
                return BadRequest($"Invalid scope: {scope}");
            }
        }

        // Return an authorization page (e.g., a consent form)
        return View("Consent", request);
    }

    [HttpPost("authorize")]
    [Authorize]
    public async Task<IActionResult> AuthorizeConfirm()
    {
        // Get the authorization request
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null)
        {
            return BadRequest("Invalid authorization request.");
        }

        // Make sure the user has consented to the requested scopes
        // TODO check consent
        /*
        if (!User.HasClaim(c => c.Type == "consented"))
        {
            // If the user hasn't consented, redirect them to the consent page
            return Redirect("/connect/consent");
        }
        */

        // Create the authorization response
        var response = new OpenIddictResponse
        {
            Code = await GenerateAuthorizationCodeAsync(request), // Generate the authorization code
            Scope = string.Join(" ", request.GetScopes())
        };

        // Redirect to the client's redirect_uri with the authorization code
        var redirectUri = request.RedirectUri;
        var queryParams = new Dictionary<string, string>
        {
            ["code"] = response.Code,
            ["state"] = request.State
        };

        var uri = QueryHelpers.AddQueryString(redirectUri, queryParams);

        return Redirect(uri); // Redirect back to the client with the authorization code
    }

    private async Task<string> GenerateAuthorizationCodeAsync(OpenIddictRequest request)
    {
        // Generate the authorization code (you would implement this according to your requirements)
        // Example: Store the code in a database or a cache
        var code = Guid.NewGuid().ToString(); // Simplified for this example
        return await Task.FromResult(code);
    }

}
