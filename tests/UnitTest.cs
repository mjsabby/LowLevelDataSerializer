namespace LowLevelDataSerializer.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static StreamSerializer;

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var fooSection = new Section();
            fooSection.AddField(new Int64Field("FieldA", 25));
            fooSection.AddField(new DoubleField("FieldB", 50.0));
            fooSection.AddField(new DoubleListField("FieldC", new List<double> { 50.0, 60.0 }));
            fooSection.AddField(new StringListField("FieldD", new List<string> { "PreList1", "PreList2" }));
            fooSection.AddField(new StringField("FieldD", "FooBar"));
            fooSection.AddField(new StringListField("FieldD", new List<string> { "PostList1", "PostList2" }));

            var barSection = new Section();

            barSection.AddField(new Int64Field("FieldA", 100));
            barSection.AddField(new StructField("FieldB", "Foo"));
            barSection.AddField(new UInt64ListField("FieldC", new List<ulong> { 1000 }));
            barSection.AddField(new StructListField("FieldD", new List<string> { "Foo", "Baz" }));

            var bazSection = new Section();

            bazSection.AddField(new Int64Field("FieldA", 100));
            bazSection.AddField(new Int64ListField("FieldB", new List<long> { 1000, 2000 }));
            bazSection.AddField(new StringListField("FieldC", new List<string> { "Str1", "Str2" }));
            bazSection.AddField(new StringListField("FieldD", new List<string> { "Str3", "Str4" }));

            var inputDataFile = new DataFile();
            inputDataFile.AddSection("Foo", fooSection);
            inputDataFile.AddSection("Bar", barSection);
            inputDataFile.AddSection("Baz", bazSection);

            using (var outputFileStream = new FileStream(@"foo.bin", FileMode.Create, FileAccess.ReadWrite))
            {
                Serialize(inputDataFile, outputFileStream);
            }
        }
    }
}