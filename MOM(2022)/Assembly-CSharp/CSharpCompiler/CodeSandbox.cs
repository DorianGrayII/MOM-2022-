namespace CSharpCompiler
{
    using DBDef;
    using MHUtils;
    using Mono.CSharp;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class CodeSandbox
    {
        private MethodInfo importTypes;
        private ReflectionImporter ri;
        private ModuleContainer mc;

        public CodeSandbox(CompilerContext ctx)
        {
            ctx.Settings.AssemblyReferences.Clear();
            ctx.Settings.LoadDefaultReferences = false;
            ctx.Settings.StdLib = false;
        }

        private static System.Type[] GetNamespaceEnumsTypes(string nameSpace)
        {
            return Array.FindAll<System.Type>(Assembly.GetExecutingAssembly().GetTypes(), o => o.IsEnum && (o.Namespace == nameSpace));
        }

        private static System.Type[] GetSubTypes(System.Type t)
        {
            return Array.FindAll<System.Type>(t.Assembly.GetTypes(), o => o.IsSubclassOf(t));
        }

        private int Import(System.Type[] type)
        {
            object[] parameters = new object[] { type, this.mc.GlobalRootNamespace, false };
            this.importTypes.Invoke(this.ri, parameters);
            return type.Length;
        }

        public void RegisterTypes(ReflectionImporter importer, ModuleContainer module)
        {
            System.Type[] types = new System.Type[] { typeof(System.Type[]), typeof(Namespace), typeof(bool) };
            this.importTypes = importer.GetType().GetMethod("ImportTypes", BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Any, types, null);
            this.ri = importer;
            this.mc = module;
            Debug.Log("Allowed Access to " + (((0 + this.Import(BuiltInTypes)) + this.Import(AdditionalTypes)) + this.Import(UnityEngineTypes)).ToString() + " external types");
            Debug.Log("Allowed Access to " + (((0 + this.Import(GameTypes)) + this.Import(GetSubTypes(typeof(DBClass)))) + this.Import(GetNamespaceEnumsTypes("DBDef"))).ToString() + " game types");
        }

        private static System.Type[] BuiltInTypes
        {
            get
            {
                System.Type[] typeArray1 = new System.Type[0x21];
                typeArray1[0] = typeof(object);
                typeArray1[1] = typeof(ValueType);
                typeArray1[2] = typeof(System.Attribute);
                typeArray1[3] = typeof(int);
                typeArray1[4] = typeof(uint);
                typeArray1[5] = typeof(long);
                typeArray1[6] = typeof(ulong);
                typeArray1[7] = typeof(float);
                typeArray1[8] = typeof(double);
                typeArray1[9] = typeof(char);
                typeArray1[10] = typeof(short);
                typeArray1[11] = typeof(decimal);
                typeArray1[12] = typeof(bool);
                typeArray1[13] = typeof(sbyte);
                typeArray1[14] = typeof(byte);
                typeArray1[15] = typeof(ushort);
                typeArray1[0x10] = typeof(string);
                typeArray1[0x11] = typeof(System.Enum);
                typeArray1[0x12] = typeof(System.Delegate);
                typeArray1[0x13] = typeof(MulticastDelegate);
                typeArray1[20] = typeof(void);
                typeArray1[0x15] = typeof(Array);
                typeArray1[0x16] = typeof(System.Type);
                typeArray1[0x17] = typeof(IEnumerator);
                typeArray1[0x18] = typeof(IEnumerable);
                typeArray1[0x19] = typeof(IDisposable);
                typeArray1[0x1a] = typeof(IntPtr);
                typeArray1[0x1b] = typeof(UIntPtr);
                typeArray1[0x1c] = typeof(RuntimeFieldHandle);
                typeArray1[0x1d] = typeof(RuntimeTypeHandle);
                typeArray1[30] = typeof(Exception);
                typeArray1[0x1f] = typeof(ParamArrayAttribute);
                typeArray1[0x20] = typeof(OutAttribute);
                return typeArray1;
            }
        }

        private static System.Type[] AdditionalTypes
        {
            get
            {
                System.Type[] typeArray1 = new System.Type[0xa8];
                typeArray1[0] = typeof(Action);
                typeArray1[1] = typeof(Action<>);
                typeArray1[2] = typeof(Action<,>);
                typeArray1[3] = typeof(Action<,,>);
                typeArray1[4] = typeof(Action<,,,>);
                typeArray1[5] = typeof(ArgumentException);
                typeArray1[6] = typeof(ArgumentNullException);
                typeArray1[7] = typeof(ArgumentOutOfRangeException);
                typeArray1[8] = typeof(ArithmeticException);
                typeArray1[9] = typeof(ArraySegment<>);
                typeArray1[10] = typeof(ArrayTypeMismatchException);
                typeArray1[11] = typeof(AsyncCallback);
                typeArray1[12] = typeof(BitConverter);
                typeArray1[13] = typeof(Buffer);
                typeArray1[14] = typeof(Comparison<>);
                typeArray1[15] = typeof(System.Convert);
                typeArray1[0x10] = typeof(Converter<,>);
                typeArray1[0x11] = typeof(DateTime);
                typeArray1[0x12] = typeof(DateTimeKind);
                typeArray1[0x13] = typeof(DateTimeOffset);
                typeArray1[20] = typeof(DayOfWeek);
                typeArray1[0x15] = typeof(DivideByZeroException);
                typeArray1[0x16] = typeof(EventArgs);
                typeArray1[0x17] = typeof(EventHandler);
                typeArray1[0x18] = typeof(EventHandler<>);
                typeArray1[0x19] = typeof(FlagsAttribute);
                typeArray1[0x1a] = typeof(FormatException);
                typeArray1[0x1b] = typeof(Func<>);
                typeArray1[0x1c] = typeof(Func<,>);
                typeArray1[0x1d] = typeof(Func<,,>);
                typeArray1[30] = typeof(Func<,,,>);
                typeArray1[0x1f] = typeof(Func<,,,,>);
                typeArray1[0x20] = typeof(Guid);
                typeArray1[0x21] = typeof(IAsyncResult);
                typeArray1[0x22] = typeof(ICloneable);
                typeArray1[0x23] = typeof(IComparable);
                typeArray1[0x24] = typeof(IComparable<>);
                typeArray1[0x25] = typeof(IConvertible);
                typeArray1[0x26] = typeof(ICustomFormatter);
                typeArray1[0x27] = typeof(IEquatable<>);
                typeArray1[40] = typeof(IFormatProvider);
                typeArray1[0x29] = typeof(IFormattable);
                typeArray1[0x2a] = typeof(IndexOutOfRangeException);
                typeArray1[0x2b] = typeof(InvalidCastException);
                typeArray1[0x2c] = typeof(InvalidOperationException);
                typeArray1[0x2d] = typeof(InvalidTimeZoneException);
                typeArray1[0x2e] = typeof(Math);
                typeArray1[0x2f] = typeof(MidpointRounding);
                typeArray1[0x30] = typeof(NonSerializedAttribute);
                typeArray1[0x31] = typeof(NotFiniteNumberException);
                typeArray1[50] = typeof(NotImplementedException);
                typeArray1[0x33] = typeof(NotSupportedException);
                typeArray1[0x34] = typeof(Nullable);
                typeArray1[0x35] = typeof(Nullable<>);
                typeArray1[0x36] = typeof(NullReferenceException);
                typeArray1[0x37] = typeof(ObjectDisposedException);
                typeArray1[0x38] = typeof(ObsoleteAttribute);
                typeArray1[0x39] = typeof(OverflowException);
                typeArray1[0x3a] = typeof(Predicate<>);
                typeArray1[0x3b] = typeof(System.Random);
                typeArray1[60] = typeof(RankException);
                typeArray1[0x3d] = typeof(SerializableAttribute);
                typeArray1[0x3e] = typeof(StackOverflowException);
                typeArray1[0x3f] = typeof(StringComparer);
                typeArray1[0x40] = typeof(StringComparison);
                typeArray1[0x41] = typeof(StringSplitOptions);
                typeArray1[0x42] = typeof(SystemException);
                typeArray1[0x43] = typeof(TimeoutException);
                typeArray1[0x44] = typeof(TimeSpan);
                typeArray1[0x45] = typeof(TimeZone);
                typeArray1[70] = typeof(TimeZoneInfo);
                typeArray1[0x47] = typeof(TimeZoneNotFoundException);
                typeArray1[0x48] = typeof(TypeCode);
                typeArray1[0x49] = typeof(Version);
                typeArray1[0x4a] = typeof(WeakReference);
                typeArray1[0x4b] = typeof(BitArray);
                typeArray1[0x4c] = typeof(ICollection);
                typeArray1[0x4d] = typeof(IComparer);
                typeArray1[0x4e] = typeof(IDictionary);
                typeArray1[0x4f] = typeof(IDictionaryEnumerator);
                typeArray1[80] = typeof(IEqualityComparer);
                typeArray1[0x51] = typeof(IList);
                typeArray1[0x52] = typeof(Comparer<>);
                typeArray1[0x53] = typeof(Dictionary<,>);
                typeArray1[0x54] = typeof(EqualityComparer<>);
                typeArray1[0x55] = typeof(ICollection<>);
                typeArray1[0x56] = typeof(IComparer<>);
                typeArray1[0x57] = typeof(IDictionary<,>);
                typeArray1[0x58] = typeof(IEnumerable<>);
                typeArray1[0x59] = typeof(IEnumerator<>);
                typeArray1[90] = typeof(IEqualityComparer<>);
                typeArray1[0x5b] = typeof(IList<>);
                typeArray1[0x5c] = typeof(KeyNotFoundException);
                typeArray1[0x5d] = typeof(KeyValuePair<,>);
                typeArray1[0x5e] = typeof(List<>);
                typeArray1[0x5f] = typeof(Collection<>);
                typeArray1[0x60] = typeof(KeyedCollection<,>);
                typeArray1[0x61] = typeof(ReadOnlyCollection<>);
                typeArray1[0x62] = typeof(CharUnicodeInfo);
                typeArray1[0x63] = typeof(CultureInfo);
                typeArray1[100] = typeof(DateTimeFormatInfo);
                typeArray1[0x65] = typeof(DateTimeStyles);
                typeArray1[0x66] = typeof(NumberFormatInfo);
                typeArray1[0x67] = typeof(NumberStyles);
                typeArray1[0x68] = typeof(RegionInfo);
                typeArray1[0x69] = typeof(StringInfo);
                typeArray1[0x6a] = typeof(TextElementEnumerator);
                typeArray1[0x6b] = typeof(TextInfo);
                typeArray1[0x6c] = typeof(UnicodeCategory);
                typeArray1[0x6d] = typeof(BinaryReader);
                typeArray1[110] = typeof(BinaryWriter);
                typeArray1[0x6f] = typeof(BufferedStream);
                typeArray1[0x70] = typeof(EndOfStreamException);
                typeArray1[0x71] = typeof(FileAccess);
                typeArray1[0x72] = typeof(FileMode);
                typeArray1[0x73] = typeof(FileNotFoundException);
                typeArray1[0x74] = typeof(IOException);
                typeArray1[0x75] = typeof(MemoryStream);
                typeArray1[0x76] = typeof(Path);
                typeArray1[0x77] = typeof(PathTooLongException);
                typeArray1[120] = typeof(SeekOrigin);
                typeArray1[0x79] = typeof(Stream);
                typeArray1[0x7a] = typeof(StringReader);
                typeArray1[0x7b] = typeof(StringWriter);
                typeArray1[0x7c] = typeof(TextReader);
                typeArray1[0x7d] = typeof(TextWriter);
                typeArray1[0x7e] = typeof(ASCIIEncoding);
                typeArray1[0x7f] = typeof(System.Text.Decoder);
                typeArray1[0x80] = typeof(System.Text.Encoder);
                typeArray1[0x81] = typeof(Encoding);
                typeArray1[130] = typeof(EncodingInfo);
                typeArray1[0x83] = typeof(StringBuilder);
                typeArray1[0x84] = typeof(UnicodeEncoding);
                typeArray1[0x85] = typeof(UTF32Encoding);
                typeArray1[0x86] = typeof(UTF7Encoding);
                typeArray1[0x87] = typeof(UTF8Encoding);
                typeArray1[0x88] = typeof(LinkedList<>);
                typeArray1[0x89] = typeof(LinkedListNode<>);
                typeArray1[0x8a] = typeof(Queue<>);
                typeArray1[0x8b] = typeof(SortedDictionary<,>);
                typeArray1[140] = typeof(SortedList<,>);
                typeArray1[0x8d] = typeof(Stack<>);
                typeArray1[0x8e] = typeof(BitVector32);
                typeArray1[0x8f] = typeof(CompressionMode);
                typeArray1[0x90] = typeof(DeflateStream);
                typeArray1[0x91] = typeof(GZipStream);
                typeArray1[0x92] = typeof(Capture);
                typeArray1[0x93] = typeof(CaptureCollection);
                typeArray1[0x94] = typeof(System.Text.RegularExpressions.Group);
                typeArray1[0x95] = typeof(GroupCollection);
                typeArray1[150] = typeof(System.Text.RegularExpressions.Match);
                typeArray1[0x97] = typeof(MatchCollection);
                typeArray1[0x98] = typeof(MatchEvaluator);
                typeArray1[0x99] = typeof(Regex);
                typeArray1[0x9a] = typeof(RegexCompilationInfo);
                typeArray1[0x9b] = typeof(RegexOptions);
                typeArray1[0x9c] = typeof(HashSet<>);
                typeArray1[0x9d] = typeof(Enumerable);
                typeArray1[0x9e] = typeof(IGrouping<,>);
                typeArray1[0x9f] = typeof(ILookup<,>);
                typeArray1[160] = typeof(IOrderedEnumerable<>);
                typeArray1[0xa1] = typeof(IOrderedQueryable);
                typeArray1[0xa2] = typeof(IOrderedQueryable<>);
                typeArray1[0xa3] = typeof(IQueryable);
                typeArray1[0xa4] = typeof(IQueryable<>);
                typeArray1[0xa5] = typeof(IQueryProvider);
                typeArray1[0xa6] = typeof(Lookup<,>);
                typeArray1[0xa7] = typeof(Queryable);
                return typeArray1;
            }
        }

        private static System.Type[] UnityEngineTypes
        {
            get
            {
                return new System.Type[] { typeof(Logger), typeof(Debug) };
            }
        }

        private static System.Type[] GameTypes
        {
            get
            {
                return new System.Type[] { typeof(ScriptBase), typeof(ScriptAttribute), typeof(MHRandom), typeof(Multitype<,>), typeof(Multitype<,,>), typeof(Multitype<,,,>) };
            }
        }
    }
}

