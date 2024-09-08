using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MHUtils
{
    public class MHThread
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

        public Callback onFinish;

        public Callback onError;

        private MHThread()
        {
        }

        private MHThread(ITask singleTask)
        {
            this.singleTask = singleTask;
            this.taskCount = 1;
        }

        private MHThread(List<ITask> listOfTasks, bool storeResults = true)
        {
            this.listOfTasks = listOfTasks;
            this.storeResults = storeResults;
            this.taskCount = listOfTasks.Count;
            if (storeResults)
            {
                this.results = new object[this.taskCount];
            }
        }

        private MHThread(ITask[] arrayOfTasks, bool storeResults = true)
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

        public static MHThread ExecuteSequence(Callback[] cbs, object data = null, Callback onFinish = null, Callback onError = null)
        {
            if (cbs == null || cbs.Length < 1)
            {
                Debug.LogError("Data not provided to initialize array of the tasks");
                return null;
            }
            BasicTask[] array = new BasicTask[cbs.Length];
            for (int i = 0; i < cbs.Length; i++)
            {
                BasicTask basicTask = new BasicTask();
                int index = i;
                basicTask.ca = delegate(object o)
                {
                    cbs[index](o);
                    return o;
                };
                basicTask.data = data;
                array[i] = basicTask;
            }
            ITask[] tasks = array;
            return MHThread.CreateSequence(tasks, onFinish, onError);
        }

        public static MHThread CreateMulti<T>(CallbackRet action, List<T> data, Callback onFinish = null, Callback onError = null)
        {
            if (data == null || data.Count < 1)
            {
                Debug.LogError("Data not provided to initialize array of the tasks");
                return null;
            }
            BasicTask[] array = new BasicTask[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                BasicTask basicTask = new BasicTask();
                basicTask.ca = action;
                basicTask.data = data[i];
                array[i] = basicTask;
            }
            ITask[] tasks = array;
            return MHThread.Create(tasks, onFinish, onError);
        }

        public static MHThread Create(ITask[] tasks, Callback onFinish = null, Callback onError = null)
        {
            MHThread mHThread = new MHThread(tasks);
            mHThread.onFinish = onFinish;
            mHThread.onError = onError;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!ThreadPool.QueueUserWorkItem(mHThread.Process, i))
                {
                    onError?.Invoke("Queue for array-task multi threading failed!");
                    break;
                }
            }
            return mHThread;
        }

        public static MHThread CreateSequence(ITask[] tasks, Callback onFinish = null, Callback onError = null)
        {
            MHThread mHThread = new MHThread(tasks);
            mHThread.onFinish = onFinish;
            mHThread.onError = onError;
            if (MHThread.DEBUG_MODE)
            {
                mHThread.ProcessSequence(" <Fake thread> ");
            }
            else if (!ThreadPool.QueueUserWorkItem(mHThread.ProcessSequence))
            {
                onError?.Invoke("Queue for array-task multi threading failed!");
            }
            return mHThread;
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
}
