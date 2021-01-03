using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace HellEngine.Core.Services.Encoding
{
    [SdkUtilService]
    public interface IStringEncoder
    {
        System.Text.Encoding GetEncoding();

        byte[] EncodeFromString(string str);
        string EncodeToString(byte[] data);

    }

    [ApplicationOptions("Encoding:StringEncoder")]
    public class StringEncoderOptions
    {
        public static StringEncoderOptions Default => new StringEncoderOptions();

        public string EncodingName { get; set; } = Constants.Defaults.EncodingName;
    }

    [ApplicationService(
        Service = typeof(IStringEncoder),
        Lifetime = ApplicationServiceLifetime.Singletone)]
    public class StringEncoder : IStringEncoder
    {
        private readonly StringEncoderOptions options;
        private readonly ILogger<StringEncoder> logger;

        private readonly System.Text.Encoding encoding;

        public StringEncoder(
            IOptions<StringEncoderOptions> options,
            ILogger<StringEncoder> logger)
        {
            this.options = options.Value?? StringEncoderOptions.Default;
            this.logger = logger;

            encoding = GetStringEncoding(this.options.EncodingName);
        }

        public System.Text.Encoding GetEncoding()
        {
            return encoding;
        }

        public byte[] EncodeFromString(string str)
        {
            return encoding.GetBytes(str);
        }

        public string EncodeToString(byte[] data)
        {
            return encoding.GetString(data);
        }

        private System.Text.Encoding GetStringEncoding(string encodingName)
        {
            return encodingName switch
            {
                Constants.EncodingNames.UTF8 => System.Text.Encoding.UTF8,
                _ => throw new NotSupportedException($"Encoding {encodingName} is not supported")
            };
        }
    }
}
