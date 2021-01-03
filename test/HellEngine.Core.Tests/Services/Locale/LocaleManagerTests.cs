using HellEngine.Core.Services.Locale;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HellEngine.Core.Tests.Services.Locale
{
    public class LocaleManagerTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public LocaleManagerOptions Options { get; }
            #endregion

            #region Services
            public IOptions<LocaleManagerOptions> OptionsService { get; }
            public ILogger<LocaleManager> Logger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = LocaleManagerOptions.Default;

                OptionsService = Mock.Of<IOptions<LocaleManagerOptions>>();
                Logger = Mock.Of<ILogger<LocaleManager>>();

                Mock.Get(OptionsService).Setup(
                    m => m.Value)
                    .Returns(Options);
            }
        }
        #endregion

        [Fact]
        public void StartingLocale()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new LocaleManager(
                context.OptionsService,
                context.Logger);

            // Act
            var actual = sut.GetLocale();

            // Assert
            Assert.Equal(context.Options.StartingLocale, actual);
        }


        [Fact]
        public void SetGetLocale()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new LocaleManager(
                context.OptionsService,
                context.Logger);

            var locale = "ru-ru";

            // Act
            sut.SetLocale(locale);
            var actual = sut.GetLocale();

            // Assert
            Assert.Equal(locale, actual);
        }
    }
}
