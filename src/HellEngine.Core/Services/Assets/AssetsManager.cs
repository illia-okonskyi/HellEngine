using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Core.Services.Encoding;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.Assets
{
    public interface IAssetsManager
    {
        Task Init(CancellationToken cancellationToken = default);

        string GetLocale();
        void SetLocale(string locale);

        AssetDescriptor GetAssetDescriptor(string key);

        Task<Asset> GetTextAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);
        Task<Asset> GetTextAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);

        Task<Asset> GetImageAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);
        Task<Asset> GetImageAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);

        Task<Asset> GetStateAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);
        Task<Asset> GetStateAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);

        Task<Asset> GetScriptAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);
        Task<Asset> GetScriptAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default);
    }

    [ApplicationOptions("AssetsManager")]
    public class AssetsManagerOptions
    {
        public static AssetsManagerOptions Default => new AssetsManagerOptions();

        public string AssetsDir { get; set; } = Constants.Defaults.AssetsDir;
        public string StartingLocale { get; set; } = Constants.Defaults.Locale;
    }

    [ApplicationService(
        Service = typeof(IAssetsManager),
        Lifetime = ApplicationServiceLifetime.Singletone)]
    public class AssetsManager : IAssetsManager
    {
        private readonly AssetsManagerOptions options;
        private readonly ILogger<AssetsManager> logger;
        private readonly IStringEncoder stringEncoder;
        private readonly IBase64Encoder base64Encoder;
        private readonly IAssetManagerDataService dataService;

        private Dictionary<string, AssetDescriptor> descriptors;
        private string locale;

        public AssetsManager(
            IOptions<AssetsManagerOptions> options,
            ILogger<AssetsManager> logger,
            IStringEncoder stringEncoder,
            IBase64Encoder base64Encoder,
            IAssetManagerDataService dataService)
        {
            this.options = options.Value?? AssetsManagerOptions.Default;
            this.logger = logger;
            this.stringEncoder = stringEncoder;
            this.base64Encoder = base64Encoder;
            this.dataService = dataService;

            locale = this.options.StartingLocale;
        }

        public async Task Init(CancellationToken cancellationToken = default)
        {
            descriptors = await dataService.LoadDescriptorsAsync(
                options.AssetsDir,
                cancellationToken);
        }

        public string GetLocale()
        {
            return locale;
        }

        public void SetLocale(string locale)
        {
            this.locale = locale;
        }

        public AssetDescriptor GetAssetDescriptor(string key)
        {
            if (!descriptors.TryGetValue(key, out AssetDescriptor result))
            {
                throw new AssetDescriptorNotFoundException(key);
            }

            return result;
        }

        public async Task<Asset> GetTextAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                AssetType.Text,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetTextAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                AssetType.Text,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetImageAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                AssetType.Image,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetImageAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                AssetType.Image,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetStateAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                AssetType.State,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetStateAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                AssetType.State,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetScriptAsset(
            AssetDescriptor descriptor,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                descriptor,
                AssetType.Script,
                forceDataEncoding,
                cancellationToken);
        }

        public async Task<Asset> GetScriptAsset(
            string key,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            return await GetAsset(
                key,
                AssetType.Script,
                forceDataEncoding,
                cancellationToken);
        }

        private async Task<Asset> GetAsset(
            AssetDescriptor descriptor,
            AssetType expectedAssetType,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            ValidateAssetType(descriptor, expectedAssetType);
            var asset = new Asset(descriptor, locale);
            await LoadAssetData(asset, forceDataEncoding, cancellationToken);
            return asset;
        }

        private async Task<Asset> GetAsset(
            string key,
            AssetType expectedAssetType,
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            var descriptor = GetAssetDescriptor(key);
            return await GetAsset(
                descriptor,
                expectedAssetType,
                forceDataEncoding,
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
            AssetDataEncoding? forceDataEncoding = null,
            CancellationToken cancellationToken = default)
        {
            var dataEncoding = forceDataEncoding?? GetDefaultAssetDataEncoding(asset);
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

        private AssetDataEncoding GetDefaultAssetDataEncoding(Asset asset)
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
