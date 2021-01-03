using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.Assets
{
    public interface IAssetDescriptorsCache
    {
        Task Init(CancellationToken cancellationToken = default);
        AssetDescriptor GetAssetDescriptor(string key);
    }

    [ApplicationService(
        Service = typeof(IAssetDescriptorsCache),
        Lifetime = ApplicationServiceLifetime.Singletone)]
    public class AssetDescriptorsCache : IAssetDescriptorsCache
    {
        private readonly AssetsOptions options;
        private readonly ILogger<AssetDescriptorsCache> logger;
        private readonly IAssetDescriptorsCacheDataService dataService;

        private Dictionary<string, AssetDescriptor> descriptors;

        public AssetDescriptorsCache(
            IOptions<AssetsOptions> options,
            ILogger<AssetDescriptorsCache> logger,
            IAssetDescriptorsCacheDataService dataService)
        {
            this.options = options.Value ?? AssetsOptions.Default;
            this.logger = logger;
            this.dataService = dataService;
        }

        public async Task Init(CancellationToken cancellationToken = default)
        {
            descriptors = await dataService.LoadDescriptorsAsync(
                options.AssetsDir,
                cancellationToken);
        }

        public AssetDescriptor GetAssetDescriptor(string key)
        {
            if (!descriptors.TryGetValue(key, out AssetDescriptor result))
            {
                throw new AssetDescriptorNotFoundException(key);
            }

            return result;
        }
    }
}
