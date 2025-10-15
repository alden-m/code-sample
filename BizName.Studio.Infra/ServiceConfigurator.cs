using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BizName.Studio.App.Configurator;
using BizName.Studio.App.Services;
using BizName.Studio.Infra.Services;

namespace BizName.Studio.Infra;

internal class ServiceConfigurator : IServiceConfigurator
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register services
        services.AddScoped<ICdnService, BunnyCdnService>();
    }
}
