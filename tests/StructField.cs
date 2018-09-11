namespace LowLevelDataSerializer.UnitTests
{
    internal sealed class StructField : Field
    {
        public StructField(string name, string value) : base(name, ScalarType.Struct, false, value)
        {
        }
    }
}