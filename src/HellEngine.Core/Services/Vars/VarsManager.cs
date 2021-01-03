using HellEngine.Core.Exceptions;
using HellEngine.Core.Models.Vars;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace HellEngine.Core.Services.Vars
{
    public interface IVarsManager
    {
        void AddVar(IVar avar);
        void RemoveVar(string key);
        void ClearVars();
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

        private readonly Dictionary<string, IVar> vars = new Dictionary<string, IVar>();

        public VarsManager(ILogger<VarsManager> logger)
        {
            this.logger = logger;
        }

        public void AddVar(IVar avar)
        {
            if (vars.ContainsKey(avar.Key))
            {
                throw new VarAlreadyExistsException(avar.Key);
            }

            vars[avar.Key] = avar;
        }

        public void RemoveVar(string key)
        {
            if (!vars.ContainsKey(key))
            {
                throw new VarNotFoundException(key);
            }

            vars.Remove(key);
        }

        public void ClearVars()
        {
            vars.Clear();
        }

        public IVar GetVar(string key)
        {
            if (!vars.ContainsKey(key))
            {
                throw new VarNotFoundException(key);
            }

            return vars[key];
        }

        public TVar GetVar<TVar>(string key) where TVar : IVar
        {
            return (TVar)GetVar(key);
        }

        public IEnumerable<IVar> GetAllVars()
        {
            return vars.Values.OrderBy(v => v.SetIndex);
        }
    }
}
