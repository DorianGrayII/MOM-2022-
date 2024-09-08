// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.MemoryManager
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHUtils;
using UnityEngine;

public class MemoryManager
{
    private static MemoryManager instance;

    private List<Object> gameplayLife = new List<Object>();

    private List<Object> permanentLife = new List<Object>();

    private List<object> turnLife = new List<object>();

    public static MemoryManager Get()
    {
        if (MemoryManager.instance == null)
        {
            MemoryManager.instance = new MemoryManager();
        }
        return MemoryManager.instance;
    }

    public static void RegisterPermanent(object obj)
    {
        MemoryManager.Get().permanentLife.Add(obj as Object);
    }

    public static void Register(object obj)
    {
        if (obj is GameObject || obj is Component || obj is Mesh || obj is Material || obj is Texture2D)
        {
            if (obj is Object)
            {
                MemoryManager.Get().gameplayLife.Add(obj as Object);
            }
            else
            {
                Debug.LogError("obj is not UnityEngine.Object, " + obj);
            }
        }
        else if (obj is Stream)
        {
            MemoryManager.Get().turnLife.Add(obj);
        }
        else
        {
            Debug.LogError("Unsupported type! " + obj);
        }
    }

    public static void LogStatus()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("-= Memory Tracker Report =-");
        stringBuilder.AppendLine("Turn Count: " + MemoryManager.Get().turnLife.Count);
        stringBuilder.AppendLine("Gameplay Count: " + MemoryManager.Get().gameplayLife.Count);
        stringBuilder.AppendLine("Permanent Count: " + MemoryManager.Get().permanentLife.Count);
        if (MemoryManager.Get().turnLife.Count > 0)
        {
            stringBuilder.AppendLine("-------------------");
            stringBuilder.AppendLine("Turn items: ");
            for (int i = 0; i < MemoryManager.Get().turnLife.Count; i++)
            {
                stringBuilder.AppendLine(i + ": " + MemoryManager.Get().turnLife[i]);
            }
        }
        if (MemoryManager.Get().turnLife.Count > 0)
        {
            stringBuilder.AppendLine("-------------------");
            stringBuilder.AppendLine("Gameplay items: ");
            for (int j = 0; j < MemoryManager.Get().gameplayLife.Count; j++)
            {
                stringBuilder.AppendLine(j + ": " + MemoryManager.Get().gameplayLife[j]);
            }
        }
        if (MemoryManager.Get().turnLife.Count > 0)
        {
            stringBuilder.AppendLine("-------------------");
            stringBuilder.AppendLine("Permanent items: ");
            for (int k = 0; k < MemoryManager.Get().permanentLife.Count; k++)
            {
                stringBuilder.AppendLine(k + ": " + MemoryManager.Get().permanentLife[k]);
            }
        }
        Debug.Log(stringBuilder.ToString());
    }

    public static void ClearGameplay()
    {
        foreach (Object item in MemoryManager.Get().gameplayLife)
        {
            if (item != null)
            {
                Object.Destroy(item);
            }
        }
        MemoryManager.Get().gameplayLife.Clear();
    }

    public static void ClearTurnStorages()
    {
        foreach (object item in MemoryManager.Get().turnLife)
        {
            if (item != null && item is Stream)
            {
                (item as Stream).Dispose();
            }
        }
        MemoryManager.Get().gameplayLife.Clear();
    }
}
