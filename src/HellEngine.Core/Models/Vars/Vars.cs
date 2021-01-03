namespace HellEngine.Core.Models.Vars
{
    public interface IVar
    {
        string Key { get; }
        string NameAssetKey { get; }

        /// <summary>
        /// Variables are sorted by this index when all queried
        /// </summary>
        uint SetIndex { get; }

        string DisplayString { get; }
    }

    public abstract class AbstractVar<T> : IVar
    {
        public static string Null = "null";

        public string Key { get; }
        public string NameAssetKey { get; }
        public uint SetIndex { get; }
        public T Value { get; }
        public abstract string DisplayString { get; }

        protected AbstractVar(string key, string nameAssetKey, uint setIndex = 0, T value = default)
        {
            Key = key;
            NameAssetKey = nameAssetKey;
            Value = value;
            SetIndex = setIndex;
        }
    }

    public class IntVar : AbstractVar<int?>
    {
        public IntVar(string key, string nameAssetKey, uint setIndex = 0, int? value = default)
            : base(key, nameAssetKey, setIndex, value)
        { }

        public override string DisplayString => Value.HasValue ? Value.ToString() : Null;
    }

    public class DoubleVar : AbstractVar<double?>
    {
        public DoubleVar(string key, string nameAssetKey, uint setIndex = 0, double? value = default)
            : base(key, nameAssetKey, setIndex, value)
        { }

        public override string DisplayString => Value.HasValue ? Value.ToString() : Null;
    }

    public class BoolVar : AbstractVar<bool?>
    {
        public BoolVar(string key, string nameAssetKey, uint setIndex = 0, bool? value = default)
            : base(key, nameAssetKey, setIndex, value)
        { }

        public override string DisplayString => Value.HasValue ? Value.ToString() : Null;
    }

    public class StringVar : AbstractVar<string>
    {
        public StringVar(string key, string nameAssetKey, uint setIndex = 0, string value = default)
            : base(key, nameAssetKey, setIndex, value)
        { }

        public override string DisplayString => Value != null ? Value.ToString() : Null;
    }
}

