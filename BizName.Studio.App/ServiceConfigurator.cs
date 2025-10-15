using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BizName.Studio.App.Configurator;
using BizName.Studio.App.Services;
using BizName.Studio.App.Services.Impl;
using BizName.Studio.Contracts.Experiences;

namespace BizName.Studio.App;

/// <summary>
/// Service configurator for App layer business services
/// </summary>
public class ServiceConfigurator : IServiceConfigurator
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register all validators from the Contracts assembly
        services.AddValidatorsFromAssembly(typeof(Experience).Assembly);
        
        // Register business services
        services.AddScoped<ITenantService, TenantServiceImpl>();
        services.AddScoped<IWebsiteService, WebsiteServiceImpl>();
        services.AddScoped<IExperienceService, ExperienceServiceImpl>();
    }
}
