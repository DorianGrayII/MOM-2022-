// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.DataBase
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using DBDef;
using MHUtils;
using UnityEngine;

public class DataBase
{
    private static readonly string TAB = "\t";

    private static CultureInfo floatingPointCultureInfo;

    public static DataBase instance;

    private Dictionary<string, DBClass> dbContent;

    private Dictionary<Type, Dictionary<string, DBClass>> dbContentByType;

    private Dictionary<Type, object> dbType;

    private Dictionary<Type, object> dbTypeAsDBClass;

    private Dictionary<string, string> deAbbreviation;

    private Dictionary<DBClass, ModOrder> instanceToMod;

    private string databaseCurentlyLoading;

    private bool databaseLoaded;

    private int hash;

    private bool customDLC;

    public static bool IsInitialized()
    {
        if (DataBase.instance != null)
        {
            return DataBase.instance.databaseLoaded;
        }
        return false;
    }

    public static void InitializeDB()
    {
        DataBase.instance = new DataBase();
        DataBase.instance.LoadDatabaseContent();
    }

    public static DataBase GetDB()
    {
        return DataBase.instance;
    }

    public static void Destroy()
    {
        DataBase.instance = null;
    }

    private static Dictionary<string, PrototypeData> GetPrototypes()
    {
        string[] prototypeFilePaths = DataBase.GetPrototypeFilePaths();
        Dictionary<string, PrototypeData> dictionary = new Dictionary<string, PrototypeData>();
        string[] array = prototypeFilePaths;
        foreach (string filename in array)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            {
                IEnumerator enumerator = xmlDocument.ChildNodes.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext() && enumerator.Current is XmlElement xmlElement)
                    {
                        string comment = null;
                        foreach (object childNode in xmlElement.ChildNodes)
                        {
                            if (childNode is XmlComment)
                            {
                                comment = (childNode as XmlComment).Value;
                                continue;
                            }
                            if (childNode is XmlElement)
                            {
                                XmlElement xmlElement2 = childNode as XmlElement;
                                PrototypeData value = default(PrototypeData);
                                value.abbreviation = xmlElement2.GetAttribute("Abbreviation");
                                value.comment = comment;
                                value.data = xmlElement2;
                                value.dbName = xmlElement2.Name;
                                value.name = xmlElement2.GetAttribute("Name");
                                dictionary[xmlElement2.Name] = value;
                            }
                            comment = null;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
        return dictionary;
    }

    private static Dictionary<string, string> GetAbbreviations(Dictionary<string, PrototypeData> source)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (KeyValuePair<string, PrototypeData> item in source)
        {
            if (!string.IsNullOrEmpty(item.Value.abbreviation))
            {
                dictionary[item.Value.abbreviation] = item.Key;
            }
        }
        return dictionary;
    }

    private static string[] GetPrototypeFilePaths()
    {
        return Directory.GetFiles(Path.Combine(Path.Combine(MHApplication.EXTERNAL_ASSETS, "Database"), "Prototype"));
    }

    public static void WritePrototypes(string savePath)
    {
        Dictionary<string, PrototypeData> prototypes = DataBase.GetPrototypes();
        StringWriter stringWriter = new StringWriter();
        StringWriter stringWriter2 = new StringWriter();
        foreach (KeyValuePair<string, PrototypeData> item in prototypes)
        {
            PrototypeData value = item.Value;
            XmlElement data = value.data;
            string attribute = data.GetAttribute("Type");
            string attribute2 = data.GetAttribute("Partial");
            if (attribute == "enum")
            {
                DataBase.WriteSingleEnumPrototype(value.data, value.comment, stringWriter);
            }
            else
            {
                DataBase.WriteSinglePrototype(value.data, value.comment, prototypes, stringWriter2, attribute2.ToUpperInvariant() == "TRUE");
            }
        }
        if (savePath != null)
        {
            string text = Path.Combine(savePath, "DBDefinitions.cs");
            StreamWriter streamWriter = new StreamWriter(Application.dataPath + text);
            streamWriter.WriteLine("// !!! This is procedurally created class");
            streamWriter.WriteLine("// all changes will be lost with next export!");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("using System.Collections.Generic;");
            streamWriter.WriteLine("using MHUtils;");
            streamWriter.WriteLine("using UnityEngine;");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("namespace DBDef");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("#region ENUM ");
            streamWriter.Write(stringWriter.ToString());
            streamWriter.WriteLine("#endregion ENUM");
            streamWriter.WriteLine("#region CLASS DEFINITIONS");
            streamWriter.Write(stringWriter2.ToString());
            streamWriter.WriteLine("#endregion CLASS DEFINITIONS");
            streamWriter.WriteLine("}");
            streamWriter.Close();
        }
    }

    private static void WriteSingleEnumPrototype(XmlElement source, string commentStore, StringWriter writer)
    {
        DataBase.WriteComments(commentStore, writer);
        writer.WriteLine(DataBase.TAB + "public enum " + source.GetAttribute("Name"));
        writer.WriteLine(DataBase.TAB + "{");
        foreach (object childNode in source.ChildNodes)
        {
            if (childNode is XmlComment)
            {
                XmlComment xmlComment = childNode as XmlComment;
                writer.WriteLine(DataBase.TAB + DataBase.TAB + "//" + xmlComment.Value);
            }
            else if (childNode is XmlElement)
            {
                XmlElement xmlElement = childNode as XmlElement;
                if (xmlElement.HasAttribute("value"))
                {
                    writer.WriteLine(DataBase.TAB + DataBase.TAB + xmlElement.Name + "=" + xmlElement.GetAttribute("value") + ",");
                }
                else
                {
                    writer.WriteLine(DataBase.TAB + DataBase.TAB + xmlElement.Name + ",");
                }
            }
        }
        writer.WriteLine(DataBase.TAB + "}");
    }

    private static void WriteSinglePrototype(XmlElement source, string commentStore, Dictionary<string, PrototypeData> typesAvaliable, StringWriter writer, bool partial = false)
    {
        DataBase.WriteComments(commentStore, writer);
        StringBuilder stringBuilder = new StringBuilder();
        string text = "";
        stringBuilder.AppendLine(DataBase.TAB + DataBase.TAB + "public " + source.GetAttribute("Name") + "(){}");
        foreach (object childNode in source.ChildNodes)
        {
            if (childNode is XmlElement xmlElement && xmlElement.GetAttribute("Type") == "DESCRIPTION_INFO")
            {
                text = ", IDescriptionInfoType";
                stringBuilder.AppendLine(DataBase.TAB + DataBase.TAB + "public DescriptionInfo GetDescriptionInfo()");
                stringBuilder.AppendLine(DataBase.TAB + DataBase.TAB + "{");
                stringBuilder.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "return " + xmlElement.GetAttribute("Name") + ";");
                stringBuilder.AppendLine(DataBase.TAB + DataBase.TAB + "}");
                break;
            }
        }
        DataBase.WriteExplicitCastMethod(source.GetAttribute("Name"), stringBuilder);
        string attribute = source.GetAttribute("Abbreviation");
        string text2 = source.GetAttribute("Extends");
        if (string.IsNullOrEmpty(text2))
        {
            text2 = "DBClass";
        }
        writer.WriteLine("{0}[ClassPrototype(\"{1}\", \"{2}\")]", DataBase.TAB, source.Name, attribute);
        writer.WriteLine(DataBase.TAB + "public " + (partial ? "partial " : "") + "class " + source.GetAttribute("Name") + " : " + text2 + text);
        writer.WriteLine(DataBase.TAB + "{");
        writer.WriteLine(DataBase.TAB + DataBase.TAB + "static public string abbreviation = \"" + attribute + "\";");
        commentStore = null;
        foreach (object childNode2 in source.ChildNodes)
        {
            if (childNode2 is XmlComment)
            {
                commentStore = (childNode2 as XmlComment).Value;
                continue;
            }
            if (childNode2 is XmlElement)
            {
                XmlElement xmlElement2 = childNode2 as XmlElement;
                string text3 = xmlElement2.GetAttribute("Type");
                string attribute2 = xmlElement2.GetAttribute("Name");
                string attribute3 = xmlElement2.GetAttribute("Required");
                string attribute4 = xmlElement2.GetAttribute("Private");
                if (attribute2 == null || text3 == null || attribute3 == null)
                {
                    Debug.LogError("[ERROR] Variable " + xmlElement2.Name + " have been skipped due to missing one of the three required parameters: Name, Type, Required");
                    continue;
                }
                writer.WriteLine("{0}{0}[Prototype(\"{1}\", {2})]", DataBase.TAB, xmlElement2.Name, attribute3.ToLowerInvariant());
                bool flag = text3.StartsWith("Array");
                if (flag)
                {
                    text3 = text3.Substring(5);
                }
                string text4 = null;
                bool flag2 = false;
                switch (text3)
                {
                case "float":
                case "int":
                case "bool":
                    text4 = text3;
                    flag2 = true;
                    break;
                case "FInt":
                    text4 = text3;
                    flag2 = true;
                    break;
                case "Color":
                    text4 = text3;
                    flag2 = true;
                    break;
                case "string":
                    text4 = text3;
                    break;
                default:
                    if (text3.StartsWith("E_"))
                    {
                        flag2 = true;
                    }
                    if (typesAvaliable.ContainsKey(text3))
                    {
                        text4 = typesAvaliable[text3].name;
                    }
                    break;
                }
                if (text4 == null)
                {
                    Debug.LogWarning("[DB]Type " + text3 + " Have not been found! variable would be skipped!");
                    continue;
                }
                string text5 = "public";
                if (attribute4.ToUpperInvariant() == "TRUE")
                {
                    text5 = "private";
                }
                if (flag)
                {
                    writer.WriteLine("{0}{0}{1} {2}[] {3};", DataBase.TAB, text5, text4, xmlElement2.GetAttribute("Name"));
                    if (flag2)
                    {
                        DataBase.WriteStructArrayMethod(attribute2, text4, stringBuilder);
                    }
                    else
                    {
                        DataBase.WriteObjectArrayMethod(attribute2, text4, stringBuilder);
                    }
                }
                else
                {
                    writer.WriteLine("{0}{0}{1} {2} {3};", DataBase.TAB, text5, text4, xmlElement2.GetAttribute("Name"));
                }
            }
            commentStore = null;
        }
        if (stringBuilder.Length > 0)
        {
            writer.WriteLine(DataBase.TAB + DataBase.TAB + "#region METHODS");
            writer.Write(stringBuilder.ToString());
            writer.WriteLine(DataBase.TAB + DataBase.TAB + "#endregion METHODS");
        }
        writer.WriteLine(DataBase.TAB + "}");
    }

    private static void WriteComments(string source, StringWriter writer)
    {
        if (source != null)
        {
            writer.WriteLine(DataBase.TAB + "/// <summary>");
            string[] array = source.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string text in array)
            {
                writer.WriteLine(DataBase.TAB + "/// " + text);
            }
            writer.WriteLine(DataBase.TAB + "/// <summary>");
        }
    }

    private static void WriteObjectArrayMethod(string variable, string typeName, StringBuilder sb)
    {
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "public void Set_" + variable + "(List<object> list)");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "{");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "if (list == null || list.Count == 0) return; ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + variable + " = new " + typeName + "[list.Count]; ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "for(int i=0; i < list.Count; i++) ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "{");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + DataBase.TAB + "if(!(list[i] is " + typeName + "))Debug.LogError(\"" + variable + " of type " + typeName + " received invalid type from array! \"+list[i]);");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + DataBase.TAB + variable + "[i] = list[i] as " + typeName + "; ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "}");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "}");
    }

    private static void WriteStructArrayMethod(string variable, string typeName, StringBuilder sb)
    {
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "public void Set_" + variable + "(List<object> list)");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "{");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "if (list == null || list.Count == 0) return; ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + variable + " = new " + typeName + "[list.Count]; ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "for(int i=0; i < list.Count; i++) ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "{");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + DataBase.TAB + variable + "[i] = (" + typeName + ")list[i]; ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "}");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "}");
    }

    private static void WriteExplicitCastMethod(string typeName, StringBuilder sb)
    {
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "public static explicit operator " + typeName + "(System.Enum e)");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "{");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "return DataBase.Get<" + typeName + ">(e); ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "}");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "public static explicit operator " + typeName + "(string e)");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "{");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + "return DataBase.Get<" + typeName + ">(e, true); ");
        sb.AppendLine(DataBase.TAB + DataBase.TAB + "}");
    }

    public static void WriteHelpers(string savePath)
    {
        Dictionary<string, PrototypeData> prototypes = DataBase.GetPrototypes();
        Dictionary<string, string> abbreviations = DataBase.GetAbbreviations(prototypes);
        Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
        string text = null;
        string text2 = null;
        foreach (string databaseFilePath in DataBase.GetDatabaseFilePaths())
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(databaseFilePath);
            {
                IEnumerator enumerator2 = xmlDocument.ChildNodes.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext() && enumerator2.Current is XmlElement xmlElement)
                    {
                        foreach (object childNode in xmlElement.ChildNodes)
                        {
                            if (!(childNode is XmlElement))
                            {
                                continue;
                            }
                            string name = (childNode as XmlElement).Name;
                            int num = name.IndexOf("-");
                            if (num < 0)
                            {
                                Debug.LogWarning("Database contains unknown type: " + name);
                                continue;
                            }
                            text = name.Substring(0, num);
                            if (!prototypes.ContainsKey(text) && !abbreviations.ContainsKey(text))
                            {
                                Debug.LogWarning("Use of " + text + " inside " + xmlElement.Name + " impossible, type not known");
                                continue;
                            }
                            if (!dictionary.ContainsKey(text))
                            {
                                dictionary[text] = new List<string>();
                            }
                            text2 = name.Substring(num + 1);
                            if (!dictionary[text].Contains(text2))
                            {
                                dictionary[text].Add(text2);
                            }
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator2 as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
        StringWriter stringWriter = new StringWriter();
        foreach (KeyValuePair<string, List<string>> item in dictionary)
        {
            stringWriter.WriteLine(DataBase.TAB + DataBase.TAB + "public enum " + item.Key);
            stringWriter.WriteLine(DataBase.TAB + DataBase.TAB + "{");
            foreach (string item2 in item.Value)
            {
                stringWriter.WriteLine(DataBase.TAB + DataBase.TAB + DataBase.TAB + item2 + ",");
            }
            stringWriter.WriteLine(DataBase.TAB + DataBase.TAB + "}");
        }
        if (savePath != null)
        {
            string text3 = Path.Combine(savePath, "DBHelpers.cs");
            StreamWriter streamWriter = new StreamWriter(Application.dataPath + text3);
            streamWriter.WriteLine("// !!! This is procedurally created class");
            streamWriter.WriteLine("// all changes will be lost with next export!");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("using System.Collections.Generic;");
            streamWriter.WriteLine("using MHUtils;");
            streamWriter.WriteLine("");
            streamWriter.WriteLine("namespace DBEnum");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("#region ENUM ");
            streamWriter.Write(stringWriter.ToString());
            streamWriter.WriteLine("#endregion ENUM");
            streamWriter.WriteLine("}");
            streamWriter.Close();
        }
    }

    private static List<string> GetDatabaseFilePaths()
    {
        List<string> list = new List<string>(Directory.GetFiles(Path.Combine(MHApplication.EXTERNAL_ASSETS, "Database"))).FindAll((string o) => o.EndsWith("xml"));
        foreach (ModOrder activeValidMod in ModManager.Get().GetActiveValidMods())
        {
            string path = activeValidMod.GetPath();
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Path for mod " + activeValidMod.name + " is missing!");
                continue;
            }
            string path2 = Path.Combine(path, "Database");
            if (!Directory.Exists(path2))
            {
                continue;
            }
            string[] files = Directory.GetFiles(path2);
            foreach (string text in files)
            {
                if (text.EndsWith(".xml"))
                {
                    list.Add(text);
                }
            }
        }
        return list;
    }

    public void LoadDatabaseContent()
    {
        List<string> databaseFilePaths = DataBase.GetDatabaseFilePaths();
        Dictionary<string, DatabaseData> dictionary = new Dictionary<string, DatabaseData>();
        Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
        foreach (string item in databaseFilePaths)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(item);
            }
            catch (Exception ex)
            {
                Debug.LogError("Loading " + item + " failed, xml file is not readable \n" + ex.Message);
                continue;
            }
            this.databaseCurentlyLoading = item;
            {
                IEnumerator enumerator2 = xmlDocument.ChildNodes.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext() && enumerator2.Current is XmlElement xmlElement)
                    {
                        foreach (object childNode in xmlElement.ChildNodes)
                        {
                            if (childNode is XmlElement)
                            {
                                XmlElement xmlElement2 = childNode as XmlElement;
                                DatabaseData value = default(DatabaseData);
                                value.data = xmlElement2;
                                value.dbName = xmlElement2.Name;
                                dictionary[xmlElement2.Name] = value;
                                dictionary2[xmlElement2.Name] = item;
                            }
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator2 as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
        Type[] types = Assembly.GetAssembly(typeof(DBClass)).GetTypes();
        Dictionary<string, Type> dictionary3 = new Dictionary<string, Type>();
        Type[] array = types;
        foreach (Type type in array)
        {
            ClassPrototype customAttribute = type.GetCustomAttribute<ClassPrototype>();
            if (customAttribute != null)
            {
                if (!string.IsNullOrEmpty(customAttribute.dbName))
                {
                    dictionary3[customAttribute.dbName] = type;
                }
                if (!string.IsNullOrEmpty(customAttribute.dbAbbreviation))
                {
                    dictionary3[customAttribute.dbAbbreviation] = type;
                }
            }
        }
        Dictionary<string, DBClass> dictionary4 = new Dictionary<string, DBClass>();
        foreach (KeyValuePair<string, DatabaseData> item2 in dictionary)
        {
            object obj = this.CreateEmptyInstance(item2.Value.dbName, dictionary3);
            if (obj == null)
            {
                continue;
            }
            if (!(obj is DBClass))
            {
                Debug.LogError("instance " + item2.Value.dbName + " in DB is not DBClass");
                continue;
            }
            if (dictionary4.ContainsKey(item2.Value.dbName))
            {
                Debug.LogWarning("instance " + item2.Value.dbName + " overrides previous instance in DB");
            }
            dictionary4[item2.Value.dbName] = obj as DBClass;
            dictionary4[item2.Value.dbName].dbName = item2.Value.dbName;
        }
        foreach (KeyValuePair<string, string> item3 in dictionary2)
        {
            if (this.instanceToMod == null)
            {
                this.instanceToMod = new Dictionary<DBClass, ModOrder>();
            }
            DBClass key = dictionary4[item3.Key];
            this.instanceToMod[key] = ModManager.Get().GetModOrderByPath(item3.Value);
        }
        foreach (KeyValuePair<string, DatabaseData> item4 in dictionary)
        {
            if (dictionary4.ContainsKey(item4.Key))
            {
                this.databaseCurentlyLoading = item4.Key;
                DBClass dBClass = dictionary4[item4.Key];
                this.FillInstance(dBClass, item4.Value.data, dictionary4);
            }
        }
        Debug.Log("Loaded database consist of " + dictionary4.Count + " objects from " + databaseFilePaths.Count + " files");
        this.dbContent = dictionary4;
        this.FinalizeLoadDatabase(-1, loadAllDLC: true);
    }

    private object CreateEmptyInstance(string name, Dictionary<string, Type> nameToType)
    {
        string[] array = name.Split("-".ToCharArray());
        if (array.Length < 2)
        {
            Debug.LogError("Invalid type in database: " + name);
            return null;
        }
        if (nameToType.ContainsKey(array[0]))
        {
            return Activator.CreateInstance(nameToType[array[0]]);
        }
        Debug.LogError("Invalid type in database: " + name + ", unknown type");
        return null;
    }

    private void FillInstance(object instance, XmlElement e, Dictionary<string, DBClass> database)
    {
        FieldInfo[] fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo[] methods = instance.GetType().GetMethods();
        if (fields == null)
        {
            return;
        }
        FieldInfo[] array = fields;
        foreach (FieldInfo fieldInfo in array)
        {
            Prototype customAttribute = fieldInfo.GetCustomAttribute<Prototype>();
            if (customAttribute == null)
            {
                continue;
            }
            bool flag = false;
            foreach (object attribute2 in e.Attributes)
            {
                XmlAttribute xmlAttribute = attribute2 as XmlAttribute;
                if (xmlAttribute.Name == customAttribute.name)
                {
                    Type fieldType = fieldInfo.FieldType;
                    object value = null;
                    try
                    {
                        value = DataBase.ConvertTo(fieldType, xmlAttribute.Value, database);
                    }
                    catch
                    {
                        Debug.LogWarning("In type: " + this.databaseCurentlyLoading + " [DB]Cannot convert " + xmlAttribute.Name + " to " + fieldType?.ToString() + " for " + e.Name);
                    }
                    fieldInfo.SetValue(instance, value);
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                continue;
            }
            List<object> list = null;
            if (fieldInfo.FieldType.IsArray)
            {
                list = new List<object>();
            }
            foreach (object childNode in e.ChildNodes)
            {
                if (!(childNode is XmlElement xmlElement) || !(xmlElement.Name == customAttribute.name))
                {
                    continue;
                }
                Type type = fieldInfo.FieldType;
                if (list != null)
                {
                    type = fieldInfo.FieldType.GetElementType();
                }
                string attribute;
                bool flag2;
                if (xmlElement.HasAttribute("VALUE"))
                {
                    attribute = xmlElement.GetAttribute("VALUE");
                    flag2 = false;
                    if (typeof(string) == type)
                    {
                        if (list == null)
                        {
                            fieldInfo.SetValue(instance, attribute);
                            flag = true;
                            break;
                        }
                        list.Add(attribute);
                        flag2 = true;
                    }
                    else
                    {
                        if (type.IsEnum || type.IsPrimitive || type == typeof(FInt))
                        {
                            try
                            {
                                object obj2 = DataBase.ConvertTo(type, attribute, database);
                                if (list != null)
                                {
                                    list.Add(obj2);
                                    flag2 = true;
                                    goto IL_02fc;
                                }
                                fieldInfo.SetValue(instance, obj2);
                                flag = true;
                            }
                            catch
                            {
                                Debug.LogWarning("In type: " + this.databaseCurentlyLoading + " [DB]Cannot convert " + xmlElement.GetAttribute("VALUE") + " to " + type?.ToString() + " for " + e.Name);
                                goto IL_02fc;
                            }
                            break;
                        }
                        if (database.ContainsKey(attribute))
                        {
                            object obj4 = database[attribute];
                            if (list == null)
                            {
                                fieldInfo.SetValue(instance, obj4);
                                flag = true;
                                break;
                            }
                            list.Add(obj4);
                            flag2 = true;
                        }
                    }
                    goto IL_02fc;
                }
                try
                {
                    if (list != null)
                    {
                        object item = Activator.CreateInstance(fieldInfo.FieldType.GetElementType());
                        this.FillInstance(item, xmlElement, database);
                        list.Add(item);
                        continue;
                    }
                    object value2 = Activator.CreateInstance(type);
                    this.FillInstance(value2, xmlElement, database);
                    fieldInfo.SetValue(instance, value2);
                    flag = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Field " + fieldInfo.Name + ", " + childNode.ToString() + "\n" + ex);
                    continue;
                }
                break;
                IL_02fc:
                if (list == null)
                {
                    Debug.LogError("In file: " + this.databaseCurentlyLoading + " Type " + e.Name + " refers to " + attribute + " of type " + fieldInfo.FieldType?.ToString() + " but one couldn't be found in database");
                    break;
                }
                if (!flag2)
                {
                    Debug.LogError("In file: " + this.databaseCurentlyLoading + " Type " + e.Name + " refers to " + attribute + " of type " + fieldInfo.FieldType?.ToString() + " but one couldn't be found in database");
                }
            }
            if (list != null && methods != null && list.Count > 0)
            {
                string text = "Set_" + fieldInfo.Name;
                bool flag3 = false;
                MethodInfo[] array2 = methods;
                foreach (MethodInfo methodInfo in array2)
                {
                    if (methodInfo.Name == text)
                    {
                        object[] parameters = new object[1] { list };
                        methodInfo.Invoke(instance, parameters);
                        flag3 = true;
                    }
                }
                if (!flag3)
                {
                    Debug.LogError("In file: " + this.databaseCurentlyLoading + " Array filling method not found for " + fieldInfo.Name + " in " + instance.GetType());
                }
            }
            else if (!flag && customAttribute.required)
            {
                Debug.LogError("In file: " + this.databaseCurentlyLoading + " Required field " + customAttribute.name + " in object " + e.Name + " of type " + instance.GetType()?.ToString() + " does not have specified data in database!");
            }
        }
        if (e.Attributes != null && fields != null)
        {
            foreach (object attribute3 in e.Attributes)
            {
                if (!(attribute3 is XmlAttribute xmlAttribute2))
                {
                    continue;
                }
                bool flag4 = false;
                array = fields;
                for (int i = 0; i < array.Length; i++)
                {
                    Prototype customAttribute2 = array[i].GetCustomAttribute<Prototype>();
                    if (customAttribute2 != null)
                    {
                        flag4 = customAttribute2.name == xmlAttribute2.Name;
                        if (flag4)
                        {
                            break;
                        }
                    }
                }
                if (!flag4)
                {
                    Debug.LogWarning("Field Attribute " + xmlAttribute2.Name + " in the " + e.Name + " is filled, but prototype does not recognize it!");
                }
            }
        }
        if (e.ChildNodes == null || fields == null)
        {
            return;
        }
        foreach (object childNode2 in e.ChildNodes)
        {
            if (!(childNode2 is XmlElement xmlElement2))
            {
                continue;
            }
            bool flag5 = false;
            array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                Prototype customAttribute3 = array[i].GetCustomAttribute<Prototype>();
                if (customAttribute3 != null)
                {
                    flag5 = customAttribute3.name == xmlElement2.Name;
                    if (flag5)
                    {
                        break;
                    }
                }
            }
            if (!flag5)
            {
                Debug.LogWarning("Field Element " + xmlElement2.Name + " in the " + e.Name + " is filled, but prototype does not recognize it!");
            }
        }
    }

    private static object ConvertTo(Type t, string content, Dictionary<string, DBClass> database)
    {
        if (typeof(FInt) == t)
        {
            return new FInt(Convert.ToSingle(content, DataBase.FPCultureInfo()));
        }
        if (typeof(string) == t)
        {
            return content;
        }
        if (typeof(int) == t)
        {
            return Convert.ToInt32(content);
        }
        if (typeof(float) == t)
        {
            return Convert.ToSingle(content, DataBase.FPCultureInfo());
        }
        if (typeof(bool) == t)
        {
            if (content == "TRUE")
            {
                return true;
            }
            if (content == "FALSE")
            {
                return false;
            }
            return Convert.ToBoolean(content);
        }
        if (typeof(Color) == t && !string.IsNullOrEmpty(content))
        {
            int num = Convert.ToInt32(content, 16);
            if (content.Length == 8)
            {
                byte b = (byte)((uint)(num >> 24) & 0xFFu);
                byte b2 = (byte)((uint)(num >> 16) & 0xFFu);
                byte b3 = (byte)((uint)(num >> 8) & 0xFFu);
                byte b4 = (byte)((uint)num & 0xFFu);
                return new Color((float)(int)b / 255f, (float)(int)b2 / 255f, (float)(int)b3 / 255f, (float)(int)b4 / 255f);
            }
            if (content.Length == 6)
            {
                byte b5 = (byte)((uint)(num >> 16) & 0xFFu);
                byte b6 = (byte)((uint)(num >> 8) & 0xFFu);
                byte b7 = (byte)((uint)num & 0xFFu);
                return new Color((float)(int)b5 / 255f, (float)(int)b6 / 255f, (float)(int)b7 / 255f, 1f);
            }
            Debug.LogError("Color have to be defined as 6 characters (for no alpha) or 8 characters (with alpha) ie 2233AA or 2233AAFF");
        }
        if (t.IsEnum)
        {
            return Enum.Parse(t, content);
        }
        if (t.IsSubclassOf(typeof(DBClass)))
        {
            if (database.ContainsKey(content))
            {
                return database[content];
            }
            Debug.LogWarning("Missing " + content + " in database when trying to reference it. ");
        }
        return null;
    }

    public static CultureInfo FPCultureInfo()
    {
        if (DataBase.floatingPointCultureInfo == null)
        {
            DataBase.floatingPointCultureInfo = new CultureInfo("en-US");
        }
        return DataBase.floatingPointCultureInfo;
    }

    private void FinalizeLoadDatabase(int overrideDLC = -1, bool loadAllDLC = false)
    {
        if (this.dbContent == null)
        {
            Debug.LogError("Database is not ready for finalization!");
            return;
        }
        int num = ((overrideDLC != -1) ? overrideDLC : PlayerPrefs.GetInt("UseDLC", 0));
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        DLCManager.SetActiveDLC(num);
        foreach (DLCManager.DLCs value in Enum.GetValues(typeof(DLCManager.DLCs)))
        {
            dictionary[value.ToString()] = ((uint)num & (uint)value) != 0;
        }
        this.dbContentByType = new Dictionary<Type, Dictionary<string, DBClass>>();
        foreach (KeyValuePair<string, DBClass> item in this.dbContent)
        {
            Type type = item.Value.GetType();
            if (!this.dbContentByType.ContainsKey(type))
            {
                this.dbContentByType[type] = new Dictionary<string, DBClass>();
            }
            if (!loadAllDLC)
            {
                bool flag = false;
                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo fieldInfo in fields)
                {
                    if (fieldInfo.Name == "dlc")
                    {
                        string text = fieldInfo.GetValue(item.Value) as string;
                        if (!string.IsNullOrEmpty(text) && (!dictionary.ContainsKey(text) || !dictionary[text]))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    continue;
                }
            }
            this.dbContentByType[type][item.Key] = item.Value;
        }
        this.databaseLoaded = true;
    }

    public static ModOrder GetModOrder(DBClass dbClass)
    {
        if (DataBase.GetDB().instanceToMod != null && DataBase.GetDB().instanceToMod.ContainsKey(dbClass))
        {
            return DataBase.GetDB().instanceToMod[dbClass];
        }
        return null;
    }

    public static List<T> GetType<T>() where T : DBClass
    {
        Type typeFromHandle = typeof(T);
        if (DataBase.GetDB().dbType == null)
        {
            DataBase.GetDB().dbType = new Dictionary<Type, object>();
        }
        if (!DataBase.GetDB().dbType.ContainsKey(typeFromHandle))
        {
            List<T> list = new List<T>();
            DataBase.GetDB().dbType[typeFromHandle] = list;
            if (DataBase.GetDB().dbContentByType.ContainsKey(typeFromHandle))
            {
                foreach (KeyValuePair<string, DBClass> item in DataBase.GetDB().dbContentByType[typeFromHandle])
                {
                    list.Add(item.Value as T);
                }
            }
        }
        return DataBase.GetDB().dbType[typeFromHandle] as List<T>;
    }

    public static List<DBClass> GetType(Type t)
    {
        if (DataBase.GetDB().dbTypeAsDBClass == null)
        {
            DataBase.GetDB().dbTypeAsDBClass = new Dictionary<Type, object>();
        }
        if (!DataBase.GetDB().dbTypeAsDBClass.ContainsKey(t))
        {
            List<DBClass> list = new List<DBClass>();
            DataBase.GetDB().dbTypeAsDBClass[t] = list;
            if (DataBase.GetDB().dbContentByType.ContainsKey(t))
            {
                foreach (KeyValuePair<string, DBClass> item in DataBase.GetDB().dbContentByType[t])
                {
                    list.Add(item.Value);
                }
            }
        }
        return DataBase.GetDB().dbTypeAsDBClass[t] as List<DBClass>;
    }

    public static DBClass Get(string instanceName, bool reportMissing)
    {
        if (DataBase.GetDB() != null && DataBase.GetDB().dbContent != null && DataBase.GetDB().dbContent.ContainsKey(instanceName))
        {
            return DataBase.GetDB().dbContent[instanceName];
        }
        if (reportMissing)
        {
            Debug.LogError("instance " + instanceName + " is missing in database!");
        }
        return null;
    }

    public static T Get<T>(Enum e, bool reportMissing = false) where T : DBClass
    {
        string text = e.GetType().ToString();
        string[] array = text.Split('.', StringSplitOptions.None);
        if (array != null && array.Length != 0)
        {
            text = array[array.Length - 1] + "-" + e;
        }
        return DataBase.Get(text, reportMissing) as T;
    }

    public static T Get<T>(string instanceName, bool reportMissing) where T : DBClass
    {
        return DataBase.Get(instanceName, reportMissing) as T;
    }

    public static int GetDBHash()
    {
        int num = DataBase.GetDB()?.hash ?? 0;
        if (num == 0)
        {
            MHTimer mHTimer = MHTimer.StartNew();
            foreach (string databaseFilePath in DataBase.GetDatabaseFilePaths())
            {
                if (File.Exists(databaseFilePath))
                {
                    string text = File.ReadAllText(databaseFilePath);
                    num ^= text.GetHashCode();
                }
            }
            Debug.Log("DB hash " + num + " took " + mHTimer.GetTime());
            if (DataBase.GetDB() != null)
            {
                DataBase.GetDB().hash = num;
            }
        }
        return num;
    }

    public static void UpdateUse(int overrideDLC = -1, bool forced = false)
    {
        if (DataBase.GetDB() != null && (forced || DataBase.GetDB().customDLC || overrideDLC != -1))
        {
            DataBase.GetDB().FinalizeLoadDatabase(overrideDLC);
            DataBase.GetDB().dbType = null;
            DataBase.GetDB().dbTypeAsDBClass = null;
        }
    }
}
