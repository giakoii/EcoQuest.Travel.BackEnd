using System.Net;
using System.Security.Claims;
using BackEnd;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Models.Helpers;
using BackEnd.Repositories;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using SystemConfig = BackEnd.Models.SystemConfig;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Get the connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable(ConstEnv.ConnectionString);

builder.Services.AddDataProtection();

builder.Services.AddDbContext<AppDbContext>(options =>
{ 
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});

builder.Services.AddScoped<ITempCodeService, TempCodeService>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
builder.Services.AddScoped<CloudinaryLogic>();
builder.Services.AddScoped<IIdentityApiClient, IdentityApiClient>();
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<IHotelService, HotelService>();

builder.Services.AddScoped<IBaseRepository<SystemConfig, string>, BaseRepository<SystemConfig, string>>();
builder.Services.AddScoped<IBaseRepository<Role, Guid>, BaseRepository<Role, Guid>>();
builder.Services.AddScoped<IBaseRepository<User, Guid>, BaseRepository<User, Guid>>();
builder.Services.AddScoped<IBaseRepository<Partner, Guid>, BaseRepository<Partner, Guid>>();
builder.Services.AddScoped<IBaseRepository<PartnerPartnerType, Guid>, BaseRepository<PartnerPartnerType, Guid>>();
builder.Services.AddScoped<IBaseRepository<Destination, Guid>, BaseRepository<Destination, Guid>>();
builder.Services.AddScoped<IBaseRepository<DestinationImage, Guid>, BaseRepository<DestinationImage, Guid>>();
builder.Services.AddScoped<IBaseRepository<Hotel, Guid>, BaseRepository<Hotel, Guid>>();

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
    options.AddDefaultPolicy(build=> build
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
        options.AllowCustomFlow("external");
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
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
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable(ConstEnv.GoogleClientId)!;
        options.ClientSecret = Environment.GetEnvironmentVariable(ConstEnv.GoogleClientSecret)!;
        options.CallbackPath = "/signin-google";
        
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
        options.ClaimActions.MapJsonKey("picture", "picture");
    });


// DB context that inherits AppDbContext
builder.Services.AddHttpContextAccessor();
// ConfigureServices
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ConstRole.Admin, policy =>
    {
        policy.RequireRole(ConstRole.Admin);
        policy.AddAuthenticationSchemes(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    });
});



// Add the worker service
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5269");
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5269");
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.UseDeveloperExceptionPage();
app.UseStatusCodePages(); 
app.Run();
app.UseHttpsRedirection();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.UseDeveloperExceptionPage();
app.UseStatusCodePages(); 
app.Run();