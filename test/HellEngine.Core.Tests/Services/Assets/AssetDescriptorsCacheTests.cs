using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Core.Services.Assets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HellEngine.Core.Tests.Services.Assets
{
    public class AssetDescriptorsCacheTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public AssetsOptions Options { get; }
            public Dictionary<string, AssetDescriptor> Descriptors { get; }
            #endregion

            #region Services
            public IOptions<AssetsOptions> OptionsService { get; }
            public ILogger<AssetDescriptorsCache> Logger { get; }
            public IAssetDescriptorsCacheDataService DataService { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = AssetsOptions.Default;
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

                OptionsService = Mock.Of<IOptions<AssetsOptions>>();
                Logger = Mock.Of<ILogger<AssetDescriptorsCache>>();
                DataService = Mock.Of<IAssetDescriptorsCacheDataService>();

                Mock.Get(OptionsService).Setup(
                    m => m.Value)
                    .Returns(Options);

                Mock.Get(DataService).Setup(
                    m => m.LoadDescriptorsAsync(Options.AssetsDir, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Descriptors);
            }
        }
        #endregion

        [Fact]
        public async Task Init()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetDescriptorsCache(
                context.OptionsService,
                context.Logger,
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
        public async Task GetAssetDescriptor_NotFoundThrows()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new AssetDescriptorsCache(
                context.OptionsService,
                context.Logger,
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
            var sut = new AssetDescriptorsCache(
                context.OptionsService,
                context.Logger,
                context.DataService);

            var descriptor = context.Descriptors["d2"];

            // Act
            await sut.Init();
            var result = sut.GetAssetDescriptor(descriptor.Key);

            // Assert
            Assert.Same(descriptor, result);
        }
    }
}
