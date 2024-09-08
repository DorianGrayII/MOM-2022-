// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHAsync<T>
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class MHAsync<T>
{
    public delegate void Processor(T o);

    public delegate void Callback(MHAsync<T> source);

    private Processor processor;

    private Callback onFinish;

    private IEnumerable<T> data;

    private T singleData;

    private int dataSize;

    public static MHAsync<T> Create(Processor processor, List<T> data, Callback onFinish)
    {
        return new MHAsync<T>
        {
            processor = processor,
            onFinish = onFinish,
            data = data,
            dataSize = data.Count
        };
    }

    public static MHAsync<T> Create(Processor processor, T[] data, Callback onFinish)
    {
        return new MHAsync<T>
        {
            processor = processor,
            onFinish = onFinish,
            data = data,
            dataSize = data.Length
        };
    }

    public static IEnumerator CreateSingle(Processor processor, T data, Callback onFinish)
    {
        MHAsync<T> mha = new MHAsync<T>
        {
            processor = processor,
            onFinish = onFinish,
            singleData = data,
            dataSize = 1
        };
        Task task = Task.Run(delegate
        {
            processor(data);
        });
        TaskAwaiter awaiter = task.GetAwaiter();
        while (!awaiter.IsCompleted)
        {
            yield return null;
        }
        onFinish?.Invoke(mha);
    }

    public IEnumerator Start(bool forceSingleThread = false)
    {
        if (forceSingleThread)
        {
            foreach (T datum in this.data)
            {
                this.processor(datum);
            }
        }
        else
        {
            List<Task> list = new List<Task>();
            foreach (T v in this.data)
            {
                list.Add(Task.Run(delegate
                {
                    this.processor(v);
                }));
            }
            Task final = Task.WhenAll(list.ToArray());
            while (!final.IsCompleted)
            {
                yield return null;
            }
        }
        if (this.onFinish != null)
        {
            this.onFinish(this);
        }
    }
}
