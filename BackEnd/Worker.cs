using BackEnd.Models.Helpers;
using BackEnd.Utils.Const;
using DotNetEnv;
using OpenIddict.Abstractions;

namespace BackEnd;

/// <summary>
/// Perform the necessary operations when the application starts
/// </summary>
public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider"></param>
    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Implement method
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
        await CreateApplicationsAsync();
        async Task CreateApplicationsAsync()
        {
            // Load environment variables from .env file
            Env.Load();
            
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("service_client") == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "service_client",
                    ClientSecret = Environment.GetEnvironmentVariable(EnvConst.ClientSecret),
                    DisplayName = "Service client application",
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Introspection,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.EndSession,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.Prefixes.Scope,
                    },
                };
                await manager.CreateAsync(descriptor);
            }
        }
    }
    
    /// <summary>
    ///  Implement method
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
