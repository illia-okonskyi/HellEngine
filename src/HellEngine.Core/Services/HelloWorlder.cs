using HellEngine.Core.Constants;

namespace HellEngine.Core.Services
{
    public interface IHelloWorlder
    {
        string GetHelloString();
    }
    
    public class HelloWorlder : IHelloWorlder
    {
        public string GetHelloString()
        {
            return HelloWorld.HelloString;
        }
    }
}
