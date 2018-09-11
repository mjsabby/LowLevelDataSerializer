namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;

    public sealed class Section : ISection
    {
        private readonly List<IField> fields = new List<IField>();

        public IReadOnlyList<IField> Fields => this.fields;

        public void AddField(IField field)
        {
            this.fields.Add(field);
        }
    }
}