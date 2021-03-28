using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.StateMachine;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.StateMachine
{
    public interface IStateMachineManager
    {
        Task SetCurrentState(
            string key,
            bool leaveCurrentState,
            CancellationToken cancellationToken = default);
        Task SetInitialState(CancellationToken cancellationToken = default);
        Task SetFinalState(CancellationToken cancellationToken = default);
        Task ApplyTransition(string key, CancellationToken cancellationToken = default);

        State GetCurrentState();
    }

    [ApplicationService(
        Service = typeof(IStateMachineManager),
        Lifetime = ApplicationServiceLifetime.Scoped)]
    public class StateMachineManager : IStateMachineManager
    {
        private readonly StateMachineOptions options;

        private readonly ILogger<StateMachineManager> logger;
        private readonly IStateMachineManagerDataService dataService;

        private State currentState;

        public StateMachineManager(
            IOptions<StateMachineOptions> options,
            ILogger<StateMachineManager> logger,
            IStateMachineManagerDataService dataService)
        {
            this.options = options.Value ?? StateMachineOptions.Default;
            this.logger = logger;
            this.dataService = dataService;
        }

        public async Task SetCurrentState(
            string key,
            bool leaveCurrentState,
            CancellationToken cancellationToken = default)
        {
            var state = await dataService.LoadState(key, cancellationToken);
            await SetCurrentStateInternal(state, leaveCurrentState, cancellationToken);
        }

        public async Task SetInitialState(
            CancellationToken cancellationToken = default)
        {
            await SetCurrentState(options.InitialStateKey, false, cancellationToken);
        }

        public async Task SetFinalState(CancellationToken cancellationToken = default)
        {
            await SetCurrentState(options.FinalStateKey, true, cancellationToken);
        }

        public async Task ApplyTransition(
            string key,
            CancellationToken cancellationToken = default)
        {
            if (currentState == null)
            {
                throw new InvalidOperationException("current state is null");
            }

            var transition = currentState.Transitions.SingleOrDefault(t => t.Key == key);
            if (transition == null)
            {
                throw new TransitionNotFoundException(key);
            }

            var output = await dataService.RunOnTransitionScript(
                currentState,
                transition,
                cancellationToken);
            var nextStateKey = output?.NextStateKeyOverride ?? transition.NextStateKey;
            await SetCurrentState(nextStateKey, true, cancellationToken);
        }

        public State GetCurrentState()
        {
            return currentState;
        }

        private async Task SetCurrentStateInternal(
            State state,
            bool leaveCurrentState,
            CancellationToken cancellationToken)
        {
            if (leaveCurrentState && currentState != null)
            {
                await dataService.RunOnLeaveStateScript(
                    currentState,
                    cancellationToken);
            }
            currentState = state;
            if (currentState != null)
            {
                await dataService.RunOnEnterStateScript(
                    currentState,
                    cancellationToken);
            }
        }
    }
}
