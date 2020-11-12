using HellEngine.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HellEngine.Core.Sdk.StaticServices
{
    public static class DummyService
    {
        public static int Sum(int a, int b)
        {
            return a + b;
        }

        public static string Concat(string a, string b)
        {
            return a + b;
        }
    }
}
