using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using HellEngine.Utils.Extensions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.Assets
{
    public interface IAssetManagerDataService
    {
        Task<byte[]> ReadAssetBytesAsync(
            string rootPath,
            Asset asset,
            CancellationToken cancellationToken = default);
    }

    [ApplicationService(Service = typeof(IAssetManagerDataService))]
    public class AssetManagerDataService : IAssetManagerDataService
    {
        public async Task<byte[]> ReadAssetBytesAsync(
            string rootPath,
            Asset asset,
            CancellationToken cancellationToken = default)
        {
            return await File.ReadAllBytesAsync(
                GetAssetLocalizedPath(rootPath, asset),
                cancellationToken);
        }

        private string GetAssetLocalizedPath(string rootPath, Asset asset)
        {
            var localizedPath = rootPath
                .AddPath(Constants.Defaults.AssetsDataDir)
                .AddPath(asset.Locale)
                .AddPath(asset.Descriptor.AssetPath)
                .NormalizeDirectorySeparators();
            if (File.Exists(localizedPath))
            {
                return localizedPath;
            }

            var defaultPath = rootPath
                .AddPath(Constants.Defaults.AssetsDataDir)
                .AddPath(Constants.Defaults.Locale)
                .AddPath(asset.Descriptor.AssetPath)
                .NormalizeDirectorySeparators();
            if (!File.Exists(defaultPath))
            {
                throw new AssetNotFoundException(
                    asset.Descriptor.Key,
                    asset.Descriptor.AssetPath,
                    asset.Locale);
            }

            return defaultPath;
        }
    }
}

