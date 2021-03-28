using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Vars;
using HellEngine.Core.Services.Vars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HellEngine.Core.Tests.Services.Vars
{
    public class VarsManagerTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public VarsManagerOptions Options { get; }
            #endregion

            #region Services
            public IOptions<VarsManagerOptions> OptionsService { get; }
            public ILogger<VarsManager> Logger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = VarsManagerOptions.Default;

                OptionsService = Mock.Of<IOptions<VarsManagerOptions>>();
                Logger = Mock.Of<ILogger<VarsManager>>();

                Mock.Get(OptionsService).Setup(
                    m => m.Value)
                    .Returns(Options);
            }
        }
        #endregion

        [Fact]
        public void Init()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var userName = "userName";
            var vars = new List<IVar>
            {
                new IntVar("v1", "v1-name", 0),
                new IntVar("v2", "v2-name", 1),
                new IntVar("v3", "v3-name", 2),
            };

            // Act
            sut.Init(userName, vars);
            var allVars = sut.GetAllVars();
            var userNameVar = sut.GetVar<StringVar>(context.Options.UserNameVarKey);

            // Assert
            Assert.Equal(vars.Count + 1, allVars.Count());
            Assert.Equal(userName, userNameVar.Value);
        }

        [Fact]
        public void AddVar_Success()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var key = "var";
            var aVar = new IntVar(key, "var-name", 15);

            // Act
            sut.AddVar(aVar);
            var actual = sut.GetVar<IntVar>(key);

            // Assert
            Assert.Same(aVar, actual);
        }

        [Fact]
        public void AddVar_SameVar_Throws()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var key = "var";
            var aVar = new IntVar(key, "var-name", 15);

            // Act + Assert
            sut.AddVar(aVar);
            Assert.Throws<VarAlreadyExistsException>(() => sut.AddVar(aVar));
        }

        [Fact]
        public void RemoveVar_NotFound_Throws()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            // Act + Assert
            Assert.Throws<VarNotFoundException>(() => sut.RemoveVar("key"));
        }

        [Fact]
        public void RemoveVar_Success()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var key = "var";
            var aVar = new IntVar(key, "var-name", 15);

            // Act + Assert
            sut.AddVar(aVar);
            sut.RemoveVar(key);
            Assert.Throws<VarNotFoundException>(() => sut.GetVar("key"));
        }

        [Fact]
        public void ClearVars()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            int varsCount = 3;

            var vars = new List<IntVar>();
            for (int i = 0; i < varsCount; ++i)
            {
                sut.AddVar(new IntVar($"key{i}", $"var-name{i}"));
            }

            // Act
            sut.ClearVars();

            // Assert
            Assert.Empty(sut.GetAllVars());
        }

        [Fact]
        public void ContainsVar()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var key = "key";

            // Act
            var beforeAdd = sut.ContainsVar(key);
            sut.AddVar(new IntVar(key, "var-name"));
            var afterAdd = sut.ContainsVar(key);

            // Assert
            Assert.False(beforeAdd);
            Assert.True(afterAdd);
        }

        [Fact]
        public void GetVar_Overload()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var key = "key";
            var aVar = new IntVar("key", "var-name", 15);

            // Act
            sut.AddVar(aVar);
            var actual1 = sut.GetVar(key) as IntVar;
            var actual2 = sut.GetVar<IntVar>(key);

            // Assert
            Assert.Same(aVar, actual1);
            Assert.Same(aVar, actual2);
        }

        [Fact]
        public void GetAllVars()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.OptionsService,
                context.Logger);

            var random = new Random();

            int varsCount = 5;
            var vars = new List<IVar>();
            for (int i = 0; i < varsCount; ++i)
            {
                vars.Add(new IntVar(
                    $"key{i}",
                    $"var-name{i}",
                    random.Next(-100, 100)));
            }

            // Act
            foreach (var v in vars)
            {
                sut.AddVar(v);
            }
            var actual = sut.GetAllVars();

            // Assert
            Assert.Equal(vars, actual);
        }
    }
}
