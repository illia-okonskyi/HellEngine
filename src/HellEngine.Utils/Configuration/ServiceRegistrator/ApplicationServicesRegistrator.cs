using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace HellEngine.Utils.Configuration.ServiceRegistrator
{
    public class ApplicationServicesRegistrator
    {
        public void RegisterApplicationServices(IServiceCollection services, Assembly assembly)
        {
            var serviceDescriptors = assembly.ExportedTypes
                .Where(t => t.IsClass && t.IsDefined(typeof(ApplicationServiceAttribute), false))
                .Select(i =>
                {
                    var attr = i.GetCustomAttribute<ApplicationServiceAttribute>();

                    var serviceLifetime = attr.Lifetime switch
                    {
                        ApplicationServiceLifetime.Transient => ServiceLifetime.Transient,
                        ApplicationServiceLifetime.Scoped => ServiceLifetime.Scoped,
                        ApplicationServiceLifetime.Singletone => ServiceLifetime.Singleton,
                        _ => throw new NotImplementedException()
                    };

                    return new ServiceDescriptor(attr.Service, i, serviceLifetime);
                });

            services.Add(serviceDescriptors);
        }

        public void RegisterApplicationOptions(
            IServiceCollection services,
            IConfiguration configuration,
            string rootPath,
            Assembly assembly)
        {
            var addOptionsOpenMi = typeof(OptionsServiceCollectionExtensions)
                .GetMethod(
                "AddOptions",
                1,
                new Type[] { typeof(IServiceCollection) });
            var bindOpenMi = typeof(OptionsBuilderConfigurationExtensions)
                .GetMethods()
                .Where(mi => mi.Name == "Bind" && mi.GetParameters().Count() == 2)
                .FirstOrDefault();
            if (addOptionsOpenMi == null || bindOpenMi == null)
            {
                throw new InvalidOperationException(
                    "Failed to find appropriate methods for binding options");
            }

            var contexts = assembly.ExportedTypes
                .Where(tOptions =>
                    tOptions.IsClass && tOptions.IsDefined(typeof(ApplicationOptionsAttribute), false))
                .Select(tOptions =>
                {
                    var attr = tOptions.GetCustomAttribute<ApplicationOptionsAttribute>();
                    var addOptions = addOptionsOpenMi.MakeGenericMethod(tOptions);
                    var bind = bindOpenMi.MakeGenericMethod(tOptions);
                    var configSection = configuration.GetSection($"{rootPath}:{attr.Path}");

                    return new
                    {
                        AddOptions = addOptions,
                        Bind = bind,
                        ConfigSection = configSection
                    };
                });

            foreach (var context in contexts)
            {
                // NOTE: invoking services.AddOptions<TOptions>().Bind(configSection)
                var optionsBuilder = context.AddOptions
                    .Invoke(null, new object[] { services });
                context.Bind
                    .Invoke(null, new object[] { optionsBuilder, context.ConfigSection });
            }
        }
    }
}
