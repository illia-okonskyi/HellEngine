namespace HellEngine.Core.Sdk.Models
{
    public class SumInput
    {
        public int IntVal1 { get; set; }
        public int IntVal2 { get; set; }
    }

    public class ConcatInput
    {
        public string StrVal1 { get; set; }
        public string StrVal2 { get; set; }
    }

    public class CombinedInput
    {
        public SumInput SumInput { get; set; }
        public ConcatInput ConcatInput { get; set; }
    }

    public class CombinedOutput
    {
        public int Sum { get; set; }
        public string Concat { get; set; }
        public string HelloWorld { get; set; }
    }
}
