namespace HellEngine.Core.Models.Assets
{
    public class Asset
    {
        public AssetDescriptor Descriptor { get; }
        public string Locale { get; }
        public bool IsDefaultLocale => Locale == Constants.Defaults.Locale;

        public AssetDataEncoding DataEncoding { get; private set; }
        public string Data { get; private set; }

        public Asset(AssetDescriptor descriptor, string locale = null)
        {
            Descriptor = descriptor;
            Locale = !string.IsNullOrEmpty(locale)
                ? locale
                : Constants.Defaults.Locale;
        }

        public void SetData(AssetDataEncoding dataEncoding, string data)
        {
            DataEncoding = dataEncoding;
            Data = data;
        }
    }
}
