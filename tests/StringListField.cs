namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;

    internal sealed class StringListField : Field
    {
        public StringListField(string name, List<string> value) : base(name, ScalarType.String, true, value)
        {
        }
    }
}