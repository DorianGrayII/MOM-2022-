// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.MHNonThread
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MHUtils;
using UnityEngine;

public class MHNonThread
{
    public static bool DEBUG_MODE = true;

    private ITask singleTask;

    private ITask[] arrayOfTasks;

    private List<ITask> listOfTasks;

    private object result;

    private object[] results;

    private bool terminate;

    private bool storeResults;

    private int taskCount;

    private int finishedCount;

    private Exception firstException;

    public global::MHUtils.Callback onFinish;

    public global::MHUtils.Callback onError;

    private MHNonThread()
    {
    }

    private MHNonThread(ITask singleTask)
    {
        this.singleTask = singleTask;
        this.taskCount = 1;
    }

    private MHNonThread(List<ITask> listOfTasks, bool storeResults = true)
    {
        this.listOfTasks = listOfTasks;
        this.storeResults = storeResults;
        this.taskCount = listOfTasks.Count;
        if (storeResults)
        {
            this.results = new object[this.taskCount];
        }
    }

    private MHNonThread(ITask[] arrayOfTasks, bool storeResults = true)
    {
        this.arrayOfTasks = arrayOfTasks;
        this.storeResults = storeResults;
        this.taskCount = arrayOfTasks.Length;
        if (storeResults)
        {
            this.results = new object[this.taskCount];
        }
    }

    public static object DirectExecution(ITask task)
    {
        return task.Execute();
    }

    public static IEnumerator ExecuteSequence(global::MHUtils.Callback[] cbs, object data = null, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null, global::MHUtils.Callback onProgress = null)
    {
        if (cbs == null || cbs.Length < 1)
        {
            Debug.LogError("Data not provided to initialize array of the tasks");
            yield break;
        }
        MHTimer t = MHTimer.StartNew();
        _ = new object[cbs.Length];
        float time = t.GetTime();
        for (int i = 0; i < cbs.Length; i++)
        {
            cbs[i](data);
            onProgress?.Invoke((float)i / (float)cbs.Length);
            if (t.GetTime() - time > 50f)
            {
                yield return null;
                time = t.GetTime();
            }
        }
        onFinish?.Invoke(null);
    }

    public static IEnumerator CreateMulti<T>(CallbackRet action, List<T> data, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null, global::MHUtils.Callback onProgress = null)
    {
        if (data == null || data.Count < 1)
        {
            Debug.LogError("Data not provided to initialize array of the tasks");
            yield break;
        }
        MHTimer t = MHTimer.StartNew();
        MHNonThread mht = new MHNonThread
        {
            onFinish = onFinish,
            onError = onError,
            results = new object[data.Count]
        };
        float time = t.GetTime();
        for (int i = 0; i < data.Count; i++)
        {
            BasicTask basicTask = new BasicTask();
            basicTask.ca = action;
            basicTask.data = data[i];
            mht.results[i] = basicTask.Execute();
            onProgress?.Invoke((float)i / (float)data.Count);
            if (t.GetTime() - time > 50f)
            {
                yield return null;
                time = t.GetTime();
            }
        }
        onFinish?.Invoke(mht.results);
    }

    public static MHNonThread Create(ITask singleTask, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null)
    {
        MHNonThread @object = new MHNonThread(singleTask)
        {
            onFinish = onFinish,
            onError = onError
        };
        if (!ThreadPool.QueueUserWorkItem(@object.ProcessSingleMode))
        {
            onError?.Invoke("Queue for single-task multi threading failed!");
        }
        return @object;
    }

    public static MHNonThread Create(List<ITask> tasks, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null)
    {
        MHNonThread mHNonThread = new MHNonThread(tasks);
        mHNonThread.onFinish = onFinish;
        mHNonThread.onError = onError;
        for (int i = 0; i < tasks.Count; i++)
        {
            if (!ThreadPool.QueueUserWorkItem(mHNonThread.Process, i))
            {
                onError?.Invoke("Queue for list-task multi threading failed!");
                break;
            }
        }
        return mHNonThread;
    }

    public static MHNonThread Create(ITask[] tasks, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null)
    {
        MHNonThread mHNonThread = new MHNonThread(tasks);
        mHNonThread.onFinish = onFinish;
        mHNonThread.onError = onError;
        for (int i = 0; i < tasks.Length; i++)
        {
            if (!ThreadPool.QueueUserWorkItem(mHNonThread.Process, i))
            {
                onError?.Invoke("Queue for array-task multi threading failed!");
                break;
            }
        }
        return mHNonThread;
    }

    public static MHNonThread CreateSequence(ITask[] tasks, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null, global::MHUtils.Callback onProgress = null)
    {
        MHNonThread mHNonThread = new MHNonThread(tasks);
        mHNonThread.onFinish = onFinish;
        mHNonThread.onError = onError;
        if (MHNonThread.DEBUG_MODE)
        {
            mHNonThread.ProcessSequence(" <Fake thread> ");
        }
        else if (!ThreadPool.QueueUserWorkItem(mHNonThread.ProcessSequence))
        {
            onError?.Invoke("Queue for array-task multi threading failed!");
        }
        return mHNonThread;
    }

    public static IEnumerator Start(List<ITask> tasks, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null)
    {
        MHNonThread mt = MHNonThread.Create(tasks, onFinish, onError);
        while (!mt.IsFinished())
        {
            yield return null;
        }
    }

    public static IEnumerator Start(ITask[] tasks, global::MHUtils.Callback onFinish = null, global::MHUtils.Callback onError = null)
    {
        MHNonThread mt = MHNonThread.Create(tasks, onFinish, onError);
        while (!mt.IsFinished())
        {
            yield return null;
        }
    }

    public IEnumerator WaitForceFinish()
    {
        this.Destroy();
        while (!this.IsFinished())
        {
            yield return null;
        }
    }

    private void ProcessSingleMode(object o)
    {
        try
        {
            if (this.singleTask != null && !this.terminate)
            {
                this.result = this.singleTask.Execute();
            }
            Interlocked.Increment(ref this.finishedCount);
        }
        catch (Exception ex)
        {
            if (this.firstException == null)
            {
                this.firstException = ex;
            }
            if (this.onError != null)
            {
                this.onError(ex);
            }
            else
            {
                Debug.LogError(ex);
            }
        }
        this.onFinish?.Invoke(this.result);
    }

    private void ProcessSequence(object o)
    {
        try
        {
            if (!this.terminate)
            {
                for (int i = 0; i < this.taskCount; i++)
                {
                    object obj = null;
                    if (this.listOfTasks != null)
                    {
                        obj = this.listOfTasks[i].Execute();
                    }
                    else if (this.arrayOfTasks != null)
                    {
                        obj = this.arrayOfTasks[i].Execute();
                    }
                    if (this.results != null)
                    {
                        this.results[i] = obj;
                    }
                    Interlocked.Increment(ref this.finishedCount);
                }
            }
        }
        catch (Exception ex)
        {
            if (this.firstException == null)
            {
                this.firstException = ex;
            }
            Debug.LogError("Thread for " + o?.ToString() + " get exception\n " + ex);
            Interlocked.Increment(ref this.finishedCount);
            this.onError?.Invoke(ex);
        }
        this.onFinish?.Invoke(this.results);
        this.onFinish = null;
    }

    private void Process(object o)
    {
        try
        {
            int num = (int)o;
            if (!this.terminate)
            {
                object obj = null;
                if (this.listOfTasks != null)
                {
                    obj = this.listOfTasks[num].Execute();
                }
                else if (this.arrayOfTasks != null)
                {
                    obj = this.arrayOfTasks[num].Execute();
                }
                if (this.results != null)
                {
                    this.results[num] = obj;
                }
            }
            Interlocked.Increment(ref this.finishedCount);
        }
        catch (Exception ex)
        {
            if (this.firstException == null)
            {
                this.firstException = ex;
            }
            Debug.LogError("Thread " + o?.ToString() + " get exception\n " + ex);
            Interlocked.Increment(ref this.finishedCount);
            this.onError?.Invoke(ex);
            this.Destroy();
        }
        if (this.finishedCount == this.taskCount && Monitor.TryEnter(this))
        {
            try
            {
                this.onFinish?.Invoke(this.results);
                this.onFinish = null;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }

    public float GetProgress()
    {
        if (this.singleTask != null)
        {
            return this.finishedCount;
        }
        return (float)this.finishedCount / (float)this.taskCount;
    }

    public bool IsFinished()
    {
        return this.finishedCount == this.taskCount;
    }

    public object[] GetResults()
    {
        return this.results;
    }

    public object GetResultSingle()
    {
        return this.result;
    }

    public int GetTaskTotal()
    {
        return this.taskCount;
    }

    public void Destroy()
    {
        this.onFinish = null;
        this.onError = null;
        this.terminate = true;
    }
}
