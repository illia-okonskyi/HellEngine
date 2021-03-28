using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.StateMachine;
using HellEngine.Core.Services.StateMachine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HellEngine.Core.Tests.Services.StateMachine
{
    public class StateMachineManagerTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            public StateMachineOptions Options { get; }
            public string TransitionKey { get; }
            public string State1Key { get; }
            public string State2Key { get; }
            public Transition Transition { get; }
            public State InitialState { get; }
            public State FinalState { get; }
            public State State1 { get; }
            public State State2 { get; }

            #endregion

            #region Services
            public IOptions<StateMachineOptions> OptionsService { get; }
            public ILogger<StateMachineManager> Logger { get; }
            public IStateMachineManagerDataService DataService { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = StateMachineOptions.Default;
                TransitionKey = "transtion";
                State1Key = "state1";
                State2Key = "state2";
                Transition = new Transition { Key = TransitionKey, NextStateKey = State1Key };
                InitialState = new State
                {
                    Key = Options.InitialStateKey,
                    Transitions = new List<Transition> { Transition }
                };
                FinalState = new State { Key = Options.FinalStateKey };
                State1 = new State { Key = State1Key };
                State2 = new State { Key = State2Key };

                OptionsService = Mock.Of<IOptions<StateMachineOptions>>();
                Logger = Mock.Of<ILogger<StateMachineManager>>();
                DataService = Mock.Of<IStateMachineManagerDataService>();

                Mock.Get(OptionsService).Setup(
                    m => m.Value)
                    .Returns(Options);

                Mock.Get(DataService).Setup(
                    m => m.LoadState(Options.InitialStateKey, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(InitialState);
                Mock.Get(DataService).Setup(
                    m => m.LoadState(Options.FinalStateKey, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(FinalState);
                Mock.Get(DataService).Setup(
                    m => m.LoadState(State1Key, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(State1);
                Mock.Get(DataService).Setup(
                    m => m.LoadState(State2Key, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(State2);
            }
        }
        #endregion

        [Fact]
        public void GetCurrentState_Default_Null()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act
            var currentState = sut.GetCurrentState();

            // Assert
            Assert.Null(currentState);
        }

        [Fact]
        public async Task SetInitialState()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act
            await sut.SetInitialState();
            var currentState = sut.GetCurrentState();

            // Assert
            Assert.Same(context.InitialState, currentState);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnEnterStateScript(context.InitialState, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SetFinalState()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act
            await sut.SetInitialState();
            await sut.SetFinalState();
            var currentState = sut.GetCurrentState();

            // Assert
            Assert.Same(context.FinalState, currentState);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnLeaveStateScript(context.InitialState, It.IsAny<CancellationToken>()),
                Times.Once);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnEnterStateScript(context.FinalState, It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetCurrentState(bool leaveCurrentState)
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act
            await sut.SetInitialState();
            await sut.SetCurrentState(context.State1Key, leaveCurrentState);
            var currentState = sut.GetCurrentState();

            // Assert
            Assert.Same(context.State1, currentState);
            var times = leaveCurrentState ? Times.Once() : Times.Never();
            Mock.Get(context.DataService).Verify(
                m => m.RunOnLeaveStateScript(context.InitialState, It.IsAny<CancellationToken>()),
                times);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnEnterStateScript(context.State1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ApplyTransition_NoState_Throws()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.ApplyTransition(context.TransitionKey));
        }

        [Fact]
        public async Task ApplyTransition_NoTransition_Throws()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act + Assert
            await sut.SetInitialState();
            await Assert.ThrowsAsync<TransitionNotFoundException>(
                () => sut.ApplyTransition(context.TransitionKey + "1"));
        }

        [Fact]
        public async Task ApplyTransition_StateIsSet()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            // Act
            await sut.SetInitialState();
            await sut.ApplyTransition(context.TransitionKey);
            var currentState = sut.GetCurrentState();

            // Assert
            Assert.Same(context.State1, currentState);

            Mock.Get(context.DataService).Verify(
                m => m.RunOnTransitionScript(context.InitialState, context.Transition, It.IsAny<CancellationToken>()),
                Times.Once);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnLeaveStateScript(context.InitialState, It.IsAny<CancellationToken>()),
                Times.Once);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnEnterStateScript(context.State1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ApplyTransition_NextStateKeyOverride()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new StateMachineManager(
                context.OptionsService,
                context.Logger,
                context.DataService);

            Mock.Get(context.DataService).Setup(
                m => m.RunOnTransitionScript(context.InitialState, context.Transition, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OnTransitionOutput { NextStateKeyOverride = context.State2Key });

            // Act
            await sut.SetInitialState();
            await sut.ApplyTransition(context.TransitionKey);
            var currentState = sut.GetCurrentState();

            // Assert
            Assert.Same(context.State2, currentState);

            Mock.Get(context.DataService).Verify(
                m => m.RunOnTransitionScript(context.InitialState, context.Transition, It.IsAny<CancellationToken>()),
                Times.Once);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnLeaveStateScript(context.InitialState, It.IsAny<CancellationToken>()),
                Times.Once);
            Mock.Get(context.DataService).Verify(
                m => m.RunOnEnterStateScript(context.State2, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
