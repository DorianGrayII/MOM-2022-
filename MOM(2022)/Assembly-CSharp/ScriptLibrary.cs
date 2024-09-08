using System;
using System.Collections.Generic;
using System.Reflection;
using MHUtils;
using UnityEngine;

public class ScriptLibrary
{
    public static Dictionary<string, MethodInfo> delegates = new Dictionary<string, MethodInfo>();

    public static void SetScript(string name, MethodInfo reference)
    {
        ScriptLibrary.delegates[name] = reference;
    }

    public static object Call(string scriptName, params object[] parameters)
    {
        int error;
        return ScriptLibrary.Call(out error, scriptName, parameters);
    }

    public static object CallNoCatch(string scriptName, params object[] parameters)
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            Debug.LogError("ScriptLibrary.Call for script name = null.");
            return null;
        }
        return ScriptLibrary.Get(scriptName).Invoke(null, parameters);
    }

    public static object Call(out int error, string scriptName, params object[] parameters)
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            Debug.LogError("ScriptLibrary.Call for script name = null.");
            error = 1;
            return null;
        }
        MethodInfo methodInfo = ScriptLibrary.Get(scriptName);
        if (methodInfo != null)
        {
            try
            {
                error = 0;
                return methodInfo.Invoke(null, parameters);
            }
            catch (Exception ex)
            {
                error = 2;
                string text = "";
                if (ScriptLibrary.delegates[scriptName].GetParameters() != null)
                {
                    for (int i = 0; i < ScriptLibrary.delegates[scriptName].GetParameters().Length; i++)
                    {
                        ParameterInfo parameterInfo = ScriptLibrary.delegates[scriptName].GetParameters()[i];
                        text = text + "\n" + parameterInfo.ToString();
                        if (parameters != null && parameters.Length > i)
                        {
                            text = text + " vs " + ((parameters[i] == null) ? "null" : parameters[i].ToString());
                        }
                    }
                    if (ScriptLibrary.delegates[scriptName].GetParameters().Length != parameters.Length)
                    {
                        Debug.LogError("[ERROR]Method " + scriptName + " expected parameter count: " + ScriptLibrary.delegates[scriptName].GetParameters().Length + ", received: " + parameters.Length + text + "\n" + ex);
                    }
                    else
                    {
                        Debug.LogError("[ERROR]Method " + scriptName + "\n" + ex);
                    }
                }
                else if (parameters == null)
                {
                    Debug.LogError("[ERROR]Script method failure for " + scriptName + " expected parameter count: 0, received: 0 \n" + ex);
                }
                else
                {
                    Debug.LogError("[ERROR]Script method failure for " + scriptName + " expected parameter count: 0, received: " + parameters.Length + "\n" + ex);
                }
            }
        }
        else
        {
            error = 3;
            Debug.LogError("[ERROR] Script named " + scriptName + " doesn't exist");
        }
        return null;
    }

    public static MethodInfo Get(string scriptName)
    {
        if (!string.IsNullOrEmpty(scriptName) && ScriptLibrary.delegates.ContainsKey(scriptName))
        {
            return ScriptLibrary.delegates[scriptName];
        }
        return null;
    }

    public static List<string> GetMetodsNamesOfType(ScriptType.Type type)
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, MethodInfo> @delegate in ScriptLibrary.delegates)
        {
            ScriptType customAttribute = @delegate.Value.GetCustomAttribute<ScriptType>();
            if (customAttribute != null && customAttribute.eType == type)
            {
                list.Add(@delegate.Key);
            }
        }
        return list;
    }

    public static List<string> GeDisplayNamesOfType(ScriptType.Type type)
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, MethodInfo> @delegate in ScriptLibrary.delegates)
        {
            ScriptType customAttribute = @delegate.Value.GetCustomAttribute<ScriptType>();
            if (customAttribute != null && customAttribute.eType == type)
            {
                string displayName = @delegate.Value.GetCustomAttribute<ScriptParameters>().displayName;
                if (displayName != null)
                {
                    list.Add(displayName);
                }
            }
        }
        return list;
    }

    public static IEnumerable<ScriptParameters> GetMetodParameterType(string name)
    {
        foreach (KeyValuePair<string, MethodInfo> @delegate in ScriptLibrary.delegates)
        {
            if (@delegate.Key == name)
            {
                return @delegate.Value.GetCustomAttributes<ScriptParameters>();
            }
        }
        return null;
    }
}
