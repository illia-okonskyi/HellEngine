using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Core.Services.Encoding;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.Assets
{
    public interface IAssetsManager
    {
        Task<Asset> GetTextAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default);
        Task<Asset> GetTextAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default);

        Task<Asset> GetImageAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default);
        Task<Asset> GetImageAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default);

        Task<Asset> GetStateAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default);
        Task<Asset> GetStateAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default);

        Task<Asset> GetScriptAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default);
        Task<Asset> GetScriptAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default);
    }

    [ApplicationService(
        Service = typeof(IAssetsManager),
        Lifetime = ApplicationServiceLifetime.Scoped)]
    public class AssetsManager : IAssetsManager
    {
        private readonly AssetsOptions options;
        private readonly ILogger<AssetsManager> logger;
        private readonly IAssetDescriptorsCache assetDescriptorsCache;
        private readonly IStringEncoder stringEncoder;
        private readonly IBase64Encoder base64Encoder;
        private readonly IAssetManagerDataService dataService;

        public AssetsManager(
            IOptions<AssetsOptions> options,
            ILogger<AssetsManager> logger,
            IAssetDescriptorsCache assetDescriptorsCache,
            IStringEncoder stringEncoder,
            IBase64Encoder base64Encoder,
            IAssetManagerDataService dataService)
        {
            this.options = options.Value?? AssetsOptions.Default;
            this.logger = logger;
            this.assetDescriptorsCache = assetDescriptorsCache;
            this.stringEncoder = stringEncoder;
            this.base64Encoder = base64Encoder;
            this.dataService = dataService;
        }

        public async Task<Asset> GetTextAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                locale,
                AssetType.Text,
                cancellationToken);
        }

        public async Task<Asset> GetTextAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                locale,
                AssetType.Text,
                cancellationToken);
        }

        public async Task<Asset> GetImageAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                locale,
                AssetType.Image,
                cancellationToken);
        }

        public async Task<Asset> GetImageAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                locale,
                AssetType.Image,
                cancellationToken);
        }

        public async Task<Asset> GetStateAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                locale,
                AssetType.State,
                cancellationToken);
        }

        public async Task<Asset> GetStateAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                locale,
                AssetType.State,
                cancellationToken);
        }

        public async Task<Asset> GetScriptAsset(
            AssetDescriptor descriptor,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                locale,
                AssetType.Script,
                cancellationToken);
        }

        public async Task<Asset> GetScriptAsset(
            string key,
            string locale,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                locale,
                AssetType.Script,
                cancellationToken);
        }

        private async Task<Asset> GetAsset(
            AssetDescriptor descriptor,
            string locale,
            AssetType expectedAssetType,
            CancellationToken cancellationToken = default)
        {
            ValidateAssetType(descriptor, expectedAssetType);
            var asset = new Asset(descriptor, locale);
            await LoadAssetData(asset, cancellationToken);
            return asset;
        }

        private async Task<Asset> GetAsset(
            string key,
            string locale,
            AssetType expectedAssetType,
            CancellationToken cancellationToken = default)
        {
            var descriptor = assetDescriptorsCache.GetAssetDescriptor(key);
            return await GetAsset(
                descriptor,
                locale,
                expectedAssetType,
                cancellationToken);
        }

        private void ValidateAssetType(AssetDescriptor descriptor, AssetType expected)
        {
            if (descriptor.AssetType != expected)
            {
                throw new InvalidAssetTypeException(
                    descriptor.Key,
                    expected,
                    descriptor.AssetType);
            }
        }

        private async Task LoadAssetData(
            Asset asset,
            CancellationToken cancellationToken = default)
        {
            var dataEncoding = GetAssetDataEncoding(asset);
            var assetBytes = await dataService.ReadAssetBytesAsync(
                options.AssetsDir,
                asset,
                cancellationToken);

            string data = dataEncoding switch
            {
                AssetDataEncoding.String => stringEncoder.EncodeToString(assetBytes),
                AssetDataEncoding.Base64 => base64Encoder.Encode(assetBytes),
                _ => throw new NotSupportedException($"Data encoding {dataEncoding} is not supported")
            };

            asset.SetData(dataEncoding, data);
        }

        private AssetDataEncoding GetAssetDataEncoding(Asset asset)
        {
            return asset.Descriptor.AssetType switch
            {
                AssetType.Text => AssetDataEncoding.Base64,
                AssetType.Image => AssetDataEncoding.Base64,
                AssetType.State => AssetDataEncoding.String,
                AssetType.Script => AssetDataEncoding.String,
                _ => throw new NotSupportedException(
                    $"Asset type {asset.Descriptor.AssetType} is not supported")
            };
        }
    }
}
