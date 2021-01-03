using HellEngine.Core.Exceptions;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HellEngine.Core.Services.Scripting
{
    public interface ISdkUtilServiceProvider : IDisposable
    {
        object GetService(Type service);
        TService GetService<TService>();
    }

    public class SdkUtilServiceProvider : ISdkUtilServiceProvider
    {
        private readonly IServiceScope serviceScope;

        public SdkUtilServiceProvider(
            IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
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
            if (!service.IsDefined(typeof(SdkUtilServiceAttribute), false))
            {
                throw new ServiceAccessDeniedException(service);
            }
        }
    }
}
