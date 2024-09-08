using System;
using System.Collections.Generic;
using System.Text;
using MHUtils;
using UnityEngine;

public static class ListUtils
{
    public static void Invert<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count / 2; i++)
        {
            int index = list.Count - 1 - i;
            T value = list[i];
            list[i] = list[index];
            list[index] = value;
        }
    }

    public static void RandomSort<T>(this List<T> list)
    {
        if (list != null && list.Count >= 2)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int index = global::UnityEngine.Random.Range(0, list.Count);
                T value = list[i];
                list[i] = list[index];
                list[index] = value;
            }
        }
    }

    public static void RandomSortThreadSafe<T>(this List<T> list)
    {
        if (list != null && list.Count >= 2)
        {
            MHRandom mHRandom = new MHRandom();
            for (int i = 0; i < list.Count; i++)
            {
                int @int = mHRandom.GetInt(0, list.Count);
                T value = list[i];
                list[i] = list[@int];
                list[@int] = value;
            }
        }
    }

    public static void SortInPlace<T>(this List<T> list, Comparison<T> comparer)
    {
        if (list != null && list.Count >= 2)
        {
            list.Sort(comparer);
        }
    }

    public static void RemoveLast<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            list.RemoveAt(list.Count - 1);
        }
    }

    public static void Print<T>(this List<T> list)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Print list of " + (list?.Count ?? 0) + " items");
        if (list != null)
        {
            foreach (T item in list)
            {
                stringBuilder.AppendLine(item.ToString());
            }
        }
        Debug.Log(stringBuilder.ToString());
    }

    public static IEnumerable<T> MultiEnumerable<T>(T a, List<T> b)
    {
        if (a != null)
        {
            yield return a;
        }
        if (b != null)
        {
            List<T>.Enumerator e = b.GetEnumerator();
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }

    public static IEnumerable<T> MultiEnumerable<T>(List<T> a, List<T> b)
    {
        if (a != null)
        {
            List<T>.Enumerator e2 = a.GetEnumerator();
            while (e2.MoveNext())
            {
                yield return e2.Current;
            }
        }
        if (b != null)
        {
            List<T>.Enumerator e2 = b.GetEnumerator();
            while (e2.MoveNext())
            {
                yield return e2.Current;
            }
        }
    }

    public static T BestAt<T>(this List<T> list, Comparison<T> comparer)
    {
        if (list.Count < 1)
        {
            return default(T);
        }
        T val = list[0];
        if (list.Count == 1)
        {
            return val;
        }
        for (int i = 1; i < list.Count; i++)
        {
            if (comparer(val, list[i]) < 0)
            {
                val = list[i];
            }
        }
        return val;
    }
}
