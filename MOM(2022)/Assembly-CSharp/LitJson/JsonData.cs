using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
    public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
    {
        private IList<JsonData> inst_array;

        private bool inst_boolean;

        private double inst_double;

        private int inst_int;

        private long inst_long;

        private IDictionary<string, JsonData> inst_object;

        private string inst_string;

        private string json;

        private JsonType type;

        private IList<KeyValuePair<string, JsonData>> object_list;

        public int Count => this.EnsureCollection().Count;

        public bool IsArray => this.type == JsonType.Array;

        public bool IsBoolean => this.type == JsonType.Boolean;

        public bool IsDouble => this.type == JsonType.Double;

        public bool IsInt => this.type == JsonType.Int;

        public bool IsLong => this.type == JsonType.Long;

        public bool IsObject => this.type == JsonType.Object;

        public bool IsString => this.type == JsonType.String;

        public ICollection<string> Keys
        {
            get
            {
                this.EnsureDictionary();
                return this.inst_object.Keys;
            }
        }

        int ICollection.Count => this.Count;

        bool ICollection.IsSynchronized => this.EnsureCollection().IsSynchronized;

        object ICollection.SyncRoot => this.EnsureCollection().SyncRoot;

        bool IDictionary.IsFixedSize => this.EnsureDictionary().IsFixedSize;

        bool IDictionary.IsReadOnly => this.EnsureDictionary().IsReadOnly;

        ICollection IDictionary.Keys
        {
            get
            {
                this.EnsureDictionary();
                IList<string> list = new List<string>();
                foreach (KeyValuePair<string, JsonData> item in this.object_list)
                {
                    list.Add(item.Key);
                }
                return (ICollection)list;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                this.EnsureDictionary();
                IList<JsonData> list = new List<JsonData>();
                foreach (KeyValuePair<string, JsonData> item in this.object_list)
                {
                    list.Add(item.Value);
                }
                return (ICollection)list;
            }
        }

        bool IJsonWrapper.IsArray => this.IsArray;

        bool IJsonWrapper.IsBoolean => this.IsBoolean;

        bool IJsonWrapper.IsDouble => this.IsDouble;

        bool IJsonWrapper.IsInt => this.IsInt;

        bool IJsonWrapper.IsLong => this.IsLong;

        bool IJsonWrapper.IsObject => this.IsObject;

        bool IJsonWrapper.IsString => this.IsString;

        bool IList.IsFixedSize => this.EnsureList().IsFixedSize;

        bool IList.IsReadOnly => this.EnsureList().IsReadOnly;

        object IDictionary.this[object key]
        {
            get
            {
                return this.EnsureDictionary()[key];
            }
            set
            {
                if (!(key is string))
                {
                    throw new ArgumentException("The key has to be a string");
                }
                JsonData value2 = this.ToJsonData(value);
                this[(string)key] = value2;
            }
        }

        object IOrderedDictionary.this[int idx]
        {
            get
            {
                this.EnsureDictionary();
                return this.object_list[idx].Value;
            }
            set
            {
                this.EnsureDictionary();
                JsonData value2 = this.ToJsonData(value);
                KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
                this.inst_object[keyValuePair.Key] = value2;
                KeyValuePair<string, JsonData> value3 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value2);
                this.object_list[idx] = value3;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this.EnsureList()[index];
            }
            set
            {
                this.EnsureList();
                JsonData value2 = this.ToJsonData(value);
                this[index] = value2;
            }
        }

        public JsonData this[string prop_name]
        {
            get
            {
                this.EnsureDictionary();
                return this.inst_object[prop_name];
            }
            set
            {
                this.EnsureDictionary();
                KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
                if (this.inst_object.ContainsKey(prop_name))
                {
                    for (int i = 0; i < this.object_list.Count; i++)
                    {
                        if (this.object_list[i].Key == prop_name)
                        {
                            this.object_list[i] = keyValuePair;
                            break;
                        }
                    }
                }
                else
                {
                    this.object_list.Add(keyValuePair);
                }
                this.inst_object[prop_name] = value;
                this.json = null;
            }
        }

        public JsonData this[int index]
        {
            get
            {
                this.EnsureCollection();
                if (this.type == JsonType.Array)
                {
                    return this.inst_array[index];
                }
                return this.object_list[index].Value;
            }
            set
            {
                this.EnsureCollection();
                if (this.type == JsonType.Array)
                {
                    this.inst_array[index] = value;
                }
                else
                {
                    KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
                    KeyValuePair<string, JsonData> value2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
                    this.object_list[index] = value2;
                    this.inst_object[keyValuePair.Key] = value;
                }
                this.json = null;
            }
        }

        public bool ContainsKey(string key)
        {
            this.EnsureDictionary();
            return this.inst_object.Keys.Contains(key);
        }

        public JsonData()
        {
        }

        public JsonData(bool boolean)
        {
            this.type = JsonType.Boolean;
            this.inst_boolean = boolean;
        }

        public JsonData(double number)
        {
            this.type = JsonType.Double;
            this.inst_double = number;
        }

        public JsonData(int number)
        {
            this.type = JsonType.Int;
            this.inst_int = number;
        }

        public JsonData(long number)
        {
            this.type = JsonType.Long;
            this.inst_long = number;
        }

        public JsonData(object obj)
        {
            if (obj is bool)
            {
                this.type = JsonType.Boolean;
                this.inst_boolean = (bool)obj;
                return;
            }
            if (obj is double)
            {
                this.type = JsonType.Double;
                this.inst_double = (double)obj;
                return;
            }
            if (obj is int)
            {
                this.type = JsonType.Int;
                this.inst_int = (int)obj;
                return;
            }
            if (obj is long)
            {
                this.type = JsonType.Long;
                this.inst_long = (long)obj;
                return;
            }
            if (obj is string)
            {
                this.type = JsonType.String;
                this.inst_string = (string)obj;
                return;
            }
            throw new ArgumentException("Unable to wrap the given object with JsonData");
        }

        public JsonData(string str)
        {
            this.type = JsonType.String;
            this.inst_string = str;
        }

        public static implicit operator JsonData(bool data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(double data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(int data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(long data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(string data)
        {
            return new JsonData(data);
        }

        public static explicit operator bool(JsonData data)
        {
            if (data.type != JsonType.Boolean)
            {
                throw new InvalidCastException("Instance of JsonData doesn't hold a double");
            }
            return data.inst_boolean;
        }

        public static explicit operator double(JsonData data)
        {
            if (data.type != JsonType.Double)
            {
                throw new InvalidCastException("Instance of JsonData doesn't hold a double");
            }
            return data.inst_double;
        }

        public static explicit operator int(JsonData data)
        {
            if (data.type != JsonType.Int && data.type != JsonType.Long)
            {
                throw new InvalidCastException("Instance of JsonData doesn't hold an int");
            }
            if (data.type != JsonType.Int)
            {
                return (int)data.inst_long;
            }
            return data.inst_int;
        }

        public static explicit operator long(JsonData data)
        {
            if (data.type != JsonType.Long && data.type != JsonType.Int)
            {
                throw new InvalidCastException("Instance of JsonData doesn't hold a long");
            }
            if (data.type != JsonType.Long)
            {
                return data.inst_int;
            }
            return data.inst_long;
        }

        public static explicit operator string(JsonData data)
        {
            if (data.type != JsonType.String)
            {
                throw new InvalidCastException("Instance of JsonData doesn't hold a string");
            }
            return data.inst_string;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.EnsureCollection().CopyTo(array, index);
        }

        void IDictionary.Add(object key, object value)
        {
            JsonData value2 = this.ToJsonData(value);
            this.EnsureDictionary().Add(key, value2);
            KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
            this.object_list.Add(item);
            this.json = null;
        }

        void IDictionary.Clear()
        {
            this.EnsureDictionary().Clear();
            this.object_list.Clear();
            this.json = null;
        }

        bool IDictionary.Contains(object key)
        {
            return this.EnsureDictionary().Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IOrderedDictionary)this).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            this.EnsureDictionary().Remove(key);
            for (int i = 0; i < this.object_list.Count; i++)
            {
                if (this.object_list[i].Key == (string)key)
                {
                    this.object_list.RemoveAt(i);
                    break;
                }
            }
            this.json = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.EnsureCollection().GetEnumerator();
        }

        bool IJsonWrapper.GetBoolean()
        {
            if (this.type != JsonType.Boolean)
            {
                throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
            }
            return this.inst_boolean;
        }

        double IJsonWrapper.GetDouble()
        {
            if (this.type != JsonType.Double)
            {
                throw new InvalidOperationException("JsonData instance doesn't hold a double");
            }
            return this.inst_double;
        }

        int IJsonWrapper.GetInt()
        {
            if (this.type != JsonType.Int)
            {
                throw new InvalidOperationException("JsonData instance doesn't hold an int");
            }
            return this.inst_int;
        }

        long IJsonWrapper.GetLong()
        {
            if (this.type != JsonType.Long)
            {
                throw new InvalidOperationException("JsonData instance doesn't hold a long");
            }
            return this.inst_long;
        }

        string IJsonWrapper.GetString()
        {
            if (this.type != JsonType.String)
            {
                throw new InvalidOperationException("JsonData instance doesn't hold a string");
            }
            return this.inst_string;
        }

        void IJsonWrapper.SetBoolean(bool val)
        {
            this.type = JsonType.Boolean;
            this.inst_boolean = val;
            this.json = null;
        }

        void IJsonWrapper.SetDouble(double val)
        {
            this.type = JsonType.Double;
            this.inst_double = val;
            this.json = null;
        }

        void IJsonWrapper.SetInt(int val)
        {
            this.type = JsonType.Int;
            this.inst_int = val;
            this.json = null;
        }

        void IJsonWrapper.SetLong(long val)
        {
            this.type = JsonType.Long;
            this.inst_long = val;
            this.json = null;
        }

        void IJsonWrapper.SetString(string val)
        {
            this.type = JsonType.String;
            this.inst_string = val;
            this.json = null;
        }

        string IJsonWrapper.ToJson()
        {
            return this.ToJson();
        }

        void IJsonWrapper.ToJson(JsonWriter writer)
        {
            this.ToJson(writer);
        }

        int IList.Add(object value)
        {
            return this.Add(value);
        }

        void IList.Clear()
        {
            this.EnsureList().Clear();
            this.json = null;
        }

        bool IList.Contains(object value)
        {
            return this.EnsureList().Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.EnsureList().IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.EnsureList().Insert(index, value);
            this.json = null;
        }

        void IList.Remove(object value)
        {
            this.EnsureList().Remove(value);
            this.json = null;
        }

        void IList.RemoveAt(int index)
        {
            this.EnsureList().RemoveAt(index);
            this.json = null;
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
        {
            this.EnsureDictionary();
            return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
        }

        void IOrderedDictionary.Insert(int idx, object key, object value)
        {
            string text = (string)key;
            JsonData value2 = (this[text] = this.ToJsonData(value));
            KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
            this.object_list.Insert(idx, item);
        }

        void IOrderedDictionary.RemoveAt(int idx)
        {
            this.EnsureDictionary();
            this.inst_object.Remove(this.object_list[idx].Key);
            this.object_list.RemoveAt(idx);
        }

        private ICollection EnsureCollection()
        {
            if (this.type == JsonType.Array)
            {
                return (ICollection)this.inst_array;
            }
            if (this.type == JsonType.Object)
            {
                return (ICollection)this.inst_object;
            }
            throw new InvalidOperationException("The JsonData instance has to be initialized first");
        }

        private IDictionary EnsureDictionary()
        {
            if (this.type == JsonType.Object)
            {
                return (IDictionary)this.inst_object;
            }
            if (this.type != 0)
            {
                throw new InvalidOperationException("Instance of JsonData is not a dictionary");
            }
            this.type = JsonType.Object;
            this.inst_object = new Dictionary<string, JsonData>();
            this.object_list = new List<KeyValuePair<string, JsonData>>();
            return (IDictionary)this.inst_object;
        }

        private IList EnsureList()
        {
            if (this.type == JsonType.Array)
            {
                return (IList)this.inst_array;
            }
            if (this.type != 0)
            {
                throw new InvalidOperationException("Instance of JsonData is not a list");
            }
            this.type = JsonType.Array;
            this.inst_array = new List<JsonData>();
            return (IList)this.inst_array;
        }

        private JsonData ToJsonData(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is JsonData)
            {
                return (JsonData)obj;
            }
            return new JsonData(obj);
        }

        private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
        {
            if (obj == null)
            {
                writer.Write(null);
            }
            else if (obj.IsString)
            {
                writer.Write(obj.GetString());
            }
            else if (obj.IsBoolean)
            {
                writer.Write(obj.GetBoolean());
            }
            else if (obj.IsDouble)
            {
                writer.Write(obj.GetDouble());
            }
            else if (obj.IsInt)
            {
                writer.Write(obj.GetInt());
            }
            else if (obj.IsLong)
            {
                writer.Write(obj.GetLong());
            }
            else if (obj.IsArray)
            {
                writer.WriteArrayStart();
                foreach (JsonData item in (IEnumerable)obj)
                {
                    JsonData.WriteJson(item, writer);
                }
                writer.WriteArrayEnd();
            }
            else
            {
                if (!obj.IsObject)
                {
                    return;
                }
                writer.WriteObjectStart();
                foreach (DictionaryEntry item2 in (IDictionary)obj)
                {
                    writer.WritePropertyName((string)item2.Key);
                    JsonData.WriteJson((JsonData)item2.Value, writer);
                }
                writer.WriteObjectEnd();
            }
        }

        public int Add(object value)
        {
            JsonData value2 = this.ToJsonData(value);
            this.json = null;
            return this.EnsureList().Add(value2);
        }

        public bool Remove(object obj)
        {
            this.json = null;
            if (this.IsObject)
            {
                JsonData value = null;
                if (this.inst_object.TryGetValue((string)obj, out value))
                {
                    if (this.inst_object.Remove((string)obj))
                    {
                        return this.object_list.Remove(new KeyValuePair<string, JsonData>((string)obj, value));
                    }
                    return false;
                }
                throw new KeyNotFoundException("The specified key was not found in the JsonData object.");
            }
            if (this.IsArray)
            {
                return this.inst_array.Remove(this.ToJsonData(obj));
            }
            throw new InvalidOperationException("Instance of JsonData is not an object or a list.");
        }

        public void Clear()
        {
            if (this.IsObject)
            {
                ((IDictionary)this).Clear();
            }
            else if (this.IsArray)
            {
                ((IList)this).Clear();
            }
        }

        public bool Equals(JsonData x)
        {
            if (x == null)
            {
                return false;
            }
            if (x.type != this.type && ((x.type != JsonType.Int && x.type != JsonType.Long) || (this.type != JsonType.Int && this.type != JsonType.Long)))
            {
                return false;
            }
            switch (this.type)
            {
            case JsonType.None:
                return true;
            case JsonType.Object:
                return this.inst_object.Equals(x.inst_object);
            case JsonType.Array:
                return this.inst_array.Equals(x.inst_array);
            case JsonType.String:
                return this.inst_string.Equals(x.inst_string);
            case JsonType.Int:
                if (x.IsLong)
                {
                    if (x.inst_long < int.MinValue || x.inst_long > int.MaxValue)
                    {
                        return false;
                    }
                    return this.inst_int.Equals((int)x.inst_long);
                }
                return this.inst_int.Equals(x.inst_int);
            case JsonType.Long:
                if (x.IsInt)
                {
                    if (this.inst_long < int.MinValue || this.inst_long > int.MaxValue)
                    {
                        return false;
                    }
                    return x.inst_int.Equals((int)this.inst_long);
                }
                return this.inst_long.Equals(x.inst_long);
            case JsonType.Double:
                return this.inst_double.Equals(x.inst_double);
            case JsonType.Boolean:
                return this.inst_boolean.Equals(x.inst_boolean);
            default:
                return false;
            }
        }

        public JsonType GetJsonType()
        {
            return this.type;
        }

        public void SetJsonType(JsonType type)
        {
            if (this.type != type)
            {
                switch (type)
                {
                case JsonType.Object:
                    this.inst_object = new Dictionary<string, JsonData>();
                    this.object_list = new List<KeyValuePair<string, JsonData>>();
                    break;
                case JsonType.Array:
                    this.inst_array = new List<JsonData>();
                    break;
                case JsonType.String:
                    this.inst_string = null;
                    break;
                case JsonType.Int:
                    this.inst_int = 0;
                    break;
                case JsonType.Long:
                    this.inst_long = 0L;
                    break;
                case JsonType.Double:
                    this.inst_double = 0.0;
                    break;
                case JsonType.Boolean:
                    this.inst_boolean = false;
                    break;
                }
                this.type = type;
            }
        }

        public string ToJson()
        {
            if (this.json != null)
            {
                return this.json;
            }
            StringWriter stringWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonWriter(stringWriter);
            jsonWriter.Validate = false;
            JsonData.WriteJson(this, jsonWriter);
            this.json = stringWriter.ToString();
            return this.json;
        }

        public void ToJson(JsonWriter writer)
        {
            bool validate = writer.Validate;
            writer.Validate = false;
            JsonData.WriteJson(this, writer);
            writer.Validate = validate;
        }

        public override string ToString()
        {
            switch (this.type)
            {
            case JsonType.Array:
                return "JsonData array";
            case JsonType.Boolean:
                return this.inst_boolean.ToString();
            case JsonType.Double:
                return this.inst_double.ToString();
            case JsonType.Int:
                return this.inst_int.ToString();
            case JsonType.Long:
                return this.inst_long.ToString();
            case JsonType.Object:
                return "JsonData object";
            case JsonType.String:
                return this.inst_string;
            default:
                return "Uninitialized JsonData";
            }
        }
    }
}
