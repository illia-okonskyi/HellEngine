using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HellEngine.Core.Services.Locale
{
    public interface ILocaleManager
    {
        string GetLocale();
        void SetLocale(string locale);
    }

    [ApplicationOptions("LocaleManager")]
    public class LocaleManagerOptions
    {
        public static LocaleManagerOptions Default => new LocaleManagerOptions();

        public string StartingLocale { get; set; } = Constants.Defaults.Locale;
    }

    [ApplicationService(
        Service = typeof(ILocaleManager),
        Lifetime = ApplicationServiceLifetime.Scoped)]
    public class LocaleManager : ILocaleManager
    {
        private readonly LocaleManagerOptions options;
        private readonly ILogger<LocaleManager> logger;

        private string locale;

        public LocaleManager(
            IOptions<LocaleManagerOptions> options,
            ILogger<LocaleManager> logger)
        {
            this.options = options.Value ?? LocaleManagerOptions.Default;
            this.logger = logger;

            locale = this.options.StartingLocale;
        }

        public string GetLocale()
        {
            return locale;
        }

        public void SetLocale(string locale)
        {
            this.locale = locale;
        }
    }
}
