using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace HellEngine.Core.Services.Scripting
{
    public class HellScriptContext
    {
        public string ScriptName { get; }
        public ILogger<HellScriptContext> Logger { get; }
        public int ManagedThreadId { get; }

        public HellScriptContext(
            string scriptName,
            ILogger<HellScriptContext> logger)
        {
            if (string.IsNullOrEmpty(scriptName))
            {
                throw new ArgumentException("Invalid script name", nameof(scriptName));
            }
            ScriptName = scriptName;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }

    public class HellScriptContext<TInput> : HellScriptContext
        where TInput : class
    {
        public TInput Input { get; }

        public HellScriptContext(
            string scriptName,
            ILogger<HellScriptContext> logger,
            TInput input)
            : base(scriptName, logger)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
        }
    }

    public class HellScriptContext<TInput, TOutput> : HellScriptContext<TInput>
        where TInput : class
        where TOutput : class, new()
    {
        public TOutput Output { get; set; }

        public HellScriptContext(
            string scriptName,
            ILogger<HellScriptContext> logger,
            TInput input)
            : base(scriptName, logger, input)
        { }
    }

}
