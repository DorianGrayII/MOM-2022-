using System.IO;
using System.Text;
using UnityEngine;

namespace CSharpCompiler
{
    internal class UnityLogTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.ASCII;

        public override void Write(string value)
        {
            Debug.Log(value);
        }
    }
}
