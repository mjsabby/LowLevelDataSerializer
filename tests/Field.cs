namespace LowLevelDataSerializer.UnitTests
{
    using LowLevelDataSerializer;

    internal abstract class Field : IField
    {
        protected Field(string name, ScalarType type, bool isRepeated, object value)
        {
            this.Name = name;
            this.Type = type;
            this.IsRepeated = isRepeated;
            this.Value = value;
        }

        public string Name { get; }

        public ScalarType Type { get; }

        public bool IsRepeated { get; }

        public object Value { get; }
    }
}