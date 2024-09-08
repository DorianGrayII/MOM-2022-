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
using DBDef;
using MHUtils;
using Mono.CSharp;
using UnityEngine;

namespace CSharpCompiler
{
    public class CodeSandbox
    {
        private MethodInfo importTypes;

        private ReflectionImporter ri;

        private ModuleContainer mc;

        private static Type[] BuiltInTypes => new Type[33]
        {
            typeof(object),
            typeof(ValueType),
            typeof(global::System.Attribute),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(short),
            typeof(decimal),
            typeof(bool),
            typeof(sbyte),
            typeof(byte),
            typeof(ushort),
            typeof(string),
            typeof(global::System.Enum),
            typeof(global::System.Delegate),
            typeof(MulticastDelegate),
            typeof(void),
            typeof(Array),
            typeof(Type),
            typeof(IEnumerator),
            typeof(IEnumerable),
            typeof(IDisposable),
            typeof(IntPtr),
            typeof(UIntPtr),
            typeof(RuntimeFieldHandle),
            typeof(RuntimeTypeHandle),
            typeof(Exception),
            typeof(ParamArrayAttribute),
            typeof(OutAttribute)
        };

        private static Type[] AdditionalTypes => new Type[168]
        {
            typeof(Action),
            typeof(Action<>),
            typeof(Action<, >),
            typeof(Action<, , >),
            typeof(Action<, , , >),
            typeof(ArgumentException),
            typeof(ArgumentNullException),
            typeof(ArgumentOutOfRangeException),
            typeof(ArithmeticException),
            typeof(ArraySegment<>),
            typeof(ArrayTypeMismatchException),
            typeof(AsyncCallback),
            typeof(BitConverter),
            typeof(Buffer),
            typeof(Comparison<>),
            typeof(Convert),
            typeof(Converter<, >),
            typeof(DateTime),
            typeof(DateTimeKind),
            typeof(DateTimeOffset),
            typeof(DayOfWeek),
            typeof(DivideByZeroException),
            typeof(EventArgs),
            typeof(EventHandler),
            typeof(EventHandler<>),
            typeof(FlagsAttribute),
            typeof(FormatException),
            typeof(Func<>),
            typeof(Func<, >),
            typeof(Func<, , >),
            typeof(Func<, , , >),
            typeof(Func<, , , , >),
            typeof(Guid),
            typeof(IAsyncResult),
            typeof(ICloneable),
            typeof(IComparable),
            typeof(IComparable<>),
            typeof(IConvertible),
            typeof(ICustomFormatter),
            typeof(IEquatable<>),
            typeof(IFormatProvider),
            typeof(IFormattable),
            typeof(IndexOutOfRangeException),
            typeof(InvalidCastException),
            typeof(InvalidOperationException),
            typeof(InvalidTimeZoneException),
            typeof(Math),
            typeof(MidpointRounding),
            typeof(NonSerializedAttribute),
            typeof(NotFiniteNumberException),
            typeof(NotImplementedException),
            typeof(NotSupportedException),
            typeof(Nullable),
            typeof(Nullable<>),
            typeof(NullReferenceException),
            typeof(ObjectDisposedException),
            typeof(ObsoleteAttribute),
            typeof(OverflowException),
            typeof(Predicate<>),
            typeof(global::System.Random),
            typeof(RankException),
            typeof(SerializableAttribute),
            typeof(StackOverflowException),
            typeof(StringComparer),
            typeof(StringComparison),
            typeof(StringSplitOptions),
            typeof(SystemException),
            typeof(TimeoutException),
            typeof(TimeSpan),
            typeof(TimeZone),
            typeof(TimeZoneInfo),
            typeof(TimeZoneNotFoundException),
            typeof(TypeCode),
            typeof(Version),
            typeof(WeakReference),
            typeof(BitArray),
            typeof(ICollection),
            typeof(IComparer),
            typeof(IDictionary),
            typeof(IDictionaryEnumerator),
            typeof(IEqualityComparer),
            typeof(IList),
            typeof(Comparer<>),
            typeof(Dictionary<, >),
            typeof(EqualityComparer<>),
            typeof(ICollection<>),
            typeof(IComparer<>),
            typeof(IDictionary<, >),
            typeof(IEnumerable<>),
            typeof(IEnumerator<>),
            typeof(IEqualityComparer<>),
            typeof(IList<>),
            typeof(KeyNotFoundException),
            typeof(KeyValuePair<, >),
            typeof(List<>),
            typeof(Collection<>),
            typeof(KeyedCollection<, >),
            typeof(ReadOnlyCollection<>),
            typeof(CharUnicodeInfo),
            typeof(CultureInfo),
            typeof(DateTimeFormatInfo),
            typeof(DateTimeStyles),
            typeof(NumberFormatInfo),
            typeof(NumberStyles),
            typeof(RegionInfo),
            typeof(StringInfo),
            typeof(TextElementEnumerator),
            typeof(TextInfo),
            typeof(UnicodeCategory),
            typeof(BinaryReader),
            typeof(BinaryWriter),
            typeof(BufferedStream),
            typeof(EndOfStreamException),
            typeof(FileAccess),
            typeof(FileMode),
            typeof(FileNotFoundException),
            typeof(IOException),
            typeof(MemoryStream),
            typeof(Path),
            typeof(PathTooLongException),
            typeof(SeekOrigin),
            typeof(Stream),
            typeof(StringReader),
            typeof(StringWriter),
            typeof(TextReader),
            typeof(TextWriter),
            typeof(ASCIIEncoding),
            typeof(Decoder),
            typeof(Encoder),
            typeof(Encoding),
            typeof(EncodingInfo),
            typeof(StringBuilder),
            typeof(UnicodeEncoding),
            typeof(UTF32Encoding),
            typeof(UTF7Encoding),
            typeof(UTF8Encoding),
            typeof(LinkedList<>),
            typeof(LinkedListNode<>),
            typeof(Queue<>),
            typeof(SortedDictionary<, >),
            typeof(SortedList<, >),
            typeof(Stack<>),
            typeof(BitVector32),
            typeof(CompressionMode),
            typeof(DeflateStream),
            typeof(GZipStream),
            typeof(Capture),
            typeof(CaptureCollection),
            typeof(global::System.Text.RegularExpressions.Group),
            typeof(GroupCollection),
            typeof(Match),
            typeof(MatchCollection),
            typeof(MatchEvaluator),
            typeof(Regex),
            typeof(RegexCompilationInfo),
            typeof(RegexOptions),
            typeof(HashSet<>),
            typeof(Enumerable),
            typeof(IGrouping<, >),
            typeof(ILookup<, >),
            typeof(IOrderedEnumerable<>),
            typeof(IOrderedQueryable),
            typeof(IOrderedQueryable<>),
            typeof(IQueryable),
            typeof(IQueryable<>),
            typeof(IQueryProvider),
            typeof(Lookup<, >),
            typeof(Queryable)
        };

        private static Type[] UnityEngineTypes => new Type[2]
        {
            typeof(Logger),
            typeof(Debug)
        };

        private static Type[] GameTypes => new Type[6]
        {
            typeof(ScriptBase),
            typeof(ScriptAttribute),
            typeof(MHRandom),
            typeof(Multitype<, >),
            typeof(Multitype<, , >),
            typeof(Multitype<, , , >)
        };

        public CodeSandbox(CompilerContext ctx)
        {
            ctx.Settings.AssemblyReferences.Clear();
            ctx.Settings.LoadDefaultReferences = false;
            ctx.Settings.StdLib = false;
        }

        public void RegisterTypes(ReflectionImporter importer, ModuleContainer module)
        {
            this.importTypes = importer.GetType().GetMethod("ImportTypes", BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.Any, new Type[3]
            {
                typeof(Type[]),
                typeof(Namespace),
                typeof(bool)
            }, null);
            this.ri = importer;
            this.mc = module;
            int num = 0;
            num += this.Import(CodeSandbox.BuiltInTypes);
            num += this.Import(CodeSandbox.AdditionalTypes);
            Debug.Log("Allowed Access to " + (num + this.Import(CodeSandbox.UnityEngineTypes)) + " external types");
            num = 0;
            num += this.Import(CodeSandbox.GameTypes);
            num += this.Import(CodeSandbox.GetSubTypes(typeof(DBClass)));
            Debug.Log("Allowed Access to " + (num + this.Import(CodeSandbox.GetNamespaceEnumsTypes("DBDef"))) + " game types");
        }

        private int Import(Type[] type)
        {
            this.importTypes.Invoke(this.ri, new object[3]
            {
                type,
                this.mc.GlobalRootNamespace,
                false
            });
            return type.Length;
        }

        private static Type[] GetSubTypes(Type t)
        {
            return Array.FindAll(t.Assembly.GetTypes(), (Type o) => o.IsSubclassOf(t));
        }

        private static Type[] GetNamespaceEnumsTypes(string nameSpace)
        {
            return Array.FindAll(Assembly.GetExecutingAssembly().GetTypes(), (Type o) => o.IsEnum && o.Namespace == nameSpace);
        }
    }
}
