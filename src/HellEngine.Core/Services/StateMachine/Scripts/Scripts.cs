using HellEngine.Core.Models.StateMachine;
using HellEngine.Core.Services.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace HellEngine.Core.Services.StateMachine.Scripts
{
    public class OnStateEnter : HellScript<HellScriptContext<OnStateEnterInput>>
    {
        public OnStateEnter(string name, Script<object> script)
            : base(name, script)
        { }
    }

    public class OnStateLeave : HellScript<HellScriptContext<OnStateLeaveInput>>
    {
        public OnStateLeave(string name, Script<object> script)
            : base(name, script)
        { }
    }

    public class OnTransition : HellScript<HellScriptContext<OnTransitionInput, OnTransitionOutput>>
    {
        public OnTransition(string name, Script<object> script)
            : base(name, script)
        { }
    }
}
