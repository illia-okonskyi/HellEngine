using HellEngine.Core.Models.Assets;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using HellEngine.Utils.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.Assets
{
    public interface IAssetDescriptorsCacheDataService
    {
        Task<Dictionary<string, AssetDescriptor>> LoadDescriptorsAsync(
            string rootPath,
            CancellationToken cancellationToken = default);
    }

    [ApplicationService(Service = typeof(IAssetDescriptorsCacheDataService))]
    public class AssetDescriptorsCacheDataService : IAssetDescriptorsCacheDataService
    {
        public async Task<Dictionary<string, AssetDescriptor>> LoadDescriptorsAsync(
            string rootPath,
            CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<string, AssetDescriptor>();

            var paths = Directory.GetFiles(
                rootPath
                    .AddPath(Constants.Defaults.AssetsDescriptorsDir)
                    .NormalizeDirectorySeparators(),
                "*.desc",
                SearchOption.AllDirectories);


            foreach (var path in paths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await File.ReadAllTextAsync(path, cancellationToken);
                var descriptor = JsonConvert.DeserializeObject<AssetDescriptor>(data);
                result.Add(descriptor.Key, descriptor);
            }

            return result;
        }

    }
}
