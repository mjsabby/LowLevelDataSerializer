namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;

    internal sealed class UInt64ListField : Field
    {
        public UInt64ListField(string name, List<ulong> value) : base(name, ScalarType.UInt64, true, value)
        {
        }
    }
}