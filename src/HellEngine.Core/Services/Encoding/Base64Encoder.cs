using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace HellEngine.Core.Services.Encoding
{
    [SdkService]
    public interface IBase64Encoder
    {
        string Encode(byte[] data);
        string Encode(string data);
        byte[] Decode(string data);
        string DecodeAsString(string data);
    }

    [ApplicationOptions("Encoding:Base64Encoder")]
    public class Base64EncoderOptions
    {
        public static Base64EncoderOptions Default => new Base64EncoderOptions();
    }

    [ApplicationService(
        Service = typeof(IBase64Encoder),
        Lifetime = ApplicationServiceLifetime.Singletone)]
    public class Base64Encoder : IBase64Encoder
    {
        private readonly Base64EncoderOptions options;
        private readonly ILogger<Base64Encoder> logger;
        private readonly IStringEncoder stringEncoder;

        public Base64Encoder(
            IOptions<Base64EncoderOptions> options,
            ILogger<Base64Encoder> logger,
            IStringEncoder stringEncoder)
        {
            this.options = options.Value?? Base64EncoderOptions.Default;
            this.logger = logger;
            this.stringEncoder = stringEncoder;
        }

        public string Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public string Encode(string data)
        {
            return Encode(stringEncoder.EncodeFromString(data));
        }

        public byte[] Decode(string data)
        {
            return Convert.FromBase64String(data);
        }

        public string DecodeAsString(string data)
        {
            return stringEncoder.EncodeToString(Decode(data));
        }
    }
}
