namespace LowLevelDataSerializer
{
    public interface IField
    {
        ScalarType Type { get; }

        bool IsRepeated { get; }

        object Value { get; }
    }
}