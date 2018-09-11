namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;
    using LowLevelDataSerializer;

    internal sealed class DataFile : IDataFile
    {
        private readonly List<string> sectionNames = new List<string>();

        private readonly Dictionary<string, ISection> sections = new Dictionary<string, ISection>();

        public IReadOnlyList<string> SectionNames => this.sectionNames;

        public IReadOnlyDictionary<string, ISection> Sections => this.sections;

        public void AddSection(string sectionName, ISection section)
        {
            this.sections.Add(sectionName, section);
            this.sectionNames.Add(sectionName);
            this.sectionNames.Sort();
        }
    }
}