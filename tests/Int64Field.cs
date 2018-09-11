namespace LowLevelDataSerializer.UnitTests
{
    internal sealed class Int64Field : Field
    {
        public Int64Field(string name, long value) : base(name, ScalarType.Int64, false, value)
        {
        }
    }
}