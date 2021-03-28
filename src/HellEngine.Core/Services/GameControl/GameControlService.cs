using HellEngine.Core.Models.GameControl;
using HellEngine.Core.Models.StateMachine;
using HellEngine.Core.Models.Vars;
using HellEngine.Core.Services.Encoding;
using HellEngine.Core.Services.Sessions;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.GameControl
{
    public interface IGameControlService
    {
        State GetCurrentGameState(Guid sessionId);
        Task StartGame(Guid sessionId, string userName, CancellationToken cancellationToken = default);
        Task ExitGame(Guid sessionId, CancellationToken cancellationToken = default);
        Task Transition(Guid sessionId, string key, CancellationToken cancellationToken = default);
        string SaveGame(Guid sessionId);
        Task LoadGame(Guid sessionId, string fileData, CancellationToken cancellationToken = default);
    }

    [ApplicationService(Service = typeof(IGameControlService))]
    public class GameControlService : IGameControlService
    {
        private readonly ILogger<GameControlService> logger;
        private readonly ISessionManager sessionManager;
        private readonly IBase64Encoder base64Encoder;
        
        private readonly Regex userNameRegex = new Regex("^[a-zA-Z0-9\\.\\-_]+$");

        public GameControlService(
            ILogger<GameControlService> logger,
            ISessionManager sessionManager,
            IBase64Encoder base64Encoder)
        {
            this.logger = logger;
            this.sessionManager = sessionManager;
            this.base64Encoder = base64Encoder;
        }

        public State GetCurrentGameState(Guid sessionId)
        {
            var session = sessionManager.GetSession(sessionId);
            return session.StateMachineManager.GetCurrentState();
        }

        public async Task StartGame(Guid sessionId, string userName, CancellationToken cancellationToken = default)
        {
            if (!userNameRegex.Match(userName).Success)
            {
                throw new ArgumentException("Bad username", nameof(userName));
            }

            var session = sessionManager.GetSession(sessionId);
            session.VarsManager.Init(userName);
            await session.StateMachineManager.SetInitialState(cancellationToken);
        }

        public async Task ExitGame(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var session = sessionManager.GetSession(sessionId);
            await session.StateMachineManager.SetFinalState(cancellationToken);
        }

        public async Task Transition(Guid sessionId, string key, CancellationToken cancellationToken = default)
        {
            var session = sessionManager.GetSession(sessionId);
            await session.StateMachineManager.ApplyTransition(key, cancellationToken);
        }

        public string SaveGame(Guid sessionId)
        {
            var session = sessionManager.GetSession(sessionId);
            
            var userName = session.VarsManager.GetUserName();
            var currentStateKey = session.StateMachineManager.GetCurrentState().Key;
            var varsInfo = new List<VarInfo>();
            var vars = session.VarsManager.GetAllVars(false);

            foreach (var avar in vars)
            {
                varsInfo.Add(MakeVarInfo(avar));
            }

            var saveGame = new SaveGame
            {
                UserName = userName,
                CurrentStateKey = currentStateKey,
                VarsInfo = varsInfo
            };
            
            var json = JsonConvert.SerializeObject(saveGame, Formatting.Indented);
            return base64Encoder.Encode(json);
        }

        private VarInfo MakeVarInfo(IVar avar)
        {
            if (avar is IntVar intVar)
            {
                return new VarInfo
                {
                    Type = VarType.IntVar,
                    Key = intVar.Key,
                    NameAssetKey = intVar.NameAssetKey,
                    Value = intVar.Value,
                    Parameters = new object[] { intVar.MinValue, intVar.MaxValue }
                };
            }

            if (avar is DoubleVar doubleVar)
            {
                return new VarInfo
                {
                    Type = VarType.DoubleVar,
                    Key = doubleVar.Key,
                    NameAssetKey = doubleVar.NameAssetKey,
                    Value = doubleVar.Value,
                    Parameters = new object[] { doubleVar.MinValue, doubleVar.MaxValue }
                };
            }

            if (avar is BoolVar boolVar)
            {
                return new VarInfo
                {
                    Type = VarType.BoolVar,
                    Key = boolVar.Key,
                    NameAssetKey = boolVar.NameAssetKey,
                    Value = boolVar.Value
                };
            }

            if (avar is StringVar stringVar)
            {
                return new VarInfo
                {
                    Type = VarType.StringVar,
                    Key = stringVar.Key,
                    NameAssetKey = stringVar.NameAssetKey,
                    Value = stringVar.Value,
                    Parameters = new object[] { stringVar.MaxLength }
                };
            }

            throw new ArgumentException("Unsupported var type", nameof(avar));
        }

        public async Task LoadGame(Guid sessionId, string fileData, CancellationToken cancellationToken = default)
        {
            var json = base64Encoder.DecodeAsString(fileData);
            var saveGame = JsonConvert.DeserializeObject<SaveGame>(json);

            var userName = saveGame.UserName;
            var currentStateKey = saveGame.CurrentStateKey;
            var vars = new List<IVar>();
            foreach (var varInfo in saveGame.VarsInfo)
            {
                vars.Add(MakeVar(varInfo));
            }

            var session = sessionManager.GetSession(sessionId);
            session.VarsManager.Init(userName, vars);
            await session.StateMachineManager.SetCurrentState(currentStateKey, false, cancellationToken);
        }

        private IVar MakeVar(VarInfo varInfo)
        {
            var parameters = varInfo.Parameters.ToList();
            switch (varInfo.Type)
            {
                case VarType.IntVar:
                    return new IntVar(
                        varInfo.Key,
                        varInfo.NameAssetKey,
                        (int?)(long?)varInfo.Value,
                        (int?)(long?)parameters[0],
                        (int?)(long?)parameters[1]);
                case VarType.DoubleVar:
                    return new DoubleVar(
                        varInfo.Key,
                        varInfo.NameAssetKey,
                        (double?)varInfo.Value,
                        (double?)parameters[0],
                        (double?)parameters[1]);
                case VarType.BoolVar:
                    return new BoolVar(
                        varInfo.Key,
                        varInfo.NameAssetKey,
                        (bool?)varInfo.Value);
                case VarType.StringVar:
                    return new StringVar(
                        varInfo.Key,
                        varInfo.NameAssetKey,
                        (string)varInfo.Value,
                        (int?)(long?)parameters[0]);
                default:
                    throw new ArgumentException("Unsupported var type", nameof(varInfo));
            }
        }
    }
}
