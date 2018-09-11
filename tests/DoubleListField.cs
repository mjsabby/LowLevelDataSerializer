namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;

    internal sealed class DoubleListField : Field
    {
        public DoubleListField(string name, List<double> value) : base(name, ScalarType.Double, true, value)
        {
        }
    }
}