using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Vars;
using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Encoding;
using HellEngine.Core.Services.Vars;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HellEngine.Core.Tests.Services.Assets
{
    public class TextAssetDataProcessorTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public AssetsOptions Options { get; }
            public IVar Var1 { get; }
            public IVar Var2 { get; }
            public string NotFoundVarKey { get; }
            public byte[] Data { get; }
            public string StringData { get; }
            public string ExpectedProcessedString { get; }
            public byte[] ProcessedData { get; }
            #endregion

            #region Services
            public IOptions<AssetsOptions> OptionsService { get; }
            public IStringEncoder StringEncoder { get; }
            public IVarsManager VarsManager { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = AssetsOptions.Default;
                Var1 = new IntVar("var1", "var1-name", 15);
                Var2 = new StringVar("var2", "var2-name", "hello");
                NotFoundVarKey = "var3";
                Data = new byte[] { 0x01, 0x02, 0x03 };
                StringData = "1 {var=var1} 22 {var=var3} 333 {var=var2} 4444";
                ExpectedProcessedString =
                    $"1 <span class=\"{Options.VarValueSpanClass}\">{Var1.DisplayString}</span> "+
                    $"22 <span class=\"{Options.VarValueSpanClass}\">VAR NOT FOUND var3</span> " +
                    $"333 <span class=\"{Options.VarValueSpanClass}\">{Var2.DisplayString}</span> " +
                    $"4444";
                ProcessedData = new byte[] { 0x04, 0x05, 0x06 };

                OptionsService = Mock.Of<IOptions<AssetsOptions>>();
                StringEncoder = Mock.Of<IStringEncoder>();
                VarsManager = Mock.Of<IVarsManager>();

                Mock.Get(StringEncoder).Setup(
                    m => m.EncodeToString(Data))
                    .Returns(StringData);

                Mock.Get(StringEncoder).Setup(
                    m => m.EncodeFromString(ExpectedProcessedString))
                    .Returns(ProcessedData);

                Mock.Get(VarsManager).Setup(
                    m => m.GetVar(Var1.Key))
                    .Returns(Var1);

                Mock.Get(VarsManager).Setup(
                    m => m.GetVar(Var2.Key))
                    .Returns(Var2);

                Mock.Get(VarsManager).Setup(
                    m => m.GetVar(NotFoundVarKey))
                    .Throws(new VarNotFoundException(NotFoundVarKey));
            }
        }
        #endregion

        [Fact]
        public void ProcessData()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new TextAssetDataProcessor(
                context.OptionsService,
                context.StringEncoder,
                context.VarsManager);

            // Act
            var actual = sut.ProcessData(context.Data);

            // Assert
            Assert.Equal(context.ProcessedData, actual);

            Mock.Get(context.StringEncoder).Verify(
                m => m.EncodeToString(context.Data),
                Times.Once);

            Mock.Get(context.StringEncoder).Verify(
                m => m.EncodeFromString(context.ExpectedProcessedString),
                Times.Once);
        }
    }
}
