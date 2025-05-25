using System.Net;
using AuthService;
using DotNetEnv;
using BackEnd.Models;
using BackEnd.Models.Helpers;
using BackEnd.Repositories;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Abstractions;
using SystemConfig = BackEnd.Models.SystemConfig;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Get the connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable(EnvConst.ConnectionString);

builder.Services.AddDataProtection();

builder.Services.AddDbContext<AppDbContext>(options =>
{ 
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
builder.Services.AddScoped<IIdentityApiClient, IdentityApiClient>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBaseRepository<SystemConfig, string>, BaseRepository<SystemConfig, string>>();
builder.Services.AddScoped<IBaseRepository<Role, Guid>, BaseRepository<Role, Guid>>();
builder.Services.AddScoped<IBaseRepository<User, Guid>, BaseRepository<User, Guid>>();

// Add services to the container.
builder.Services.AddControllers();

// Swagger configuration to output API type definitions
builder.Services.AddSwaggerDocument(config =>
{
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
    config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
        new OpenApiSecurityScheme()
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = nameof(Authorization),
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copy this into the value field: Bearer {token}"
        }
    );
});

// Allow API to be read from outside
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Configure(builder.Configuration.GetSection("Kestrel"));
});
var urls = builder.Configuration["Kestrel:Endpoints:Http:Url"];
Console.WriteLine($"Kestrel listening on: {urls}");

// Configure the OpenIdDict server
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(options =>
    {
        options.DisableAccessTokenEncryption();
        options.AcceptAnonymousClients();
        // Enable the required endpoints
        options.SetTokenEndpointUris("/connect/token");
        options.SetIntrospectionEndpointUris("/connect/introspect");
        options.SetUserInfoEndpointUris("/connect/userinfo");
        options.SetEndSessionEndpointUris("/connect/logout");
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.AllowCustomFlow("google");
        // Enable the client credentials flow
        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();
        options.AllowClientCredentialsFlow();
        options.AllowCustomFlow("logout");
        options.AllowAuthorizationCodeFlow();
        options.RegisterScopes(OpenIddictConstants.Scopes.OfflineAccess);
        // Register the signing and encryption credentials
        options.UseReferenceAccessTokens();
        options.UseReferenceRefreshTokens();
        options.DisableAccessTokenEncryption();
        // Register your scopes
        options.RegisterScopes(
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles);
        // Register the encryption credentials
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();  
        
        // Set the lifetime of the tokens
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
        options.SetRefreshTokenLifetime(TimeSpan.FromMinutes(120));
        // Register ASP.NET Core host and configure options
        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough()
            .DisableTransportSecurityRequirement();
        
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
    options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});


// DB context that inherits AppDbContext
builder.Services.AddHttpContextAccessor();
// ConfigureServices
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add the worker service
builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5269");
app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.UseDeveloperExceptionPage();
app.UseStatusCodePages(); 
app.Run();