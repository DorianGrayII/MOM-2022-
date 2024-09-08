using DBDef;
using DBEnum;
using MHUtils;
using System;
using System.Runtime.CompilerServices;

[Extension]
public static class IAttributeableExtension
{
    [Extension]
    public static void EnsureFinal(IAttributable t)
    {
        if (t.GetAttributes().GetDirty())
        {
            t.GetAttributes().SetDirty();
            object[] parameters = new object[] { t };
            ScriptLibrary.Call("UpdateAttributes", parameters);
        }
    }

    [Extension]
    public static FInt GetAttFinal(IAttributable t, Tag tag)
    {
        return t.GetAttributes().GetFinal(tag);
    }

    [Extension]
    public static FInt GetAttFinal(IAttributable t, TAG tag)
    {
        return t.GetAttributes().GetFinal(tag);
    }
}

