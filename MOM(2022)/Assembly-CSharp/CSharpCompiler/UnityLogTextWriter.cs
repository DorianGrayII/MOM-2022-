// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CSharpCompiler.UnityLogTextWriter
using System.IO;
using System.Text;
using UnityEngine;

internal class UnityLogTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.ASCII;

    public override void Write(string value)
    {
        Debug.Log(value);
    }
}
