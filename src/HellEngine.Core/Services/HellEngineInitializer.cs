using HellEngine.Core.Services.Assets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services
{
    public class HellEngineInitializer : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public HellEngineInitializer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = serviceProvider.CreateScope();
            var scopedServiceProvider = scope.ServiceProvider;

            var assetsManager = scopedServiceProvider.GetRequiredService<IAssetsManager>();

            await assetsManager.Init(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
