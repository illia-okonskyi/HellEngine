using HellEngine.Core.Constants;
using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Assets;
using HellEngine.Core.Models.Vars;
using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Encoding;
using HellEngine.Core.Services.Vars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HellEngine.Core.Tests.Services.Vars
{
    public class VarsManagerTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            #endregion

            #region Services
            public ILogger<VarsManager> Logger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Logger = Mock.Of<ILogger<VarsManager>>();
            }
        }
        #endregion

        [Fact]
        public void AddVar_Success()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.Logger);

            var key = "var";
            var aVar = new IntVar(key, "var-name", 0, 15);

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
                context.Logger);

            var key = "var";
            var aVar = new IntVar(key, "var-name", 0, 15);

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
                context.Logger);

            var key = "var";
            var aVar = new IntVar(key, "var-name", 0, 15);

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
            for (int i = 0; i < varsCount; ++i)
            {
                Assert.Throws<VarNotFoundException>(() => sut.GetVar($"key{i}"));
            }
        }

        [Fact]
        public void GetVar_Overload()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new VarsManager(
                context.Logger);

            var key = "key";
            var aVar = new IntVar("key", "var-name", 3, 15);

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
                context.Logger);

            var random = new Random();

            int varsCount = 5;
            var vars = new List<IVar>();
            for (int i = 0; i < varsCount; ++i)
            {
                vars.Add(new IntVar(
                    $"key{i}",
                    $"var-name{i}",
                    (uint)i,
                    random.Next(-100, 100)));
            }
            vars = vars
                .OrderBy(v => v.SetIndex)
                .ToList();

            // Act
            foreach (var v in vars.OrderByDescending(v => v.SetIndex))
            {
                sut.AddVar(v);
            }
            var actual = sut.GetAllVars();

            // Assert
            Assert.Equal(vars, actual);
        }
    }
}
