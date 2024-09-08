using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ConsoleProDebug
{
    public static void Clear()
    {
    }

    public static void LogAsType(string inLog, string inTypeName, UnityEngine.Object inContext)
    {
        Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"LogType\", \"name\":\"" + inTypeName + "\"}", inContext);
    }

    public static void LogToFilter(string inLog, string inFilterName, UnityEngine.Object inContext)
    {
        Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\", \"name\":\"" + inFilterName + "\"}", inContext);
    }

    public static void Search(string inText)
    {
        Debug.Log("\nCPAPI:{\"cmd\":\"Search\", \"text\":\"" + inText + "\"}");
    }

    public static void Watch(string inName, string inValue)
    {
        string[] textArray1 = new string[] { inName, " : ", inValue, "\nCPAPI:{\"cmd\":\"Watch\", \"name\":\"", inName, "\"}" };
        Debug.Log(string.Concat(textArray1));
    }
}

