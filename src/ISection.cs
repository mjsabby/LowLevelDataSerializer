namespace LowLevelDataSerializer
{
    using System.Collections.Generic;

    public interface ISection
    {
        IReadOnlyList<IField> Fields { get; }
    }
}