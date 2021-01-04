using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Vars;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HellEngine.Core.Services.Vars
{
    public interface IVarsManager
    {
        void AddVar(IVar avar);
        void RemoveVar(string key);
        void ClearVars();
        bool ContainsVar(string key);
        IVar GetVar(string key);
        TVar GetVar<TVar>(string key) where TVar : IVar;

        IEnumerable<IVar> GetAllVars();
    }


    [ApplicationService(
        Service = typeof(IVarsManager),
        Lifetime = ApplicationServiceLifetime.Scoped)]
    public class VarsManager : IVarsManager
    {
        private readonly ILogger<VarsManager> logger;

        private readonly List<IVar> vars = new List<IVar>();
        private readonly Dictionary<string, int> index = new Dictionary<string, int>();

        public VarsManager(ILogger<VarsManager> logger)
        {
            this.logger = logger;
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

        public IEnumerable<IVar> GetAllVars()
        {
            return vars;
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
