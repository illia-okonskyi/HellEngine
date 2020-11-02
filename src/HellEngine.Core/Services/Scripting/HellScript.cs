using Microsoft.CodeAnalysis.Scripting;
using System;

namespace HellEngine.Core.Services.Scripting
{
    public class HellScript<THellScriptContext>
        where THellScriptContext : HellScriptContext
    {
        public string Name { get; }
        public Script<object> Script { get; }
        public Type ContextType => typeof(THellScriptContext);

        public HellScript(string name, Script<object> script)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid script name", nameof(name));
            }
            Name = name;
            Script = script ?? throw new ArgumentNullException(nameof(script));
        }
    }
}
