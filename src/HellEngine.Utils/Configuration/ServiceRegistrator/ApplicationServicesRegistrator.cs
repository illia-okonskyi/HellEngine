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
    }
}
