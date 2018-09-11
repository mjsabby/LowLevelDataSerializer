namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;

    internal sealed class StructListField : Field
    {
        public StructListField(string name, List<string> value) : base(name, ScalarType.Struct, true, value)
        {
        }
    }
}