using System.Security.Claims;
using OpenIdAuthServer;
using OpenIdAuthServer.OpenIdCustom;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Server.OpenIddictServerEvents;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000") // React app URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for cookies or tokens in CORS
    });
});

// Add authentication and authorization
/*
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:7003"; // Your authorization server URL
    options.Audience = "api";
    options.RequireHttpsMetadata = true;
});
*/
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies"; // Use cookies for user authentication
    options.DefaultChallengeScheme = "Cookies"; // Redirect to login page when unauthenticated
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Account/Login"; // Path to redirect if not authenticated
}); // Add cookie-based authentication
/*
builder.Services.AddAuthentication(options =>  {
    options.DefaultScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
});
*/


builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseMongoDb()
               .UseDatabase(new MongoDbContext(builder.Configuration).GetDatabase());
    })
    .AddValidation(options => {
        options.UseLocalServer();
        options.UseAspNetCore();
    })
    .AddServer(options =>
    {
        options.AllowAuthorizationCodeFlow()
               .AllowClientCredentialsFlow()
               .AllowRefreshTokenFlow();

        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetTokenEndpointUris("/connect/token")
               .SetEndSessionEndpointUris("/connect/logout");

        // Register custom authorization handler
        options.AddEventHandler<HandleAuthorizationRequestContext>(
            builder => builder.UseScopedHandler<CustomAuthorizationHandler>());

        // Register your custom handler for end session requests.
        options.AddEventHandler<HandleEndSessionRequestContext>(
            builder => builder.UseScopedHandler<CustomEndSessionRequestHandler>());

        // Register the event handler.
        options.AddEventHandler<OpenIddictServerEvents.ValidateTokenContext>(builder =>
        {
            builder.UseInlineHandler(async context =>
            {
                // Customize token request handling here.
                if (context.Request.IsPasswordGrantType())
                {
                    // Example: Validate custom credentials.
                    if (context.Request.Username == "customUser" && context.Request.Password == "customPassword")
                    {
                        context.Principal = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                new[] { new Claim("Subject", "userId") },
                                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                            )
                        );

                        // Set the requested scopes.
                        context.Principal.SetScopes(context.Request.GetScopes());
                    }
                    else
                    {
                        context.Reject(
                            error: "InvalidGrant",
                            description: "Invalid username or password."
                        );
                    }
                }

                // Let OpenIddict handle other grant types.
            });
        });

        options.AddEphemeralEncryptionKey()
               .AddEphemeralSigningKey();

        options.UseAspNetCore();
               //.EnableTokenEndpointPassthrough();
               //.EnableAuthorizationEndpointPassthrough()
    });

/*
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "XSRF-TOKEN";  // You can customize the cookie name if needed
    options.FormFieldName = "__RequestVerificationToken";
});
*/

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

// Use CORS
app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();

// Call the seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DatabaseSeeder.SeedAsync(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
