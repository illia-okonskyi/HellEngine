using HellEngine.Core.Services.Assets;
using HellEngine.Core.Services.Locale;
using System;

namespace HellEngine.Core.Models
{
    public class Session
    {
        public Guid Id { get; set; }

        public ILocaleManager LocaleManager { get; set; }
        public IAssetsManager AssetsManager { get; set; }
    }
}
