namespace LowLevelDataSerializer.UnitTests
{
    internal sealed class UInt64Field : Field
    {
        public UInt64Field(string name, ulong value) : base(name, ScalarType.UInt64, false, value)
        {
        }
    }
}