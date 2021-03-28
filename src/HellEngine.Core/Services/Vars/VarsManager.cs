using HellEngine.Core.Constants;
using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Vars;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace HellEngine.Core.Services.Vars
{
    public interface IVarsManager
    {
        void Init(string userName, List<IVar> vars = null);
        string GetUserName();
        void AddVar(IVar avar);
        void RemoveVar(string key);
        void ClearVars();
        bool ContainsVar(string key);
        IVar GetVar(string key);
        TVar GetVar<TVar>(string key) where TVar : IVar;

        IEnumerable<IVar> GetAllVars(bool includeUserName = true);
    }

    [ApplicationOptions("VarsManager")]
    public class VarsManagerOptions
    {
        public static VarsManagerOptions Default => new VarsManagerOptions();

        public string UserNameVarKey { get; set; } = Defaults.VarsManagerUserNameVarKey;
        public string UserNameVarNameAssetKey { get; set; } = Defaults.VarsManagerUserNameVarNameAssetKey;
    }

    [ApplicationService(
        Service = typeof(IVarsManager),
        Lifetime = ApplicationServiceLifetime.Scoped)]
    public class VarsManager : IVarsManager
    {
        private readonly VarsManagerOptions options;
        private readonly ILogger<VarsManager> logger;

        private readonly List<IVar> vars = new List<IVar>();
        private readonly Dictionary<string, int> index = new Dictionary<string, int>();

        public VarsManager(
            IOptions<VarsManagerOptions> options,
            ILogger<VarsManager> logger)
        {
            this.options = options.Value ?? VarsManagerOptions.Default;
            this.logger = logger;
        }

        public void Init(string userName, List<IVar> vars = null)
        {
            this.vars.Clear();
            this.vars.Add(new StringVar(
                options.UserNameVarKey,
                options.UserNameVarNameAssetKey,
                userName));
            if (vars != null)
            {
                this.vars.AddRange(vars);
            }
            RebuildIndex();
        }

        public string GetUserName()
        {
            var avar = GetVar<StringVar>(options.UserNameVarKey);
            return avar.Value;
        }

        public void AddVar(IVar avar)
        {
            if (ContainsVar(avar.Key))
            {
                throw new VarAlreadyExistsException(avar.Key);
            }

            vars.Add(avar);
            RebuildIndex();
        }

        public void RemoveVar(string key)
        {
            if (!ContainsVar(key))
            {
                throw new VarNotFoundException(key);
            }

            vars.Remove(vars.Find(v => v.Key == key));
            RebuildIndex();
        }

        public void ClearVars()
        {
            vars.Clear();
            RebuildIndex();
        }

        public bool ContainsVar(string key)
        {
            return index.ContainsKey(key);
        }

        public IVar GetVar(string key)
        {
            if (!ContainsVar(key))
            {
                throw new VarNotFoundException(key);
            }

            return vars[index[key]];
        }

        public TVar GetVar<TVar>(string key) where TVar : IVar
        {
            return (TVar)GetVar(key);
        }

        public IEnumerable<IVar> GetAllVars(bool includeUserName = true)
        {
            IEnumerable<IVar> result = vars;
            if (!includeUserName)
            {
                result = result.Where(v => v.Key != options.UserNameVarKey);
            }

            return result;
        }

        private void RebuildIndex()
        {
            index.Clear();
            for (int i = 0; i < vars.Count; ++i)
            {
                index[vars[i].Key] = i;
            }
        }
    }
}
