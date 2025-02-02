using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

[Route("connect")]
public class TokenController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public TokenController(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

/*
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromForm] string code, [FromForm] string clientId)
    {
        try
        {
            //var response  = await _tokenManager.ExchangeAuthorizationCodeAsync(code, clientId);
            //var response = await ExchangeAuthorizationCodeAsync(code, clientId);
            await Task.CompletedTask;
            return Ok(); // Return the tokens
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "invalid_grant", error_description = ex.Message });
        }
    }
    */


    [HttpPost("tokenBAD")]
    public async Task<IActionResult> TokenAsync()
    {
        // Extract the OpenID Connect request from the HttpContext
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null)
        {
            return BadRequest("Invalid token request.");
        }

        // Validate the client_id and client_secret
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId);
        if (application is null)
        {
            return BadRequest("Invalid client ID.");
        }

        // Custom token issuance logic
        if (request.IsAuthorizationCodeGrantType())
        {
            // Issue an access token for the provided authorization code
            return SignIn(new ClaimsPrincipal(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsClientCredentialsGrantType())
        {
            // Issue an access token for the client credentials grant
            return SignIn(new ClaimsPrincipal(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest("Unsupported grant type.");
    }
}
