// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.ScriptLoader
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CSharpCompiler;
using MHUtils;
using UnityEngine;

public class ScriptLoader : MonoBehaviour
{
    private static string header = "#if USE_DEBUG_SCRIPT && UNITY_EDITOR";

    private static string exportedHeader = "#if !USE_DEBUG_SCRIPT || !UNITY_EDITOR";

    public static ScriptLoader instance;

    private DeferredSynchronizeInvoke synchronizedInvoke;

    private ScriptBundleLoader loader;

    private void Start()
    {
        ScriptLoader.instance = this;
        this.synchronizedInvoke = new DeferredSynchronizeInvoke();
        this.loader = new ScriptBundleLoader(this.synchronizedInvoke);
        this.loader.logWriter = new UnityLogTextWriter();
        this.loader.createInstance = (Type t) => typeof(Component).IsAssignableFrom(t) ? base.gameObject.AddComponent(t) : Activator.CreateInstance(t);
        this.loader.destroyInstance = delegate(object instance)
        {
            if (instance is Component)
            {
                global::UnityEngine.Object.Destroy(instance as Component);
            }
        };
    }

    public static ScriptLoader Get()
    {
        return ScriptLoader.instance;
    }

    private void Update()
    {
        if (this.synchronizedInvoke != null)
        {
            this.synchronizedInvoke.ProcessQueue();
        }
    }

    public void LoadScripts()
    {
        List<string> list = new List<string>();
        string[] files = Directory.GetFiles(Path.Combine(MHApplication.EXTERNAL_ASSETS, "Scripts"), "*", SearchOption.AllDirectories);
        foreach (string text in files)
        {
            if (text.EndsWith(".cs"))
            {
                list.Add(text);
            }
        }
        list.AddRange(this.LoadModScripts());
        if (list.Count > 0)
        {
            this.loader.LoadAndWatchScriptsBundle(list.ToArray());
        }
    }

    private List<string> LoadModScripts()
    {
        List<ModOrder> activeValidMods = ModManager.Get().GetActiveValidMods();
        List<string> list = new List<string>();
        foreach (ModOrder item in activeValidMods)
        {
            string path = item.GetPath();
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Path for mod " + item.name + " is missing!");
                continue;
            }
            string path2 = Path.Combine(path, "Scripts");
            if (!Directory.Exists(path2))
            {
                continue;
            }
            string[] files = Directory.GetFiles(path2);
            foreach (string text in files)
            {
                if (text.EndsWith(".cs"))
                {
                    list.Add(text);
                }
            }
        }
        return list;
    }

    private void LoadDebugScripts()
    {
        Type typeFromHandle = typeof(ScriptBase);
        Type[] types = Assembly.GetAssembly(typeFromHandle).GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeFromHandle))
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (MethodInfo methodInfo in methods)
                {
                    ScriptLibrary.SetScript(methodInfo.Name, methodInfo);
                }
            }
        }
    }

    public static void UpdateScriptToStreaming()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string path = Path.Combine(Application.dataPath, "ScriptingSources");
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string text in files)
            {
                if (text.EndsWith(".cs"))
                {
                    string fileName = Path.GetFileName(text);
                    string value = File.ReadAllText(text);
                    dictionary[fileName] = value;
                }
            }
        }
        foreach (KeyValuePair<string, string> item in dictionary)
        {
            string value2 = item.Value.Replace(ScriptLoader.header, ScriptLoader.exportedHeader);
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Path.Combine(Application.streamingAssetsPath, "Scripts"), item.Key), append: false, Encoding.UTF8))
            {
                streamWriter.Write(value2);
            }
        }
    }

    public static void UpdateExternalScripts()
    {
        string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "Scripts");
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        ScriptLoader.LoadFilesFromTestingScripts(dictionary);
        foreach (KeyValuePair<string, string> item in dictionary)
        {
            string path2 = Path.Combine(path, item.Key);
            string text = item.Value.Replace(ScriptLoader.header, ScriptLoader.exportedHeader);
            if (text == item.Value)
            {
                Debug.LogError("file " + item.Key + " did not correctly update header #if for external scripts");
            }
            else
            {
                File.WriteAllText(path2, text);
            }
        }
    }

    private static void LoadFilesFromTestingScripts(Dictionary<string, string> fileToContent)
    {
        string path = Path.Combine(Application.dataPath, "ScriptingSources");
        if (!Directory.Exists(path))
        {
            return;
        }
        string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        new List<string>();
        string[] array = files;
        foreach (string text in array)
        {
            if (text.EndsWith(".cs"))
            {
                string value = File.ReadAllText(text);
                fileToContent[Path.GetFileName(text)] = value;
            }
        }
    }

    public static int GetScriptHash()
    {
        string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "Scripts");
        MHTimer mHTimer = MHTimer.StartNew();
        List<string> list = new List<string>(Directory.GetFiles(path)).FindAll((string o) => o.EndsWith(".cs"));
        int num = 0;
        foreach (string item in list)
        {
            if (File.Exists(item))
            {
                string text = File.ReadAllText(item);
                num ^= text.GetHashCode();
            }
        }
        Debug.Log("Script hash " + num + " took " + mHTimer.GetTime());
        return num;
    }
}
