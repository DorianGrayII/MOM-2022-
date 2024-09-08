// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHZombieMemoryDetector
using System;
using System.Collections;
using System.Collections.Generic;

public class MHZombieMemoryDetector
{
    private const bool USE_DETECTOR = false;

    private static MHZombieMemoryDetector instance;

    private List<WeakReference> allocations = new List<WeakReference>(64000);

    private static MHZombieMemoryDetector Get()
    {
        if (MHZombieMemoryDetector.instance == null)
        {
            MHZombieMemoryDetector.instance = new MHZombieMemoryDetector();
        }
        return MHZombieMemoryDetector.instance;
    }

    public static void Track(object obj)
    {
    }

    public static string LogStatus()
    {
        return null;
    }

    public static void Clear()
    {
        MHZombieMemoryDetector.Get().allocations.Clear();
    }

    public static IEnumerator LogToFile()
    {
        yield return null;
    }
}
