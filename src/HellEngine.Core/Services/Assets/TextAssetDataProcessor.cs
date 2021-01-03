using HellEngine.Core.Exceptions;
using HellEngine.Core.Services.Encoding;
using HellEngine.Core.Services.Vars;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace HellEngine.Core.Services.Assets
{
    public interface ITextAssetDataProcessor
    {
        byte[] ProcessData(byte[] data);
    }

    [ApplicationService(Service = typeof(ITextAssetDataProcessor))]
    public class TextAssetDataProcessor : ITextAssetDataProcessor
    {
        private readonly AssetsOptions options;

        private readonly IStringEncoder stringEncoder;
        private readonly IVarsManager varsManager;

        private readonly Regex varRegex = new Regex("{var=([a-zA-Z0-9\\.\\-_]+)}", RegexOptions.Compiled);

        public TextAssetDataProcessor(
            IOptions<AssetsOptions> options,
            IStringEncoder stringEncoder,
            IVarsManager varsManager)
        {
            this.options = options.Value ?? AssetsOptions.Default;
            this.stringEncoder = stringEncoder;
            this.varsManager = varsManager;
        }

        public byte[] ProcessData(byte[] data)
        {
            var stringData = stringEncoder.EncodeToString(data);
            var sb = new StringBuilder();

            var matches = varRegex.Matches(stringData);
            var startIndex = 0;
            for (var i = 0; i < matches.Count; ++i)
            {
                var match = matches[i];
                var varKey = match.Groups[1].Value;
                var varValue = GetVarValue(varKey);
                sb.Append(stringData.Substring(startIndex, match.Index - startIndex));
                sb.Append($"<span class=\"{options.VarValueSpanClass}\">{varValue}</span>");
                startIndex = match.Index + match.Value.Length;
            }
            sb.Append(stringData.Substring(startIndex));
            var replacedString = sb.ToString();

            return stringEncoder.EncodeFromString(replacedString); 
        }

        private string GetVarValue(string varKey)
        {
            try
            {
                return varsManager.GetVar(varKey).DisplayString;
            }
            catch (VarNotFoundException)
            {
                return $"VAR NOT FOUND {varKey}";
            }
        }
    }
}
