using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class ProtoLibrary
{
    public enum SerializationStatus
    {
        Invalid = 0,
        NotStarted = 1,
        Started = 2,
        Ready = 3,
        Destroyed = 4
    }

    private static ProtoLibrary instance;

    [ProtoMember(1)]
    public List<ProtoLibraryItem> list;

    [ProtoMember(10)]
    public ProtoRef<PlaneMeshData> arcanus;

    [ProtoMember(11)]
    public ProtoRef<PlaneMeshData> myrror;

    [ProtoMember(12)]
    public int mapResolution;

    [ProtoMember(13)]
    public int gameID;

    [ProtoIgnore]
    private Dictionary<object, int> refs;

    [ProtoIgnore]
    public MemoryStream serialized;

    [ProtoIgnore]
    private SerializationStatus status;

    [ProtoIgnore]
    private Thread thread;

    [ProtoIgnore]
    private Mutex mutex = new Mutex();

    [ProtoIgnore]
    private float seraializationDelay = -1f;

    [ProtoIgnore]
    private Dictionary<Vertex, float> heightReverse;

    private ProtoLibrary()
    {
    }

    ~ProtoLibrary()
    {
        this.mutex.WaitOne();
        this.status = SerializationStatus.Destroyed;
        this.thread = null;
        if (this.serialized != null)
        {
            this.serialized.Dispose();
        }
        this.mutex.ReleaseMutex();
        this.mutex.Dispose();
        this.mutex = null;
    }

    public static int Add(ProtoLibraryItem obj)
    {
        if (ProtoLibrary.instance == null)
        {
            if (Settings.GetData().GetExperimentalLoading() < 2)
            {
                return 0;
            }
            ProtoLibrary.instance = new ProtoLibrary();
            ProtoLibrary.instance.gameID = World.Get().gameID;
        }
        return ProtoLibrary.instance._Add(obj);
    }

    public int _Add(ProtoLibraryItem obj)
    {
        if (this.refs != null && this.refs.ContainsKey(obj))
        {
            return this.refs[obj];
        }
        if (this.refs == null)
        {
            this.refs = new Dictionary<object, int>();
        }
        if (this.list == null)
        {
            this.list = new List<ProtoLibraryItem>();
        }
        int count = this.list.Count;
        this.list.Add(obj);
        this.refs[obj] = count;
        return count;
    }

    public static ProtoLibraryItem Get(int idx)
    {
        if (Settings.GetData().GetExperimentalLoading() < 2)
        {
            return null;
        }
        if (ProtoLibrary.instance == null)
        {
            ProtoLibrary.instance = new ProtoLibrary();
            ProtoLibrary.instance.gameID = World.Get().gameID;
        }
        if (ProtoLibrary.instance.list != null && ProtoLibrary.instance.list.Count > idx)
        {
            return ProtoLibrary.instance.list[idx];
        }
        return null;
    }

    public static void Clear()
    {
        if (ProtoLibrary.instance != null)
        {
            ProtoLibrary.instance.ClearSerializationBuffer();
            ProtoLibrary.instance.list?.Clear();
            ProtoLibrary.instance.refs?.Clear();
            ProtoLibrary.instance.arcanus = null;
            ProtoLibrary.instance.myrror = null;
            ProtoLibrary.instance.heightReverse.Clear();
            ProtoLibrary.instance = null;
        }
    }

    public void ResetAfterChanges()
    {
        if (this.list != null)
        {
            foreach (ProtoLibraryItem item in this.list)
            {
                if (item is MeshCell meshCell)
                {
                    meshCell.hex = null;
                }
                else if (item is Chunk chunk)
                {
                    chunk.mcs.DuringReset();
                }
            }
        }
        if (this.heightReverse == null)
        {
            return;
        }
        foreach (KeyValuePair<Vertex, float> item2 in this.heightReverse)
        {
            item2.Key.position.y = item2.Value;
        }
    }

    public void RecordHeightChange(Vertex v, float h)
    {
        if (this.heightReverse == null)
        {
            this.heightReverse = new Dictionary<Vertex, float>();
        }
        if (!this.heightReverse.ContainsKey(v))
        {
            this.heightReverse[v] = h;
        }
    }

    public static ProtoLibrary GetInstance(bool testSettings = true)
    {
        if (testSettings && Settings.GetData().GetExperimentalLoading() < 2)
        {
            ProtoLibrary.instance?.ClearSerializationBuffer();
            ProtoLibrary.instance = null;
        }
        return ProtoLibrary.instance;
    }

    public static void SetInstance(ProtoLibrary pl)
    {
        if (Settings.GetData().GetExperimentalLoading() >= 2)
        {
            ProtoLibrary.instance = pl;
        }
    }

    public static SerializationStatus GetSerializedStatus()
    {
        return ProtoLibrary.GetInstance()?.status ?? SerializationStatus.Invalid;
    }

    private void SerializeSelf_()
    {
        this.mutex.WaitOne();
        this.thread = null;
        this.status = SerializationStatus.Started;
        if (this.serialized != null)
        {
            this.serialized.Dispose();
        }
        this.serialized = null;
        this.thread = new Thread(SerializationProcess);
        this.thread.Start();
        this.mutex.ReleaseMutex();
    }

    private void SerializationProcess()
    {
        int num = World.Get().gameID;
        MemoryStream memoryStream = null;
        try
        {
            if (!Directory.Exists(MHApplication.CACHE))
            {
                Directory.CreateDirectory(MHApplication.CACHE);
            }
            string path = num + "_" + this.mapResolution + ".bin";
            string path2 = Path.Combine(MHApplication.CACHE, path);
            if (!File.Exists(path2))
            {
                memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, this);
                byte[] bytes = memoryStream.ToArray();
                File.WriteAllBytes(path2, bytes);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("CACHE FAILURE \n" + ex);
        }
        memoryStream?.Dispose();
        this.mutex?.WaitOne();
        if (this.status != SerializationStatus.Destroyed && this.thread == Thread.CurrentThread)
        {
            this.serialized?.Dispose();
            this.serialized = memoryStream;
            this.status = SerializationStatus.Ready;
        }
        else
        {
            memoryStream?.Dispose();
        }
        this.mutex?.ReleaseMutex();
    }

    private void ClearSerializationBuffer()
    {
        this.mutex?.WaitOne();
        this.status = SerializationStatus.NotStarted;
        this.serialized?.Dispose();
        this.serialized = null;
        this.mutex?.ReleaseMutex();
    }

    public void Update()
    {
        if (this.seraializationDelay > 0f)
        {
            this.seraializationDelay -= Time.deltaTime;
        }
        else if (this.status == SerializationStatus.NotStarted)
        {
            this.SerializeSelf_();
        }
    }

    public static void MeshIteration()
    {
        if (!(World.Get() == null))
        {
            ProtoLibrary.GetInstance().seraializationDelay = 0.5f;
            ProtoLibrary.GetInstance().ClearSerializationBuffer();
        }
    }

    public static void SanitizeSaveCache(List<SaveMeta> avaliableMetas)
    {
        if (!Directory.Exists(MHApplication.CACHE))
        {
            return;
        }
        try
        {
            List<string> list = new List<string>();
            if (ProtoLibrary.GetInstance() != null)
            {
                list.Add(ProtoLibrary.GetInstance().gameID.ToString());
            }
            if (avaliableMetas != null && Settings.GetData().GetExperimentalLoading() == 2)
            {
                foreach (SaveMeta avaliableMeta in avaliableMetas)
                {
                    string item = avaliableMeta.gameID.ToString();
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            string[] files = Directory.GetFiles(MHApplication.CACHE);
            foreach (string path in files)
            {
                string fileName = Path.GetFileName(path);
                bool flag = false;
                foreach (string item2 in list)
                {
                    if (fileName.StartsWith(item2))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    File.Delete(path);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Cache clearing functionality throw an exception! \n" + ex);
        }
    }
}
