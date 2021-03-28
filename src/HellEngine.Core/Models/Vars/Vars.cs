using System;

namespace HellEngine.Core.Models.Vars
{
    public interface IVar
    {
        string Key { get; }
        string NameAssetKey { get; }
        string DisplayString { get; }
    }

    public abstract class AbstractVar<T> : IVar
    {
        public static string Null = "null";

        protected T varValue = default;

        public string Key { get; }
        public string NameAssetKey { get; }
        public abstract T Value { get; set; }
        public abstract string DisplayString { get; }

        protected AbstractVar(string key, string nameAssetKey)
        {
            Key = key;
            NameAssetKey = nameAssetKey;
        }
    }

    public sealed class IntVar : AbstractVar<int?>
    {
        public int? MinValue { get; }
        public int? MaxValue { get; }

        public override int? Value
        { 
            get
            {
                return varValue;
            }
            set
            {
                if (!value.HasValue)
                {
                    varValue = value;
                    return;
                }

                var correctedValue = value.Value;
                if (MinValue.HasValue)
                {
                    correctedValue = Math.Max(MinValue.Value, correctedValue);
                }

                if (MaxValue.HasValue)
                {
                    correctedValue = Math.Min(MaxValue.Value, correctedValue);
                }

                varValue = correctedValue;
            }
        }

        public IntVar(
            string key,
            string nameAssetKey,
            int? value = default,
            int? minValue = default,
            int? maxValue = default)
            : base(key, nameAssetKey)
        {
            if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
            {
                throw new ArgumentException("minValue > maxValue");
            }
            
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }

        public override string DisplayString => Value.HasValue ? Value.ToString() : Null;
    }

    public sealed class DoubleVar : AbstractVar<double?>
    {
        public double? MinValue { get; }
        public double? MaxValue { get; }

        public override double? Value
        {
            get
            {
                return varValue;
            }
            set
            {
                if (!value.HasValue)
                {
                    varValue = value;
                    return;
                }

                var correctedValue = value.Value;
                if (MinValue.HasValue)
                {
                    correctedValue = Math.Max(MinValue.Value, correctedValue);
                }

                if (MaxValue.HasValue)
                {
                    correctedValue = Math.Min(MaxValue.Value, correctedValue);
                }

                varValue = correctedValue;
            }
        }

        public DoubleVar(
            string key,
            string nameAssetKey,
            double? value = default,
            double? minValue = default,
            double? maxValue = default)
            : base(key, nameAssetKey)
        {
            if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue)
            {
                throw new ArgumentException("minValue > maxValue");
            }

            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }

        public override string DisplayString => Value.HasValue ? Value.ToString() : Null;
    }

    public sealed class BoolVar : AbstractVar<bool?>
    {
        public override bool? Value { get => varValue; set => varValue = value; }

        public BoolVar(string key, string nameAssetKey, bool? value = default)
            : base(key, nameAssetKey)
        { }

        public override string DisplayString => Value.HasValue ? Value.ToString() : Null;
    }

    public sealed class StringVar : AbstractVar<string>
    {
        public int? MaxLength { get; set; }

        public override string Value
        {
            get => varValue;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    varValue = value;
                    return;
                }

                string correctedValue = value;
                if (MaxLength.HasValue)
                {
                    correctedValue = value.Substring(0, MaxLength.Value);
                }
                varValue = correctedValue;
            }
        }

        public StringVar(
            string key,
            string nameAssetKey,
            string value = default,
            int? maxLength = default)
            : base(key, nameAssetKey)
        {
            MaxLength = maxLength;
            Value = value;
        }

        public override string DisplayString => Value != null ? Value.ToString() : Null;
    }
}

