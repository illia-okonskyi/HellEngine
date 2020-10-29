using System;

namespace HellEngine.Utils.Configuration.ServiceRegistrator
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    public class ApplicationServiceAttribute : Attribute
    {
        public Type Service { get; set; }
        public ApplicationServiceLifetime Lifetime { get; set; } = ApplicationServiceLifetime.Transient;
    }
}
