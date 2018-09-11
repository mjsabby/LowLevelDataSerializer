namespace LowLevelDataSerializer.UnitTests
{
    internal sealed class StringField : Field
    {
        public StringField(string name, string value) : base(name, ScalarType.String, false, value)
        {
        }
    }
}