using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Encoding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
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
            public AssetsManagerOptions Options { get; }
            public Dictionary<string, AssetDescriptor> Descriptors { get; }
            public byte [] AssetBytes { get; }
            public string AssetDataStringEncoded { get; }
            public string AssetDataBase64Encoded { get; }
            #endregion

            #region Services
            public IOptions<AssetsManagerOptions> OptionsService { get; }
            public ILogger<AssetsManager> Logger { get; }
            public IStringEncoder StringEncoder { get; }
            public IBase64Encoder Base64Encoder { get; }
            public IAssetManagerDataService DataService { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = AssetsManagerOptions.Default;
                Descriptors = new Dictionary<string, AssetDescriptor>();
                for (int i = 0; i < 5; ++i)
                {
                    var key = $"d{i}";
                    var path = $"p{i}";
                    Descriptors.Add(
                        key,
                        new AssetDescriptor
                        {
                            Key = key,
                            AssetType = AssetType.Text,
                            AssetPath = path
                        });
                }
                AssetBytes = new byte[] { 0x01, 0x02, 0x03 };
                AssetDataStringEncoded = "AssetDataStringEncoded";
                AssetDataBase64Encoded = "AssetDataBase64Encoded";

                OptionsService = Mock.Of<IOptions<AssetsManagerOptions>>();
                Logger = Mock.Of<ILogger<AssetsManager>>();
                StringEncoder = Mock.Of<IStringEncoder>();
                Base64Encoder = Mock.Of<IBase64Encoder>();
                DataService = Mock.Of<IAssetManagerDataService>();

                Mock.Get(OptionsService).Setup(
                    m => m.Value)
                    .Returns(Options);

                Mock.Get(DataService).Setup(
                    m => m.LoadDescriptorsAsync(Options.AssetsDir, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Descriptors);

                Mock.Get(DataService).Setup(
                    m => m.ReadAssetBytesAsync(
                        Options.AssetsDir,
                        It.IsAny<Asset>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(AssetBytes);

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
        public async Task Init()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            // Act
            await sut.Init();

            // Assert
            Mock.Get(context.DataService).Verify(
                m => m.LoadDescriptorsAsync(
                    context.Options.AssetsDir, default),
                Times.Once);
        }

        [Fact]
        public void SetGetLocale()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var locale = "en-us";

            // Act
            var startingLocale = sut.GetLocale();
            sut.SetLocale(locale);
            var actualLocale = sut.GetLocale();

            // Assert
            Assert.Equal(context.Options.StartingLocale, startingLocale);
            Assert.Equal(locale, actualLocale);
        }

        [Fact]
        public async Task GetAssetDescriptor_NotFoundThrows()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var key = $"d{context.Descriptors.Count}";

            // Act + Assert
            await sut.Init();
            Assert.Throws<AssetDescriptorNotFoundException>(() => sut.GetAssetDescriptor(key));
        }

        [Fact]
        public async Task GetAssetDescriptor_Found()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var descriptor = context.Descriptors["d2"];

            // Act
            await sut.Init();
            var result = sut.GetAssetDescriptor(descriptor.Key);

            // Assert
            Assert.Same(descriptor, result);
        }

        [Fact]
        public async Task GetAsset_NotExpectedAssetType_Throws()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var descriptor = context.Descriptors["d2"];

            // Act + Assert
            await sut.Init();
            await Assert.ThrowsAsync<InvalidAssetTypeException>(() => sut.GetImageAsset(descriptor));
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
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var key = "d";
            var descriptor = new AssetDescriptor
            {
                Key = key,
                AssetType = assetType,
                AssetPath = key
            };

            context.Descriptors.Clear();
            context.Descriptors.Add(key, descriptor);

            // Act
            await sut.Init();
            var asset = assetType switch
            {
                AssetType.Text => await sut.GetTextAsset(descriptor),
                AssetType.Image => await sut.GetImageAsset(descriptor),
                AssetType.State => await sut.GetStateAsset(descriptor),
                AssetType.Script => await sut.GetScriptAsset(descriptor),
                _ => throw new NotImplementedException()
            };

            // Assert
            Assert.NotNull(asset);
            Assert.Equal(expected, asset.DataEncoding);
        }

        [Fact]
        public async Task GetAsset_DataEncoding_Force_Applied()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetsManager(
                context.OptionsService,
                context.Logger,
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var key = "d";
            var descriptor = new AssetDescriptor
            {
                Key = key,
                AssetType = AssetType.Text,
                AssetPath = key
            };

            context.Descriptors.Clear();
            context.Descriptors.Add(key, descriptor);

            // Act
            await sut.Init();
            var asset = await sut.GetTextAsset(descriptor, AssetDataEncoding.String);

            // Assert
            Assert.NotNull(asset);
            Assert.Equal(AssetDataEncoding.String, asset.DataEncoding);
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
                context.StringEncoder,
                context.Base64Encoder,
                context.DataService);

            var key = "d";
            var descriptor = new AssetDescriptor
            {
                Key = key,
                AssetType = assetType,
                AssetPath = key
            };

            context.Descriptors.Clear();
            context.Descriptors.Add(key, descriptor);
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
            await sut.Init();
            var asset1 = assetType switch
            {
                AssetType.Text => await sut.GetTextAsset(descriptor),
                AssetType.Image => await sut.GetImageAsset(descriptor),
                AssetType.State => await sut.GetStateAsset(descriptor),
                AssetType.Script => await sut.GetScriptAsset(descriptor),
                _ => throw new NotImplementedException()
            };
            var asset2 = assetType switch
            {
                AssetType.Text => await sut.GetTextAsset(descriptor),
                AssetType.Image => await sut.GetImageAsset(descriptor),
                AssetType.State => await sut.GetStateAsset(descriptor),
                AssetType.Script => await sut.GetScriptAsset(descriptor),
                _ => throw new NotImplementedException()
            };

            // Assert
            Assert.NotNull(asset1);
            Assert.NotNull(asset2);
            Assert.Same(descriptor, asset1.Descriptor);
            Assert.Same(descriptor, asset2.Descriptor);
            Assert.Equal(context.Options.StartingLocale, asset1.Locale);
            Assert.Equal(context.Options.StartingLocale, asset2.Locale);
            Assert.True(asset1.IsDefaultLocale);
            Assert.True(asset2.IsDefaultLocale);
            Assert.Equal(dataEncoding, asset1.DataEncoding);
            Assert.Equal(dataEncoding, asset2.DataEncoding);
            Assert.Equal(data, asset1.Data);
            Assert.Equal(data, asset2.Data);
        }
    }
}
