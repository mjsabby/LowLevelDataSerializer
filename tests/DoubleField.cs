namespace LowLevelDataSerializer.UnitTests
{
    internal sealed class DoubleField : Field
    {
        public DoubleField(string name, double value) : base(name, ScalarType.Double, false, value)
        {
        }
    }
}