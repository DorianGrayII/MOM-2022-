using System.IO;
using System.Text;
using UnityEngine;

namespace CSharpCompiler
{
    internal class UnityErrorTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.ASCII;

        public override void Write(string value)
        {
            Debug.LogError(value);
        }
    }
}
