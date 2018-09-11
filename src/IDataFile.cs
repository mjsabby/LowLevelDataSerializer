namespace LowLevelDataSerializer
{
    using System.Collections.Generic;

    public interface IDataFile
    {
        IReadOnlyList<string> SectionNames { get; }

        IReadOnlyDictionary<string, ISection> Sections { get; }
    }
}