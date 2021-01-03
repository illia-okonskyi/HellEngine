using System;

namespace HellEngine.Utils.Configuration.ServiceRegistrator
{
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    public class SdkUtilServiceAttribute : Attribute
    {}
}
