using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Data.Database;

namespace BizName.Studio.Api.IntegrationTests.Common;

public class ApiTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly ConcurrentDictionary<Guid, bool> _provisionedTenants = new();

    public async Task InitializeAsync()
    {
        // Initialize test database only - tenants are provisioned per-fixture on demand
        using var scope = Services.CreateScope();
        var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await databaseInitializer.InitializeAsync();
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            // Load test-specific configuration from environment-specific JSON file
            var testProjectPath = Path.GetDirectoryName(typeof(ApiTestFixture).Assembly.Location);
            var testConfigPath = Path.Combine(testProjectPath!, "appsettings.Testing.json");
            
            if (File.Exists(testConfigPath))
            {
                config.AddJsonFile(testConfigPath, optional: false, reloadOnChange: false);
            }
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Override authentication with test authentication
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
        });
    }

    public HttpClient CreateAuthenticatedClientWithUniqueTenant()
    {
        var uniqueTenantId = Guid.NewGuid();
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", uniqueTenantId.ToString());
        
        // Ensure tenant is provisioned synchronously to avoid race conditions
        using var scope = Services.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        try
        {
            EnsureTenantProvisioned(uniqueTenantId, tenantService).GetAwaiter().GetResult();
        }
        catch
        {
            // Ignore provisioning errors in tests - tenant will be created on demand by API
        }
        
        return client;
    }

    public HttpClient CreateAnonymousClient()
    {
        return CreateClient();
    }

    private async Task EnsureTenantProvisionedSafe(Guid tenantId)
    {
        try
        {
            using var scope = Services.CreateScope();
            var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
            await EnsureTenantProvisioned(tenantId, tenantService);
        }
        catch
        {
            // Ignore provisioning errors in tests
        }
    }

    private async Task EnsureTenantProvisioned(Guid tenantId, ITenantService tenantService)
    {
        if (_provisionedTenants.ContainsKey(tenantId))
            return;

        var testTenant = Tenant.New(tenantId, $"Test Organization {tenantId:N}");
        await tenantService.EnsureProvisioning(testTenant);
        _provisionedTenants[tenantId] = true;
    }
}

public class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if the Authorization header is present
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Test "))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Extract tenant ID from the token part of Authorization header
        var tenantIdString = authHeader.Substring(5); // Remove "Test "
        if (!Guid.TryParse(tenantIdString, out var tenantId))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid tenant ID in token"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim("extension_Organization", tenantId.ToString()),
            new Claim("scp", "components.read components.write") // Add scope claims for authorization
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
