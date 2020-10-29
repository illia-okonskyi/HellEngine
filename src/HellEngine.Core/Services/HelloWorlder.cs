using HellEngine.Core.Constants;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HellEngine.Core.Services
{
    public interface IHelloWorlder
    {
        string GetHelloString();
    }

    public class HelloWorlderOptions
    {
        public static string Path = "HelloWorlder";
        
        public string HelloString { get; set; }
    }

    [ApplicationService(Service = typeof(IHelloWorlder))]
    public class HelloWorlder : IHelloWorlder
    {
        private readonly string helloString;
        private readonly ILogger<HelloWorlder> logger;
        
        public HelloWorlder(
            IOptions<HelloWorlderOptions> options,
            ILogger<HelloWorlder> logger)
        {
            this.helloString = options.Value?.HelloString;
            this.logger = logger;
        }

        public string GetHelloString()
        {
            logger.LogInformation("GetHello request");
            return !string.IsNullOrEmpty(helloString)
                ? helloString
                : HelloWorld.HelloString;
        }
    }
}
