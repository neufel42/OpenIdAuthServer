using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Bson;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using System.Security.Claims;
using System.Text.Json;

namespace OpenIddict.ConnectControllers;

[Route("connect")]
public class AuthorizationController : Controller
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IOpenIddictTokenManager _tokenManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;

    public AuthorizationController(IOpenIddictScopeManager scopeManager, IOpenIddictTokenManager tokenManager, IOpenIddictAuthorizationManager authorizationManager)
    {
        _scopeManager = scopeManager;
        _tokenManager = tokenManager;
        _authorizationManager = authorizationManager;
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

/*
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
        // TODO: Check consent logic here, if necessary

        // Generate the authorization code (custom implementation)
        var customCode = await GenerateAuthorizationCodeAsync(request); // Your method to generate a code
        var customAuthID = await GenerateAuthorizationCodeAsync(request); // Your method to generate a code

        // Create the authorization response
        var response = new OpenIddictResponse
        {
            Code = customCode, // The generated authorization code
            Scope = string.Join(" ", request.GetScopes())
        };

        // Create the Authorization Code entry in the OpenIddict store
        var authorizationDescriptor = new OpenIddictMongoDbAuthorization
        {
            ApplicationId = new ObjectId("67887aa48a2c877f9693c2cb"), // TODO Fix hardcoded value
            CreationDate = DateTime.UtcNow,
            // TODO: Scopes
            // TODO: Properties
            Status = "active",
            Subject = "john_doe_code",
            Type = "authorization_code",
        };



        var existingAuthorizationList = _authorizationManager.FindBySubjectAsync(authorizationDescriptor.Subject).ToBlockingEnumerable();
        var existingAuthorization = existingAuthorizationList.FirstOrDefault();
        if (existingAuthorization != null)
        {
            // Authorization already exists, you can update it or handle it accordingly
            // ...
        }
        else
        {

            var validations = _authorizationManager.ValidateAsync(authorizationDescriptor);
            var validationResult = validations.ToBlockingEnumerable();
            foreach (var validation in validationResult)
            {
                Console.WriteLine(validation.ErrorMessage);
            }
            // Create a new authorization
            await _authorizationManager.CreateAsync(authorizationDescriptor);
        }

        existingAuthorizationList = _authorizationManager.FindBySubjectAsync(authorizationDescriptor.Subject).ToBlockingEnumerable();
        existingAuthorization = existingAuthorizationList.FirstOrDefault();



        Console.WriteLine(JsonSerializer.Serialize(existingAuthorization));

        // Create the token (authorization code) entry in the OpenIddict store
        await _tokenManager.CreateAsync(new OpenIddictTokenDescriptor()
        {
            CreationDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddMinutes(5), // Set an expiration time for the code
            Payload = customCode, // Your generated custom authorization code
            //Principal = User, // ClaimsPrincipal representing the authenticated user
            Status = OpenIddictConstants.Statuses.Valid,
            Subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, // The user associated with this token
            Type = "authorization_code"
        });

        // Redirect to the client's redirect_uri with the authorization code
        var redirectUri = request.RedirectUri;
        var queryParams = new Dictionary<string, string>
        {
            ["code"] = customCode, // The custom authorization code
            ["state"] = request.State // The original state parameter
        };

        var uri = QueryHelpers.AddQueryString(redirectUri, queryParams);

        return Redirect(uri); // Redirect back to the client with the authorization code
    }
*/

    private async Task<string> GenerateAuthorizationCodeAsync(OpenIddictRequest request)
    {
        // Generate the authorization code (you would implement this according to your requirements)
        // Example: Store the code in a database or a cache
        var code = Guid.NewGuid().ToString(); // Simplified for this example
        return await Task.FromResult(code);
    }

}
