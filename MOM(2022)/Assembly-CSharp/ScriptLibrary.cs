using MHUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class ScriptLibrary
{
    public static Dictionary<string, MethodInfo> delegates = new Dictionary<string, MethodInfo>();

    public static object Call(string scriptName, params object[] parameters)
    {
        int num;
        return Call(out num, scriptName, parameters);
    }

    public static object Call(out int error, string scriptName, params object[] parameters)
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            Debug.LogError("ScriptLibrary.Call for script name = null.");
            error = 1;
            return null;
        }
        MethodInfo info = Get(scriptName);
        if (info == null)
        {
            error = 3;
            Debug.LogError("[ERROR] Script named " + scriptName + " doesn't exist");
        }
        else
        {
            try
            {
                error = 0;
                return info.Invoke(null, parameters);
            }
            catch (Exception exception)
            {
                error = 2;
                string str = "";
                if (delegates[scriptName].GetParameters() == null)
                {
                    if (parameters == null)
                    {
                        string text3;
                        if (exception != null)
                        {
                            text3 = exception.ToString();
                        }
                        else
                        {
                            Exception local3 = exception;
                            text3 = null;
                        }
                        Debug.LogError("[ERROR]Script method failure for " + scriptName + " expected parameter count: 0, received: 0 \n" + text3);
                    }
                    else
                    {
                        string text4;
                        string[] textArray2 = new string[6];
                        textArray2[0] = "[ERROR]Script method failure for ";
                        textArray2[1] = scriptName;
                        textArray2[2] = " expected parameter count: 0, received: ";
                        textArray2[3] = parameters.Length.ToString();
                        textArray2[4] = "\n";
                        string[] textArray4 = textArray2;
                        if (exception != null)
                        {
                            text4 = exception.ToString();
                        }
                        else
                        {
                            Exception local4 = exception;
                            text4 = null;
                        }
                        textArray2[5] = text4;
                        Debug.LogError(string.Concat(textArray2));
                    }
                }
                else
                {
                    int index = 0;
                    while (true)
                    {
                        if (index >= delegates[scriptName].GetParameters().Length)
                        {
                            if (delegates[scriptName].GetParameters().Length == parameters.Length)
                            {
                                string text2;
                                if (exception != null)
                                {
                                    text2 = exception.ToString();
                                }
                                else
                                {
                                    Exception local2 = exception;
                                    text2 = null;
                                }
                                Debug.LogError("[ERROR]Method " + scriptName + "\n" + text2);
                            }
                            else
                            {
                                string text1;
                                string[] textArray1 = new string[9];
                                textArray1[0] = "[ERROR]Method ";
                                textArray1[1] = scriptName;
                                textArray1[2] = " expected parameter count: ";
                                textArray1[3] = delegates[scriptName].GetParameters().Length.ToString();
                                textArray1[4] = ", received: ";
                                textArray1[5] = parameters.Length.ToString();
                                textArray1[6] = str;
                                textArray1[7] = "\n";
                                string[] textArray3 = textArray1;
                                if (exception != null)
                                {
                                    text1 = exception.ToString();
                                }
                                else
                                {
                                    Exception local1 = exception;
                                    text1 = null;
                                }
                                textArray1[8] = text1;
                                Debug.LogError(string.Concat(textArray1));
                            }
                            break;
                        }
                        str = str + "\n" + delegates[scriptName].GetParameters()[index].ToString();
                        if ((parameters != null) && (parameters.Length > index))
                        {
                            str = str + " vs " + ((parameters[index] == null) ? "null" : parameters[index].ToString());
                        }
                        index++;
                    }
                }
            }
        }
        return null;
    }

    public static object CallNoCatch(string scriptName, params object[] parameters)
    {
        if (!string.IsNullOrEmpty(scriptName))
        {
            return Get(scriptName).Invoke(null, parameters);
        }
        Debug.LogError("ScriptLibrary.Call for script name = null.");
        return null;
    }

    public static List<string> GeDisplayNamesOfType(ScriptType.Type type)
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, MethodInfo> pair in delegates)
        {
            ScriptType customAttribute = CustomAttributeExtensions.GetCustomAttribute<ScriptType>(pair.Value);
            if ((customAttribute != null) && (customAttribute.eType == type))
            {
                string displayName = CustomAttributeExtensions.GetCustomAttribute<ScriptParameters>(pair.Value).displayName;
                if (displayName != null)
                {
                    list.Add(displayName);
                }
            }
        }
        return list;
    }

    public static MethodInfo Get(string scriptName)
    {
        return ((string.IsNullOrEmpty(scriptName) || !delegates.ContainsKey(scriptName)) ? null : delegates[scriptName]);
    }

    public static IEnumerable<ScriptParameters> GetMetodParameterType(string name)
    {
        IEnumerable<ScriptParameters> customAttributes;
        using (Dictionary<string, MethodInfo>.Enumerator enumerator = delegates.GetEnumerator())
        {
            while (true)
            {
                if (enumerator.MoveNext())
                {
                    KeyValuePair<string, MethodInfo> current = enumerator.Current;
                    if (current.Key != name)
                    {
                        continue;
                    }
                    customAttributes = CustomAttributeExtensions.GetCustomAttributes<ScriptParameters>(current.Value);
                }
                else
                {
                    return null;
                }
                break;
            }
        }
        return customAttributes;
    }

    public static List<string> GetMetodsNamesOfType(ScriptType.Type type)
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, MethodInfo> pair in delegates)
        {
            ScriptType customAttribute = CustomAttributeExtensions.GetCustomAttribute<ScriptType>(pair.Value);
            if ((customAttribute != null) && (customAttribute.eType == type))
            {
                list.Add(pair.Key);
            }
        }
        return list;
    }

    public static void SetScript(string name, MethodInfo reference)
    {
        delegates[name] = reference;
    }
}

