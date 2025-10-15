using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BizName.Studio.App.Configurator;

public static class ServiceConfiguratorInvoker
{
    public static void RegisterAll(IServiceCollection services, IConfiguration configuration)
    {
        // Load all assemblies from the bin folder
        var binPath = AppDomain.CurrentDomain.BaseDirectory;
        var assemblyFiles = Directory.GetFiles(binPath, "*.dll");

        foreach (var assemblyFile in assemblyFiles)
        {
            try
            {
                // Load assembly if not already loaded
                var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName == assemblyName.FullName))
                {
                    Assembly.Load(assemblyName);
                }
            }
            catch (BadImageFormatException)
            {
                // Skip files that are not .NET assemblies (like native DLLs)
                continue;
            }
            catch (Exception)
            {
                // Skip any other problematic files
                continue;
            }
        }

        // Filter assemblies that start with "BizName.Studio"
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.GetName().Name.StartsWith("BizName.Studio", StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Iterate through all assemblies to find types implementing IServiceConfigurator
        var configurators = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IServiceConfigurator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        foreach (var configuratorType in configurators)
        {
            // Create an instance of the configurator
            if (Activator.CreateInstance(configuratorType) is IServiceConfigurator configurator)
            {
                // Invoke the Register method
                configurator.Register(services, configuration);
            }
            else
            {
                throw new InvalidOperationException($"Could not create an instance of {configuratorType.FullName}");
            }
        }
    }
}
