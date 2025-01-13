using OpenIddict.Abstractions;

namespace OpenIdAuthServer;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        // Resolve the required OpenIddict managers
        var scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();
        var appManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Seed scopes
        if (await scopeManager.FindByNameAsync("api1") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api1",
                DisplayName = "My API",
                Resources = { "api1" }
            });
        }

        // Seed applications (clients)
        if (await appManager.FindByClientIdAsync("react-client") is null)
        {
            await appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "react-client",
                ClientSecret = "react-secret",
                DisplayName = "React Client",
                RedirectUris = { new Uri("http://localhost:3000/callback") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Scopes.Address,
                }
            });
        }
    }
}
