// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.ModulesImport
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHUtils;
using MOM;
using MOM.Adventures;
using UnityEngine;

public class ModulesImport : MonoBehaviour
{
    public const string EVENT = "-- [EVENT] --";

    public const string NODE = "+[NODE]";

    public const string STORY = "[STORY]";

    public const string OUT = "[OUT]";

    public const string EVENT_END = "[/EVENT]";

    public const string NODE_END = "[/NODE]";

    public const string STORY_END = "[/STORY]";

    public const string FILE_NAME = "[FILE NAME:]";

    public const string ID = "[ID]";

    public const string TXT = "[TXT]";

    public const string bundleBlockName = "BundleBlock";

    public static Multitype<string, int, int> importDataError;

    public static void SingleModeExport(SimpleCallback onReady, SimpleCallback onError)
    {
        Module selectedModule = EventEditorModules.GetSelectedModule();
        if (selectedModule != null)
        {
            ModulesImport.ExportDialogForModule(selectedModule, onReady, onError);
            ModulesImport.ExportModuleForVO(selectedModule);
        }
    }

    private static void ExportModuleForVO(Module module)
    {
        string text = Path.Combine(Application.streamingAssetsPath, "Exported Modules");
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        text = Path.Combine(text, "VO");
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        text = Path.Combine(text, module.name);
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        if (module.adventures == null)
        {
            return;
        }
        StreamWriter streamWriter = new StreamWriter(text + "/VO.txt");
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < module.adventures.Count; i++)
        {
            Adventure adventure = module.adventures[i];
            stringBuilder.AppendLine("-- [EVENT] -- TITLE: " + adventure.name);
            if (adventure.nodes != null)
            {
                for (int j = 0; j < adventure.nodes.Count; j++)
                {
                    BaseNode baseNode = adventure.nodes[j];
                    if (baseNode is NodeStory)
                    {
                        stringBuilder.AppendLine("[FILE NAME:]" + adventure.uniqueID + "_" + baseNode.ID + ".ogg");
                        stringBuilder.AppendLine((baseNode as NodeStory).story);
                        stringBuilder.AppendLine("");
                    }
                }
            }
            stringBuilder.AppendLine("");
        }
        streamWriter.Write(stringBuilder.ToString().ToCharArray());
        streamWriter.Close();
    }

    private static void ExportDialogForModule(Module module, SimpleCallback onReady, SimpleCallback onError)
    {
        string text = Application.streamingAssetsPath + "/Exported Modules";
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        if (module.adventures != null)
        {
            StreamWriter streamWriter = new StreamWriter(text + "/" + module.name + ".txt");
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < module.adventures.Count; i++)
            {
                Adventure adventure = module.adventures[i];
                stringBuilder.AppendLine("-- [EVENT] --" + adventure.name + "(" + i + ")");
                if (adventure.nodes != null)
                {
                    for (int j = 0; j < adventure.nodes.Count; j++)
                    {
                        BaseNode baseNode = adventure.nodes[j];
                        if (baseNode is NodeStory)
                        {
                            stringBuilder.AppendLine("+[NODE]" + baseNode.ID);
                            stringBuilder.AppendLine("[STORY]");
                            stringBuilder.AppendLine((baseNode as NodeStory).story);
                            stringBuilder.AppendLine("[/STORY]");
                            for (int k = 0; k < baseNode.outputs.Count; k++)
                            {
                                stringBuilder.AppendLine("[OUT]" + baseNode.outputs[k].name);
                            }
                            stringBuilder.AppendLine("[/NODE]");
                            stringBuilder.AppendLine("");
                        }
                    }
                }
                stringBuilder.AppendLine("[/EVENT]");
                stringBuilder.AppendLine("");
            }
            streamWriter.Write(stringBuilder.ToString().ToCharArray());
            streamWriter.Close();
            Globals.SetDynamicData("ModuleSaveID", module.name);
            Globals.SetDynamicData("ModuleSaveDir", text);
            onReady();
        }
        else
        {
            onError();
        }
    }

    public static void ExportSelectedModules(List<Module> modules, SimpleCallback onReady, SimpleCallback onError)
    {
        string text = Application.streamingAssetsPath + "/Exported Modules";
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        if (modules != null)
        {
            List<Module> list = modules.FindAll((Module o) => o.isAllowed);
            Dictionary<long, string> dictionary = new Dictionary<long, string>();
            Vector2i data = default(Vector2i);
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter ms = new BinaryWriter(memoryStream);
            bool exportStory = !Input.GetKey(KeyCode.LeftShift);
            foreach (Module item in list)
            {
                ModulesImport.PartExportDialogsToIndexedDict(item, dictionary, ms, exportStory, ref data);
            }
            float num = 0f;
            if (data.x > 0)
            {
                num = (float)data.y / (float)data.x;
            }
            num *= 100f;
            StreamWriter streamWriter = new StreamWriter(text + "/BundleBlock.txt");
            StringBuilder stringBuilder = new StringBuilder();
            int value = global::UnityEngine.Random.Range(0, int.MaxValue);
            stringBuilder.AppendLine("[ID]" + value);
            foreach (Module item2 in list)
            {
                stringBuilder.AppendLine("IncludedModule: " + item2.name);
            }
            stringBuilder.AppendLine("Cumulative text export contains " + data.x + " characters input and requires translation of " + data.y + " which is " + (int)num + "% of all text");
            stringBuilder.AppendLine("NOTE! Translate only texts after [TXT] elements below");
            stringBuilder.AppendLine("");
            foreach (KeyValuePair<long, string> item3 in dictionary)
            {
                stringBuilder.AppendLine("[ID]" + item3.Key + "[TXT]" + item3.Value);
            }
            streamWriter.Write(stringBuilder.ToString().ToCharArray());
            streamWriter.Close();
            streamWriter = new StreamWriter(text + "/BundleBlockRef.bin");
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
                new BinaryWriter(memoryStream2).Write(value);
                memoryStream2.Position = 0L;
                memoryStream2.WriteTo(streamWriter.BaseStream);
            }
            memoryStream.Position = 0L;
            memoryStream.WriteTo(streamWriter.BaseStream);
            streamWriter.Close();
            memoryStream.Dispose();
            Globals.SetDynamicData("ModuleSaveID", "BundleBlock");
            Globals.SetDynamicData("ModuleSaveDir", text);
            onReady();
            Debug.Log("Cumulative text export contains " + data.x + " characters input and requires translation of " + data.y + " which is " + (int)num + "% of all text");
        }
        else
        {
            onError();
        }
    }

    private static void PartExportDialogsToIndexedDict(Module module, Dictionary<long, string> dict, BinaryWriter ms, bool exportStory, ref Vector2i data)
    {
        if (module.adventures == null)
        {
            return;
        }
        ms.Write((byte)31);
        ms.Write(module.uniqueID);
        for (int i = 0; i < module.adventures.Count; i++)
        {
            Adventure adventure = module.adventures[i];
            ms.Write((byte)32);
            ms.Write(adventure.uniqueID);
            if (adventure.nodes == null)
            {
                continue;
            }
            for (int j = 0; j < adventure.nodes.Count; j++)
            {
                BaseNode baseNode = adventure.nodes[j];
                if (!(baseNode is NodeStory))
                {
                    continue;
                }
                string story = (baseNode as NodeStory).story;
                if (exportStory && !string.IsNullOrEmpty(story))
                {
                    long andRecordHash = ModulesImport.GetAndRecordHash(story, dict, ref data);
                    ms.Write((byte)34);
                    ms.Write(baseNode.ID);
                    ms.Write(andRecordHash);
                }
                for (int k = 0; k < baseNode.outputs.Count; k++)
                {
                    story = baseNode.outputs[k].name;
                    if (!string.IsNullOrEmpty(story))
                    {
                        long andRecordHash2 = ModulesImport.GetAndRecordHash(story, dict, ref data);
                        ms.Write((byte)35);
                        ms.Write(baseNode.ID);
                        ms.Write(andRecordHash2);
                    }
                }
            }
        }
    }

    public static void SingleModeImport(SimpleCallback onFinished, SimpleCallback onError)
    {
        Module selectedModule = EventEditorModules.GetSelectedModule();
        if (selectedModule != null)
        {
            ModulesImport.ImportDialogForModule(selectedModule, onFinished, onError);
        }
    }

    private static void ImportDialogForModule(Module module, SimpleCallback onFinished, SimpleCallback onError)
    {
        string text = Application.streamingAssetsPath + "/Exported Modules";
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        if (!File.Exists(text + "/" + module.name + ".txt"))
        {
            Debug.LogWarning("File " + module.name + ".txt in direction: " + text + " doesn't exists!");
            onError();
            return;
        }
        StreamReader file = new StreamReader(text + "/" + module.name + ".txt");
        int num = ModulesImport.ApplyLocalization(module, file);
        if (num == 0)
        {
            Globals.SetDynamicData("ModuleSaveID", module.name);
            Globals.SetDynamicData("ModuleSaveDir", text);
            onFinished();
        }
        else if (ModulesImport.importDataError != null)
        {
            Globals.SetDynamicData("ErrorID", num.ToString());
            Globals.SetDynamicData("ErrorAtModule", ModulesImport.importDataError.t0);
            Globals.SetDynamicData("ErrorAtEvent", ModulesImport.importDataError.t1.ToString());
            Globals.SetDynamicData("ErrorAtNode", ModulesImport.importDataError.t2.ToString());
            onError();
        }
        EventEditorModules.MarkModuleAsModified(module);
    }

    public static void ImportSingleForModules(List<Module> modules, string directory, string languagePostfix = null)
    {
        string text = Application.streamingAssetsPath + "/Exported Modules";
        if (directory != null)
        {
            text = directory;
        }
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        foreach (Module module in modules)
        {
            string text2 = module.name + languagePostfix + ".txt";
            if (File.Exists(text + "/" + text2))
            {
                StreamReader file = new StreamReader(text + "/" + text2);
                if (ModulesImport.ApplyLocalization(module, file) > 0)
                {
                    Debug.Log("[ERROR]" + text2 + " failed to localize adventures!");
                }
            }
        }
    }

    public static void ImportBlockOfModules(List<Module> modules, SimpleCallback importFinished, SimpleCallback importFailed)
    {
        ModulesImport.ImportBlockOfModules(modules, importFinished, importFailed, null);
    }

    public static void ImportBlockOfModules(List<Module> modules, SimpleCallback importFinished, SimpleCallback importFailed, string directory, string languagePostfix = null)
    {
        string text = Application.streamingAssetsPath + "/Exported Modules";
        if (directory != null)
        {
            text = directory;
        }
        bool flag = false;
        bool flag2 = false;
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        if (languagePostfix == null)
        {
            languagePostfix = "";
        }
        string text2 = "BundleBlock" + languagePostfix + ".txt";
        if (!File.Exists(text + "/" + text2))
        {
            Debug.LogWarning("[ERROR] there is no bundle block (" + text2 + ") to import");
            flag = true;
        }
        string text3 = "BundleBlockRef.bin";
        if (!File.Exists(text + "/" + text3))
        {
            Debug.LogWarning("[ERROR] there is no bundle block references (" + text3 + ") to import");
            flag = true;
        }
        if (flag)
        {
            importFailed();
            return;
        }
        StreamReader streamReader = new StreamReader(text + "/" + text2);
        StreamReader streamReader2 = new StreamReader(text + "/" + text3);
        BinaryReader binaryReader = new BinaryReader(streamReader2.BaseStream);
        string text4 = binaryReader.ReadInt32().ToString();
        string text5 = streamReader.ReadLine().Substring("[ID]".Length);
        if (text4 != text5)
        {
            Debug.LogWarning("[ERROR]identifiers not matching. Translation id: " + text5 + " reference id: " + text4);
            flag = true;
        }
        Dictionary<long, string> dictionary = new Dictionary<long, string>();
        if (!flag)
        {
            try
            {
                bool flag3 = false;
                long num = 0L;
                while (!streamReader.EndOfStream)
                {
                    string text6 = streamReader.ReadLine();
                    if (text6.StartsWith("[ID]"))
                    {
                        int num2 = text6.IndexOf("[TXT]");
                        num = Convert.ToInt64(text6.Substring("[ID]".Length, num2 - "[ID]".Length));
                        string value = text6.Substring(num2 + "[TXT]".Length);
                        flag3 = true;
                        dictionary[num] = value;
                    }
                    else if (flag3)
                    {
                        Dictionary<long, string> dictionary2 = dictionary;
                        long key = num;
                        dictionary2[key] = dictionary2[key] + Environment.NewLine + text6;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[ERROR]" + ex);
                flag = true;
            }
        }
        if (!flag)
        {
            long moduleHashID = 0L;
            int eventUniqueID = 0;
            int nodeUniqueID = 0;
            Module module = null;
            Adventure adventure = null;
            BaseNode baseNode = null;
            int num3 = -1;
            while (!flag && streamReader2.BaseStream.Position < streamReader2.BaseStream.Length)
            {
                long key2 = 0L;
                byte b = binaryReader.ReadByte();
                switch (b)
                {
                case 31:
                    moduleHashID = binaryReader.ReadInt32();
                    continue;
                case 32:
                    eventUniqueID = binaryReader.ReadInt32();
                    continue;
                case 34:
                    nodeUniqueID = binaryReader.ReadInt32();
                    key2 = binaryReader.ReadInt64();
                    break;
                case 35:
                    num3++;
                    nodeUniqueID = binaryReader.ReadInt32();
                    key2 = binaryReader.ReadInt64();
                    break;
                default:
                    Debug.LogWarning("[ERROR]Unidentified ID!! " + b);
                    flag = true;
                    break;
                }
                if (dictionary.ContainsKey(key2))
                {
                    if (module == null || module.uniqueID != moduleHashID)
                    {
                        if (modules == null)
                        {
                            module = null;
                        }
                        else
                        {
                            module = modules.Find((Module o) => o.uniqueID == moduleHashID);
                            if (module != null && languagePostfix == null)
                            {
                                EventEditorModules.MarkModuleAsModified(module);
                            }
                            adventure = null;
                            baseNode = null;
                        }
                    }
                    if (module == null)
                    {
                        Debug.LogWarning("[ERROR]module of the id:" + moduleHashID + " not found!");
                        flag2 = true;
                        continue;
                    }
                    if (adventure == null || adventure.uniqueID != eventUniqueID)
                    {
                        if (module.adventures == null)
                        {
                            adventure = null;
                        }
                        else
                        {
                            adventure = module.adventures.Find((Adventure o) => o.uniqueID == eventUniqueID);
                            baseNode = null;
                        }
                    }
                    if (adventure == null)
                    {
                        Debug.LogWarning("[ERROR]module :" + module.name + " is missing event id: " + eventUniqueID);
                        flag2 = true;
                        continue;
                    }
                    if (baseNode == null || baseNode.ID != nodeUniqueID)
                    {
                        if (adventure.nodes == null)
                        {
                            baseNode = null;
                        }
                        else
                        {
                            baseNode = adventure.nodes.Find((BaseNode o) => o.ID == nodeUniqueID);
                            num3 = -1;
                            if (b == 35)
                            {
                                num3++;
                            }
                        }
                    }
                    if (baseNode == null)
                    {
                        Debug.LogWarning("[ERROR]module :" + module.name + " , event id: " + eventUniqueID + " is missing node id: " + nodeUniqueID);
                        flag2 = true;
                    }
                    else if (num3 == -1)
                    {
                        if (!(baseNode is NodeStory))
                        {
                            Debug.LogWarning("[ERROR]localization for node in module :" + module.name + " ,event id: " + eventUniqueID + " is not and Adventure node id: " + nodeUniqueID);
                            flag2 = true;
                        }
                        else
                        {
                            (baseNode as NodeStory).story = dictionary[key2];
                        }
                    }
                    else if (baseNode.outputs == null || baseNode.outputs.Count <= num3)
                    {
                        Debug.LogWarning("[ERROR]localization for node in module :" + module.name + " ,event id: " + eventUniqueID + " in node id: " + nodeUniqueID + " refers to output " + (num3 + 1) + " but it does not have that many");
                        flag2 = true;
                    }
                    else
                    {
                        baseNode.outputs[num3].name = dictionary[key2];
                    }
                }
                else
                {
                    Debug.LogWarning("[ERROR]localization missing hash: " + key2);
                    flag2 = true;
                }
            }
        }
        if (flag2 || flag)
        {
            importFailed();
        }
        else
        {
            importFinished();
        }
        streamReader.Close();
        streamReader2.Close();
    }

    private static long GetAndRecordHash(string s, Dictionary<long, string> dict, ref Vector2i data)
    {
        long num = s.GetHashCode();
        while (true)
        {
            if (!dict.ContainsKey(num))
            {
                dict[num] = s;
                data.x += s.Length;
                data.y += s.Length;
                return num;
            }
            if (dict[num] == s)
            {
                break;
            }
            num = ((num == 0L) ? 17 : (num << 1));
        }
        data.x += s.Length;
        return num;
    }

    public static int ApplyLocalization(Module module, StreamReader file)
    {
        ModulesImport.importDataError = null;
        int num = 0;
        string text = "";
        int num2 = -1;
        int t = -1;
        if (module.adventures != null)
        {
            text = module.name;
            int num3 = 0;
            while (num == 0 && num3 < module.adventures.Count)
            {
                Adventure adventure = module.adventures[num3];
                num2 = adventure.uniqueID;
                while (num == 0)
                {
                    if (file.EndOfStream)
                    {
                        if (ModulesImport.importDataError == null)
                        {
                            ModulesImport.importDataError = new Multitype<string, int, int>(text, num2, t);
                        }
                        num = 1;
                        break;
                    }
                    if (file.ReadLine().StartsWith("-- [EVENT] --"))
                    {
                        break;
                    }
                }
                bool flag = false;
                while (!flag)
                {
                    if (num != 0)
                    {
                        break;
                    }
                    int expectedNodeID = -1;
                    while (true)
                    {
                        if (file.EndOfStream)
                        {
                            if (ModulesImport.importDataError == null)
                            {
                                ModulesImport.importDataError = new Multitype<string, int, int>(text, num2, t);
                            }
                            num = 3;
                            break;
                        }
                        string text2 = file.ReadLine();
                        if (text2.StartsWith("[/EVENT]"))
                        {
                            flag = true;
                            break;
                        }
                        if (!text2.StartsWith("+[NODE]"))
                        {
                            continue;
                        }
                        string text3 = text2.Substring("+[NODE]".Length);
                        text3 = text3.Replace(" ", "");
                        try
                        {
                            expectedNodeID = Convert.ToInt32(text3);
                        }
                        catch
                        {
                            if (ModulesImport.importDataError == null)
                            {
                                ModulesImport.importDataError = new Multitype<string, int, int>(text, num2, t);
                            }
                            num = 4;
                        }
                        break;
                    }
                    if (flag || num != 0 || adventure.nodes == null)
                    {
                        continue;
                    }
                    bool flag2 = false;
                    bool flag3 = false;
                    BaseNode baseNode = adventure.nodes.Find((BaseNode o) => o.ID == expectedNodeID);
                    if (baseNode == null)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        t = baseNode.ID;
                        if (!(baseNode is NodeStory))
                        {
                            flag2 = true;
                        }
                    }
                    if (num == 0 && flag2)
                    {
                        while (!file.ReadLine().StartsWith("[/NODE]"))
                        {
                        }
                        flag3 = true;
                        continue;
                    }
                    while (num == 0 && !flag3)
                    {
                        if (file.EndOfStream)
                        {
                            if (ModulesImport.importDataError == null)
                            {
                                ModulesImport.importDataError = new Multitype<string, int, int>(text, num2, t);
                            }
                            num = 5;
                            break;
                        }
                        string text4 = file.ReadLine();
                        if (text4.StartsWith("[STORY]"))
                        {
                            break;
                        }
                        if (text4.StartsWith("[/NODE]"))
                        {
                            flag3 = true;
                        }
                    }
                    string text5 = "";
                    while (num == 0 && !flag3)
                    {
                        if (file.EndOfStream)
                        {
                            if (ModulesImport.importDataError == null)
                            {
                                ModulesImport.importDataError = new Multitype<string, int, int>(text, num2, t);
                            }
                            num = 6;
                            break;
                        }
                        string text6 = file.ReadLine();
                        if (text6.StartsWith("[/STORY]"))
                        {
                            break;
                        }
                        if (text6.StartsWith("[/NODE]"))
                        {
                            flag3 = true;
                        }
                        text5 = ((!string.IsNullOrEmpty(text5)) ? (text5 + Environment.NewLine + text6) : text6);
                    }
                    if (num == 0 && !flag3)
                    {
                        (baseNode as NodeStory).story = text5;
                    }
                    int num4 = 0;
                    while (!flag3 && num == 0 && num4 < baseNode.outputs.Count)
                    {
                        while (num == 0 && !flag3)
                        {
                            if (file.EndOfStream)
                            {
                                if (ModulesImport.importDataError == null)
                                {
                                    ModulesImport.importDataError = new Multitype<string, int, int>(text, num2, t);
                                }
                                num = 7;
                                break;
                            }
                            string text7 = file.ReadLine();
                            if (text7.StartsWith("[OUT]"))
                            {
                                string text8 = text7.Substring("[OUT]".Length);
                                baseNode.outputs[num4].name = text8;
                                break;
                            }
                            if (text7.StartsWith("[/NODE]"))
                            {
                                flag3 = true;
                            }
                        }
                        num4++;
                    }
                }
                num3++;
            }
            file.Close();
        }
        return num;
    }
}
