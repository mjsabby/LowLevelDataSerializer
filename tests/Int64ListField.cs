namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;

    internal sealed class Int64ListField : Field
    {
        public Int64ListField(string name, List<long> value) : base(name, ScalarType.Int64, true, value)
        {
        }
    }
}