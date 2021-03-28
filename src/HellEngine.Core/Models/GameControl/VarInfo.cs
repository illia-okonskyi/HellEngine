using System.Collections.Generic;

namespace HellEngine.Core.Models.GameControl
{
    public enum VarType
    {
        IntVar,
        DoubleVar,
        BoolVar,
        StringVar
    }

    public class VarInfo
    {
        public VarType Type { get; set; }
        public string Key { get; set; }
        public string NameAssetKey { get; set; }
        public object Value { get; set; }
        public IEnumerable<object> Parameters { get; set; }
    }
}
