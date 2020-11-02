using HellEngine.Core.Exceptions;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HellEngine.Core.Services.Scripting
{
    public interface IScriptHost
    {
        HellScript<HellScriptContext> CreateScript(string name, string code);
        HellScript<HellScriptContext> CreateScript(string name, Stream code);
        HellScript<HellScriptContext<TInput>> CreateScript<TInput>(string name, string code)
            where TInput : class;
        HellScript<HellScriptContext<TInput>> CreateScript<TInput>(string name, Stream code)
            where TInput : class;
        HellScript<HellScriptContext<TInput, TOutput>> CreateScript<TInput, TOutput>(
            string name,
            string code)
            where TInput : class
            where TOutput : class, new();
        HellScript<HellScriptContext<TInput, TOutput>> CreateScript<TInput, TOutput>(
            string name,
            Stream code)
            where TInput : class
            where TOutput : class, new();

        Task RunScript(
            HellScript<HellScriptContext> script,
            CancellationToken cancellationToken = default);
        Task RunScript<TInput>(
            HellScript<HellScriptContext<TInput>> script,
            TInput input,
            CancellationToken cancellationToken = default)
            where TInput : class;
        Task<TOutput> RunScript<TInput, TOutput>(
            HellScript<HellScriptContext<TInput, TOutput>> script,
            TInput input,
            CancellationToken cancellationToken = default)
            where TInput : class
            where TOutput : class, new();
    }

    public class ScriptHostOptions
    {
        public static string Path = "ScriptHost";
        public static ScriptHostOptions Default => new ScriptHostOptions();
    }

    [ApplicationService(Service = typeof(IScriptHost))]
    public class ScriptHost : IScriptHost
    {
        private readonly ScriptHostOptions options;
        private readonly ILogger<ScriptHost> logger;
        private readonly ILogger<HellScriptContext> scriptLogger;

        public ScriptHost(
            IOptions<ScriptHostOptions> options,
            ILogger<ScriptHost> logger,
            ILogger<HellScriptContext> scriptLogger)
        {
            this.options = options.Value ?? ScriptHostOptions.Default;
            this.logger = logger;
            this.scriptLogger = scriptLogger;
        }

        public HellScript<HellScriptContext> CreateScript(string name, string code)
        {
            logger.LogDebug($"Creating script {name}");

            return new HellScript<HellScriptContext>(name, CreateScript<HellScriptContext>(code));
        }
        
        public HellScript<HellScriptContext> CreateScript(string name, Stream code)
        {
            logger.LogDebug($"Creating script {name}");

            return new HellScript<HellScriptContext>(name, CreateScript<HellScriptContext>(code));
        }

        public HellScript<HellScriptContext<TInput>> CreateScript<TInput>(string name, string code)
            where TInput : class
        {
            logger.LogDebug($"Creating script {name}");

            return new HellScript<HellScriptContext<TInput>>(
                name,
                CreateScript<HellScriptContext<TInput>>(code));
        }

        public HellScript<HellScriptContext<TInput>> CreateScript<TInput>(string name, Stream code)
            where TInput : class
        {
            logger.LogDebug($"Creating script {name}");

            return new HellScript<HellScriptContext<TInput>>(
                name,
                CreateScript<HellScriptContext<TInput>>(code));
        }

        public HellScript<HellScriptContext<TInput, TOutput>> CreateScript<TInput, TOutput>(
            string name,
            string code)
            where TInput : class
            where TOutput : class, new()
        {
            logger.LogDebug($"Creating script {name}");

            return new HellScript<HellScriptContext<TInput, TOutput>>(
                name,
                CreateScript<HellScriptContext<TInput, TOutput>>(code));
        }

        public HellScript<HellScriptContext<TInput, TOutput>> CreateScript<TInput, TOutput>(
            string name,
            Stream code)
            where TInput : class
            where TOutput : class, new()
        {
            logger.LogDebug($"Creating script {name}");

            return new HellScript<HellScriptContext<TInput, TOutput>>(
                name,
                CreateScript<HellScriptContext<TInput, TOutput>>(code));
        }

        private Script<object> CreateScript<THellScriptContext>(string code)
            where THellScriptContext : HellScriptContext
        {
            return CSharpScript.Create(
                code,
                MakeScriptOptions(),
                typeof(THellScriptContext));
        }

        private Script<object> CreateScript<THellScriptContext>(Stream code)
            where THellScriptContext : HellScriptContext
        {
            return CSharpScript.Create(
                code,
                MakeScriptOptions(),
                typeof(THellScriptContext));
        }
        public static ScriptOptions MakeScriptOptions()
        {
            return ScriptOptions.Default
                .AddReferences(
                    typeof(AssemblyEntryPoint).Assembly,
                    typeof(ILogger).Assembly);
        }

        public async Task RunScript(
            HellScript<HellScriptContext> script,
            CancellationToken cancellationToken = default)
        {
            logger.LogDebug($"Running script {script.Name}");

            var context = new HellScriptContext(script.Name, scriptLogger);
            await RunScript(script, context, cancellationToken);
        }

        public async Task RunScript<TInput>(
            HellScript<HellScriptContext<TInput>> script,
            TInput input,
            CancellationToken cancellationToken = default)
            where TInput : class
        {
            logger.LogDebug($"Running script {script.Name}");

            var context = new HellScriptContext<TInput>(script.Name, scriptLogger, input);
            await RunScript(script, context, cancellationToken);
        }

        public async Task<TOutput> RunScript<TInput, TOutput>(
            HellScript<HellScriptContext<TInput, TOutput>> script,
            TInput input,
            CancellationToken cancellationToken = default)
            where TInput : class
            where TOutput : class, new()
        {
            logger.LogDebug($"Running script {script.Name}");

            var context = new HellScriptContext<TInput, TOutput>(script.Name, scriptLogger, input);
            await RunScript(script, context, cancellationToken);
            return context.Output;
        }

        private async Task RunScript<THellScriptContext>(
            HellScript<THellScriptContext> script,
            THellScriptContext context,
            CancellationToken cancellationToken = default)
            where THellScriptContext : HellScriptContext
        {
            if (script.ContextType != typeof(THellScriptContext))
            {
                throw new HellScriptException("Unexpected script context type");
            }

            var state = await script.Script.RunAsync(context, e => true, cancellationToken);
            if (state.Exception != null)
            {
                throw new HellScriptException("Exception executing HellScript", state.Exception);
            }
        }
    }
}
