using HellEngine.Core.Services;
using HellEngine.Core.Services.Scripting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HellEngine.Core.Tests.Services.Scripting
{
    public class ScriptHostTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public string HelloString { get; }
            #endregion

            #region Services
            public IOptions<ScriptHostOptions> Options { get; }
            public ILogger<ScriptHost> Logger { get; }
            public ILogger<HellScriptContext> ScriptLogger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = Mock.Of<IOptions<ScriptHostOptions>>();
                Logger = Mock.Of<ILogger<ScriptHost>>();
                ScriptLogger = Mock.Of<ILogger<HellScriptContext>>();

                Mock.Get(Options).Setup(
                    m => m.Value)
                    .Returns(new ScriptHostOptions { });
            }
        }
        #endregion

        [Fact]
        public async Task CreateAndRunScript()
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new ScriptHost(context.Options, context.Logger, context.ScriptLogger);

            var scriptName = "testScript";
            var a = 3;
            var b = 7;
            var code = $@"
                using Microsoft.Extensions.Logging;
                var a = {a};
                var b = {b};
                var c = a + b;
                Logger.LogDebug($""Running script {{ScriptName}}; Sum = {{c}}"");";

            // Act + Assert
            var script = sut.CreateScript(scriptName, code);

            Assert.NotNull(script);
            Assert.Equal(scriptName, script.Name);
            Assert.NotNull(script.Script);
            Assert.Equal(typeof(HellScriptContext), script.ContextType);

            await sut.RunScript(script);

            Func<object, Type, bool> expectedState =
                (v, t) => v.ToString().CompareTo($"Running script {scriptName}; Sum = {a + b}") == 0;
            Mock.Get(context.ScriptLogger).Verify(
                m => m.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => expectedState(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
