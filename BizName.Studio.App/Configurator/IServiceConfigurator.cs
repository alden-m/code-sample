using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BizName.Studio.App.Configurator;

public interface IServiceConfigurator
{
    void Register(IServiceCollection services, IConfiguration configuration);
}
