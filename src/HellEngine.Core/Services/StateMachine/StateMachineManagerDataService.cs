using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.StateMachine;
using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Locale;
using HellEngine.Core.Services.Scripting;
using HellEngine.Core.Services.Sessions;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.StateMachine
{
    public interface IStateMachineManagerDataService
    {
        Task<State> LoadState(string key, CancellationToken cancellationToken);

        Task RunOnEnterStateScript(
            State state,
            CancellationToken cancellationToken);
        Task RunOnLeaveStateScript(
            State state,
            CancellationToken cancellationToken);
        Task<OnTransitionOutput> RunOnTransitionScript(
            State state,
            Transition transition,
            CancellationToken cancellationToken);

    }

    [ApplicationService(Service = typeof(IStateMachineManagerDataService))]
    public class StateMachineManagerDataService : IStateMachineManagerDataService
    {
        private readonly ISessionIdProvider sessionIdProvider;
        private readonly ILocaleManager localeManager;
        private readonly IAssetsManager assetsManager;
        private readonly IScriptHost scriptHost;

        public StateMachineManagerDataService(
            ISessionIdProvider sessionIdProvider,
            ILocaleManager localeManager,
            IAssetsManager assetsManager,
            IScriptHost scriptHost)
        {
            this.sessionIdProvider = sessionIdProvider;
            this.localeManager = localeManager;
            this.assetsManager = assetsManager;
            this.scriptHost = scriptHost;
        }

        public async Task<State> LoadState(string key, CancellationToken cancellationToken)
        {
            try
            {
                var asset = await assetsManager.GetStateAsset(
                    key,
                    localeManager.GetLocale(),
                    cancellationToken);
                var state = JsonConvert.DeserializeObject<State>(asset.Data);
                return state;
            }
            catch (AssetException ex)
            {
                throw new StateNotFoundException(key, ex);
            }
            catch (JsonReaderException ex)
            {
                throw new BadStateException(key, ex);
            }
        }

        public async Task RunOnEnterStateScript(
            State state,
            CancellationToken cancellationToken)
        {
            var key = state.OnEnterScriptAssetKey;
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var scriptAsset = await assetsManager.GetScriptAsset(
                key,
                localeManager.GetLocale(),
                cancellationToken);
            var script = scriptHost.CreateScript<OnStateEnterInput>(
                key,
                scriptAsset.Data);
            var input = new OnStateEnterInput { State = state };
            await scriptHost.RunScript(script,
                sessionIdProvider.GetSessionId(),
                input,
                cancellationToken);
        }

        public async Task RunOnLeaveStateScript(
            State state,
            CancellationToken cancellationToken)
        {
            var key = state.OnLeaveScriptAssetKey;
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var scriptAsset = await assetsManager.GetScriptAsset(
                key,
                localeManager.GetLocale(),
                cancellationToken);
            var script = scriptHost.CreateScript<OnStateLeaveInput>(
                key,
                scriptAsset.Data);
            var input = new OnStateLeaveInput { State = state };
            await scriptHost.RunScript(script,
                sessionIdProvider.GetSessionId(),
                input,
                cancellationToken);
        }

        public async Task<OnTransitionOutput> RunOnTransitionScript(
            State state,
            Transition transition,
            CancellationToken cancellationToken)
        {
            var key = state.OnTransitionScriptAssetKey;
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var scriptAsset = await assetsManager.GetScriptAsset(
                key,
                localeManager.GetLocale(),
                cancellationToken);
            var script = scriptHost.CreateScript<OnTransitionInput, OnTransitionOutput>(
                key,
                scriptAsset.Data);
            var input = new OnTransitionInput { State = state, Transition = transition };
            var output = await scriptHost.RunScript(script,
                sessionIdProvider.GetSessionId(),
                input,
                cancellationToken);
            return output;
        }
    }
}
