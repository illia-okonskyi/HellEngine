using HellEngine.Core.Models.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace HellEngine.Core.Exceptions
{
    public class AssetException : Exception
    {
        public AssetException()
            : base()
        { }

        public AssetException(string message)
            : base(message)
        { }

        public AssetException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class AssetDescriptorNotFoundException : AssetException
    {
        public AssetDescriptorNotFoundException(string key)
            : base($"Asset descriptor {key} not found")
        { }
    }

    public class InvalidAssetTypeException : AssetException
    {
        public InvalidAssetTypeException(string key, AssetType expected, AssetType actual)
            : base($"Asset {key} has invalid type. Expected {expected}; Actual {actual}")
        { }
    }

    public class AssetNotFoundException : AssetException
    {
        public AssetNotFoundException(string key, string path, string locale)
            : base($"Asset {key} not found. Path = {path}; Locale = {locale}")
        { }
    }
}
