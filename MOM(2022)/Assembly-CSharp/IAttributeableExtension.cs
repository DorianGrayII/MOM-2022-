using DBDef;
using DBEnum;
using MHUtils;

public static class IAttributeableExtension
{
    public static void EnsureFinal(this IAttributable t)
    {
        if (t.GetAttributes().GetDirty())
        {
            t.GetAttributes().SetDirty();
            ScriptLibrary.Call("UpdateAttributes", t);
        }
    }

    public static FInt GetAttFinal(this IAttributable t, TAG tag)
    {
        return t.GetAttributes().GetFinal(tag);
    }

    public static FInt GetAttFinal(this IAttributable t, Tag tag)
    {
        return t.GetAttributes().GetFinal(tag);
    }
}
