// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CSharpCompiler.UnityErrorTextWriter
using System.IO;
using System.Text;
using UnityEngine;

internal class UnityErrorTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.ASCII;

    public override void Write(string value)
    {
        Debug.LogError(value);
    }
}
