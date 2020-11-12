using HellEngine.Core.Services;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HellEngine.Core.Configuration
{
    public static class HellEngineConfigurator
    {
        public static void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            string configRootPath)
        {
            var assembly = typeof(HellEngineConfigurator).Assembly;
            var registrator = new ApplicationServicesRegistrator();

            registrator.RegisterApplicationOptions(
                services,
                configuration,
                configRootPath,
                assembly);
            registrator.RegisterApplicationServices(
                services,
                assembly);

            services.AddHostedService<HellEngineInitializer>();
        }
    }
}
