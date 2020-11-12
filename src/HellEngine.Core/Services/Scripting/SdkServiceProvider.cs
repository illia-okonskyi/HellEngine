using HellEngine.Core.Exceptions;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HellEngine.Core.Services.Scripting
{
    public interface ISdkServiceProvider : IDisposable
    {
        object GetService(Type service);
        TService GetService<TService>();
    }

    public class SdkServiceProvider : ISdkServiceProvider
    {
        private readonly IServiceScope serviceScope;
        private readonly bool unsafeMode;

        public SdkServiceProvider(
            IServiceScope serviceScope,
            bool unsafeMode)
        {
            this.serviceScope = serviceScope;
            this.unsafeMode = unsafeMode;
        }
        
        public void Dispose()
        {
            serviceScope.Dispose();
        }

        public object GetService(Type service)
        {
            ValidateAccess(service);
            return serviceScope.ServiceProvider.GetService(service);
        }

        public TService GetService<TService>()
        {
            ValidateAccess(typeof(TService));
            return serviceScope.ServiceProvider.GetService<TService>();
        }

        private void ValidateAccess(Type service)
        {
            if (unsafeMode)
            {
                return;
            }

            if (!service.IsDefined(typeof(SdkServiceAttribute), false))
            {
                throw new ServiceAccessDeniedException(service);
            }
        }
    }
}
