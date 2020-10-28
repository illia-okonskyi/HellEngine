using HellEngine.Core.Constants;
using Microsoft.Extensions.Logging;

namespace HellEngine.Core.Services
{
    public interface IHelloWorlder
    {
        string GetHelloString();
    }
    
    public class HelloWorlder : IHelloWorlder
    {
        private readonly ILogger<HelloWorlder> logger;
        public HelloWorlder(ILogger<HelloWorlder> logger)
        {
            this.logger = logger;
        }

        public string GetHelloString()
        {
            logger.LogInformation("GetHello request");
            return HelloWorld.HelloString;
        }
    }
}
