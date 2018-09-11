namespace LowLevelDataSerializer
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class StreamSerializer
    {
        private static readonly int[] Padding = { 0, 7, 6, 5, 4, 3, 2, 1 };

        public static void Serialize(IDataFile inputDataFile, Stream outputStream)
        {
            const long HeaderOffset = 16;

            const long PointerSize = 8;

            var sectionProcessList = new List<Tuple<long, string>>();
            var sectionPositionDict = new Dictionary<string, long>();

            var sectionCount = (long)inputDataFile.Sections.Count;

#if NETCOREAPP2_1
                outputStream.Write(new ReadOnlySpan<byte>(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x42, 0x49, 0x4e, 0x49 })); // 0000BINI
#else
            var buf = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x42, 0x49, 0x4e, 0x49 };
            outputStream.Write(buf, 0, buf.Length);
#endif

            SerializeInt64(outputStream, sectionCount);

            long offset = HeaderOffset + sectionCount * PointerSize * 2;
            outputStream.Position = offset;

            var arr = new long[sectionCount];

            for (var i = 0; i < sectionCount; ++i)
            {
                var sectionName = inputDataFile.SectionNames[i];
                arr[i] = offset;
                offset += SerializeStringBulk(outputStream, sectionName);
            }

            outputStream.Position = HeaderOffset;

            for (var i = 0; i < sectionCount; ++i)
            {
                SerializeInt64(outputStream, arr[i]);
            }

            var sectionPointerOffset = HeaderOffset + sectionCount * PointerSize;

            for (var i = 0; i < sectionCount; ++i)
            {
                sectionProcessList.Add(new Tuple<long, string>(sectionPointerOffset + 8 * i, inputDataFile.SectionNames[i]));
            }

            outputStream.Position = offset;

            for (var i = 0; i < sectionCount; ++i)
            {
                var sectionName = inputDataFile.SectionNames[i];
                sectionPositionDict.Add(sectionName, offset);

                var section = inputDataFile.Sections[sectionName];

                var fieldCount = section.Fields.Count;
                offset += fieldCount * 8;

                for (var j = 0; j < fieldCount; ++j)
                {
                    var field = section.Fields[j];
                    var data = field.Value;

                    if (field.IsRepeated)
                    {
                        var comeBackPosition = outputStream.Position;
                        var beginningOfListPosition = SerializeListField(outputStream, field.Type, data, sectionProcessList, ref offset);
                        outputStream.Position = comeBackPosition;
                        SerializeInt64(outputStream, beginningOfListPosition);
                    }
                    else
                    {
                        SerializeScalarField(outputStream, data, field.Type, sectionProcessList, ref offset);
                    }
                }

                outputStream.Position = offset;
            }

            foreach (var entry in sectionProcessList)
            {
                var offsetToWriteTo = entry.Item1;
                var valueToWrite = sectionPositionDict[entry.Item2];

                outputStream.Position = offsetToWriteTo;
                SerializeInt64(outputStream, valueToWrite);
            }
        }

        internal static void SerializeScalarField(Stream stream, object data, ScalarType fieldType, List<Tuple<long, string>> sectionProcessList, ref long offset)
        {
            switch (fieldType)
            {
                case ScalarType.Double:
                    SerializeDouble(stream, (double)data);
                    break;
                case ScalarType.Int64:
                    SerializeInt64(stream, (long)data);
                    break;
                case ScalarType.UInt64:
                    SerializeUInt64(stream, (ulong)data);
                    break;
                case ScalarType.String:
                    {
                        var comeBackPosition = stream.Position;
                        var beginningOfStringPosition = SerializeString(stream, (string)data, ref offset);
                        stream.Position = comeBackPosition;
                        SerializeInt64(stream, beginningOfStringPosition);
                        break;
                    }
                case ScalarType.Struct:
                    sectionProcessList.Add(new Tuple<long, string>(stream.Position, (string)data));
                    SerializeInt64(stream, 0xDEADBEEF);
                    break;
            }
        }

        internal static void SerializeUInt64(Stream stream, ulong data)
        {
#if NETCOREAPP2_1
            unsafe
            {
                stream.Write(new ReadOnlySpan<byte>(&data, sizeof(ulong)));
            }
#else
            var buf = BitConverter.GetBytes(data);
            stream.Write(buf, 0, buf.Length);
#endif
        }

        internal static void SerializeInt64(Stream stream, long data)
        {
#if NETCOREAPP2_1
            unsafe
            {
                stream.Write(new ReadOnlySpan<byte>(&data, sizeof(long)));
            }
#else
            var buf = BitConverter.GetBytes(data);
            stream.Write(buf, 0, buf.Length);
#endif
        }

        internal static void SerializeDouble(Stream stream, double data)
        {
#if NETCOREAPP2_1
            unsafe
            {
                stream.Write(new ReadOnlySpan<byte>(&data, sizeof(double)));
            }
#else
            var buf = BitConverter.GetBytes(data);
            stream.Write(buf, 0, buf.Length);
#endif
        }

        internal static long SerializeStringBulk(Stream stream, string data)
        {
            const long PointerSize = 8;
            var totalBytes = data.Length * 2;

            SerializeInt64(stream, data.Length);

#if NETCOREAPP2_1
            unsafe
            {
                fixed (char* s = data)
                {
                    stream.Write(new ReadOnlySpan<byte>(s, totalBytes));
                }
            }
#else
            var buf = System.Text.Encoding.Unicode.GetBytes(data);
            stream.Write(buf, 0, buf.Length);
#endif

            var o = Padding[data.Length * 2 % 8];
            stream.Position += o;

            return PointerSize + totalBytes + o;
        }

        internal static long SerializeString(Stream stream, string data, ref long offset)
        {
            long beginningOfStringPosition = offset;
            stream.Position = offset;

            SerializeInt64(stream, data.Length);

#if NETCOREAPP2_1
            unsafe
            {
                fixed (char* s = data)
                {
                    stream.Write(new ReadOnlySpan<byte>(s, data.Length * 2));
                }
            }
#else
            var buf = System.Text.Encoding.Unicode.GetBytes(data);
            stream.Write(buf, 0, buf.Length);
#endif

            offset = stream.Position + Padding[data.Length * 2 % 8];
            return beginningOfStringPosition;
        }

        internal static long SerializeListField(Stream stream, ScalarType fieldType, object data, List<Tuple<long, string>> sectionProcessList, ref long offset)
        {
            var beginningOfListPosition = offset;
            stream.Position = offset;

            switch (fieldType)
            {
                case ScalarType.Int64:
                    {
                        var list = (List<long>)data;
                        SerializeInt64(stream, list.Count);

                        foreach (var t in list)
                        {
                            SerializeInt64(stream, t);
                        }

                        break;
                    }

                case ScalarType.UInt64:
                    {
                        var list = (List<ulong>)data;
                        SerializeInt64(stream, list.Count);

                        foreach (var t in list)
                        {
                            SerializeUInt64(stream, t);
                        }

                        break;
                    }

                case ScalarType.Double:
                    {
                        var list = (List<double>)data;
                        SerializeInt64(stream, list.Count);

                        foreach (var t in list)
                        {
                            SerializeDouble(stream, t);
                        }

                        break;
                    }

                case ScalarType.String:
                    {
                        var list = (List<string>)data;

                        var count = list.Count;

                        SerializeInt64(stream, count);
                        var comeBackPosition = stream.Position;

                        offset = stream.Position + count * 8;
                        stream.Position = offset;

                        var rvaList = new List<long>(count);

                        foreach (var t in list)
                        {
                            rvaList.Add(offset);
                            offset += SerializeStringBulk(stream, t);
                        }

                        stream.Position = comeBackPosition;

                        foreach (var t in rvaList)
                        {
                            SerializeInt64(stream, t);
                        }

                        break;
                    }

                case ScalarType.Struct:
                    {
                        var list = (List<string>)data;
                        SerializeInt64(stream, list.Count);

                        foreach (var t in list)
                        {
                            sectionProcessList.Add(new Tuple<long, string>(stream.Position, t));
                            SerializeInt64(stream, 0xDEADBEEF);
                        }

                        break;
                    }
            }

            return beginningOfListPosition;
        }
    }
}