using HellEngine.Core.Constants;
using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Encoding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HellEngine.Core.Tests.Services.Assets
{
    public class AssetsManagerTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public AssetsOptions Options { get; }
            public AssetDescriptor Descriptor { get; }
            public string Locale { get; }
            public byte [] AssetBytes { get; }
            public string AssetDataStringEncoded { get; }
            public string AssetDataBase64Encoded { get; }
            #endregion

            #region Services
            public IOptions<AssetsOptions> OptionsService { get; }
            public ILogger<AssetsManager> Logger { get; }
            public IAssetDescriptorsCache AssetDescriptorsCache { get; }
            public IStringEncoder StringEncoder { get; }
            public IBase64Encoder Base64Encoder { get; }
            public ITextAssetDataProcessor TextAssetDataProcessor { get; }
            public IAssetManagerDataService DataService { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = AssetsOptions.Default;
                Descriptor = new AssetDescriptor
                {
                    Key = "d",
                    AssetPath = "root/d",
                    AssetType = AssetType.Text
                };
                Locale = Defaults.Locale;
                AssetBytes = new byte[] { 0x01, 0x02, 0x03 };
                AssetDataStringEncoded = "AssetDataStringEncoded";
                AssetDataBase64Encoded = "AssetDataBase64Encoded";

                OptionsService = Mock.Of<IOptions<AssetsOptions>>();
                Logger = Mock.Of<ILogger<AssetsManager>>();
                AssetDescriptorsCache = Mock.Of<IAssetDescriptorsCache>();
                StringEncoder = Mock.Of<IStringEncoder>();
                Base64Encoder = Mock.Of<IBase64Encoder>();
                TextAssetDataProcessor = Mock.Of<ITextAssetDataProcessor>();
                DataService = Mock.Of<IAssetManagerDataService>();

                Mock.Get(OptionsService).Setup(
                    m => m.Value)
                    .Returns(Options);

                Mock.Get(AssetDescriptorsCache).Setup(
                    m => m.GetAssetDescriptor(It.IsAny<string>()))
                    .Returns(Descriptor);

                Mock.Get(DataService).Setup(
                    m => m.ReadAssetBytesAsync(
                        Options.AssetsDir,
                        It.IsAny<Asset>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(AssetBytes);

                Mock.Get(TextAssetDataProcessor).Setup(
                    m => m.ProcessData(AssetBytes))
                    .Returns(AssetBytes);

                Mock.Get(StringEncoder).Setup(
                    m => m.EncodeToString(AssetBytes))
                    .Returns(AssetDataStringEncoded);

                Mock.Get(Base64Encoder).Setup(
                    m => m.Encode(AssetBytes))
                    .Returns(AssetDataBase64Encoded);
            }
        }
        #endregion

        [Fact]
        public async Task GetAsset_NotExpectedAssetType_Throws()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.AssetDescriptorsCache,
                context.StringEncoder,
                context.Base64Encoder,
                context.TextAssetDataProcessor,
                context.DataService);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidAssetTypeException>(
                () => sut.GetImageAsset(context.Descriptor, context.Locale));
        }

        public static IEnumerable<object[]> GetAsset_DataEncoding_Default_Mapped_Data()
        {
            yield return new object[] { AssetType.Text, AssetDataEncoding.Base64 };
            yield return new object[] { AssetType.Image, AssetDataEncoding.Base64 };
            yield return new object[] { AssetType.State, AssetDataEncoding.String };
            yield return new object[] { AssetType.Script, AssetDataEncoding.String };
        }

        [Theory]
        [MemberData(nameof(GetAsset_DataEncoding_Default_Mapped_Data))]
        public async Task GetAsset_DataEncoding_Default_Mapped(
            AssetType assetType,
            AssetDataEncoding expected)
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.AssetDescriptorsCache,
                context.StringEncoder,
                context.Base64Encoder,
                context.TextAssetDataProcessor,
                context.DataService);

            var key = "d";
            var descriptor = new AssetDescriptor
            {
                Key = key,
                AssetType = assetType,
                AssetPath = key
            };

            // Act
            var asset = assetType switch
            {
                AssetType.Text => await sut.GetTextAsset(descriptor, context.Locale),
                AssetType.Image => await sut.GetImageAsset(descriptor, context.Locale),
                AssetType.State => await sut.GetStateAsset(descriptor, context.Locale),
                AssetType.Script => await sut.GetScriptAsset(descriptor, context.Locale),
                _ => throw new NotImplementedException()
            };

            // Assert
            Assert.NotNull(asset);
            Assert.Equal(expected, asset.DataEncoding);
        }

        [Theory]
        [InlineData(AssetType.Text)]
        [InlineData(AssetType.Image)]
        [InlineData(AssetType.State)]
        [InlineData(AssetType.Script)]
        public async Task GetAsset_Returns(AssetType assetType)
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.AssetDescriptorsCache,
                context.StringEncoder,
                context.Base64Encoder,
                context.TextAssetDataProcessor,
                context.DataService);

            var key = "d";
            var descriptor = new AssetDescriptor
            {
                Key = key,
                AssetType = assetType,
                AssetPath = key
            };

            var dataEncoding = assetType switch
            {
                AssetType.Text => AssetDataEncoding.Base64,
                AssetType.Image => AssetDataEncoding.Base64,
                AssetType.State => AssetDataEncoding.String,
                AssetType.Script => AssetDataEncoding.String,
                _ => throw new NotImplementedException()
            };
            var data = assetType switch
            {
                AssetType.Text => context.AssetDataBase64Encoded,
                AssetType.Image => context.AssetDataBase64Encoded,
                AssetType.State => context.AssetDataStringEncoded,
                AssetType.Script => context.AssetDataStringEncoded,
                _ => throw new NotImplementedException()
            };

            // Act
            var asset1 = assetType switch
            {
                AssetType.Text => await sut.GetTextAsset(descriptor, context.Locale),
                AssetType.Image => await sut.GetImageAsset(descriptor, context.Locale),
                AssetType.State => await sut.GetStateAsset(descriptor, context.Locale),
                AssetType.Script => await sut.GetScriptAsset(descriptor, context.Locale),
                _ => throw new NotImplementedException()
            };
            var asset2 = assetType switch
            {
                AssetType.Text => await sut.GetTextAsset(descriptor, context.Locale),
                AssetType.Image => await sut.GetImageAsset(descriptor, context.Locale),
                AssetType.State => await sut.GetStateAsset(descriptor, context.Locale),
                AssetType.Script => await sut.GetScriptAsset(descriptor, context.Locale),
                _ => throw new NotImplementedException()
            };

            // Assert
            Assert.NotNull(asset1);
            Assert.NotNull(asset2);
            Assert.Same(descriptor, asset1.Descriptor);
            Assert.Same(descriptor, asset2.Descriptor);
            Assert.Equal(context.Locale, asset1.Locale);
            Assert.Equal(context.Locale, asset2.Locale);
            Assert.Equal(dataEncoding, asset1.DataEncoding);
            Assert.Equal(dataEncoding, asset2.DataEncoding);
            Assert.Equal(data, asset1.Data);
            Assert.Equal(data, asset2.Data);

            if (assetType == AssetType.Text)
            {
                Mock.Get(context.TextAssetDataProcessor).Verify(
                    m => m.ProcessData(context.AssetBytes),
                    Times.Exactly(2));
            }
        }
    }
}
