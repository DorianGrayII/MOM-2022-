using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MHUtils
{
    public class MHThreadingSideLineSystem : MonoBehaviour
    {
        private bool multithreading = true;

        private int count = 1;

        private List<ITask> listOfTasks = new List<ITask>();

        private BaseTaskStruct[] arrayOfTaskStructs;

        private object[] results;

        private List<Thread> threads = new List<Thread>();

        private string threadSource;

        private int[] workDone;

        private bool terminate;

        private Coroutine fakeThread;

        private Exception[] exception;

        public static MHThreadingSideLineSystem Create(int count = 1, bool debugThreads = false)
        {
            MHThreadingSideLineSystem mHThreadingSideLineSystem = new GameObject("MHThreadingSideLineSystem").AddComponent<MHThreadingSideLineSystem>();
            mHThreadingSideLineSystem.threadSource = Environment.StackTrace;
            mHThreadingSideLineSystem.multithreading = !debugThreads;
            mHThreadingSideLineSystem.count = count;
            return mHThreadingSideLineSystem;
        }

        public void Destroy()
        {
            global::UnityEngine.Object.Destroy(base.gameObject);
        }

        public void Initialize(IEnumerable tasksData, int length, Func<object, object> executor)
        {
            if (tasksData == null)
            {
                Debug.LogError("[ERROR]Task data is null");
                return;
            }
            BaseTaskStruct[] array = new BaseTaskStruct[length];
            int num = 0;
            foreach (object tasksDatum in tasksData)
            {
                array[num].data = tasksDatum;
                array[num].ca = executor;
                num++;
            }
            this.Initialize(array);
        }

        public void Initialize(BaseTaskStruct[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                Debug.LogError("[ERROR]Task count required to be 1 or more");
                return;
            }
            if (!this.IsFinished())
            {
                Debug.LogError("[ERROR]Thread system already working when fed with new tasks");
                return;
            }
            this.arrayOfTaskStructs = tasks;
            this.results = new object[tasks.Length];
            this.workDone = new int[this.count];
            if (this.multithreading)
            {
                for (int i = 0; i < this.count; i++)
                {
                    Thread thread = new Thread(ThreadStructWorker);
                    this.threads.Add(thread);
                    thread.Start(i);
                }
            }
            else
            {
                Debug.LogError("fake thread not supported for struct tasks");
            }
        }

        public void Initialize(List<ITask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                Debug.LogError("[ERROR]Task count required to be 1 or more");
                return;
            }
            if (!this.IsFinished())
            {
                Debug.LogError("[ERROR]Thread system already working when fed with new tasks");
                return;
            }
            this.listOfTasks = new List<ITask>(tasks);
            this.results = new object[this.listOfTasks.Count];
            this.workDone = new int[this.count];
            if (this.multithreading)
            {
                for (int i = 0; i < this.count; i++)
                {
                    Thread thread = new Thread(ThreadWorker);
                    this.threads.Add(thread);
                    thread.Start(i);
                }
            }
            else
            {
                this.fakeThread = base.StartCoroutine(this.FakeThread());
            }
        }

        public bool IsFinished()
        {
            if (this.multithreading)
            {
                if (this.threads.Count != 0)
                {
                    return this.threads.Find((Thread o) => o.IsAlive) == null;
                }
                return true;
            }
            return this.fakeThread == null;
        }

        private void OnDestroy()
        {
            if (this.multithreading)
            {
                if (this.threads == null)
                {
                    return;
                }
                this.terminate = true;
            }
            else
            {
                if (this.fakeThread == null)
                {
                    return;
                }
                this.terminate = true;
                base.StopCoroutine(this.fakeThread);
            }
            if (this.threads == null)
            {
                return;
            }
            for (int i = 0; i < this.threads.Count; i++)
            {
                if (this.threads[i] != null && !this.threads[i].Join(200))
                {
                    Debug.LogError("Thread end failed (MHThreadingSideLineSystem) created at: \n" + this.threadSource);
                }
            }
        }

        public object[] GetResults()
        {
            this.threads.Clear();
            return this.results;
        }

        public int GetProgress()
        {
            int num = 0;
            if (this.workDone != null)
            {
                int[] array = this.workDone;
                foreach (int num2 in array)
                {
                    num += num2;
                }
            }
            return num;
        }

        private void ThreadStructWorker(object o)
        {
            int num = (int)o;
            try
            {
                for (int i = num; i < this.arrayOfTaskStructs.Length; i += this.count)
                {
                    if (this.terminate)
                    {
                        break;
                    }
                    BaseTaskStruct baseTaskStruct = this.arrayOfTaskStructs[i];
                    object obj = baseTaskStruct.Execute();
                    this.results[i] = obj;
                    this.workDone[num]++;
                }
            }
            catch (Exception message)
            {
                Debug.LogError(message);
            }
        }

        private void ThreadWorker(object o)
        {
            int num = (int)o;
            try
            {
                for (int i = num; i < this.listOfTasks.Count; i += this.count)
                {
                    if (this.terminate)
                    {
                        break;
                    }
                    object obj = this.listOfTasks[i].Execute();
                    this.results[i] = obj;
                    this.workDone[num]++;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("SideLine error for :" + this?.ToString() + " \n " + ex.ToString());
            }
        }

        private IEnumerator FakeThread()
        {
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            int itemsPerFrame = 2;
            int num = 0;
            float frameSize = 0.003f;
            for (int index = 0; index < this.listOfTasks.Count; index++)
            {
                if (this.terminate)
                {
                    break;
                }
                object obj = this.listOfTasks[index].Execute();
                this.results[index] = obj;
                this.workDone[0]++;
                if (index >= itemsPerFrame + num)
                {
                    float num2 = Time.realtimeSinceStartup - realtimeSinceStartup;
                    if (num2 < frameSize)
                    {
                        itemsPerFrame = Mathf.CeilToInt((float)itemsPerFrame * frameSize / num2);
                        continue;
                    }
                    if (num2 > frameSize * 2f)
                    {
                        itemsPerFrame = Mathf.Max(1, Mathf.CeilToInt((float)itemsPerFrame * frameSize / num2));
                        continue;
                    }
                    yield return null;
                    num = index;
                    realtimeSinceStartup = Time.realtimeSinceStartup;
                }
            }
            this.fakeThread = null;
        }
    }
}
