using System;
using System.Threading;
using UnityEngine;

namespace MHUtils
{
    public class MHRandom
    {
        private static Semaphore semaphore;

        private static MHRandom singleton;

        public static int seedSource;

        private global::System.Random random;

        private const int day = 86400000;

        private const int hour = 3600000;

        private const int minutes = 60000;

        private const int seconds = 1000;

        private bool semaphored;

        public static MHRandom Get()
        {
            if (MHRandom.singleton == null)
            {
                MHRandom.singleton = new MHRandom();
                MHRandom.singleton.semaphored = true;
            }
            if (MHRandom.semaphore == null)
            {
                MHRandom.semaphore = new Semaphore(1, 1);
            }
            return MHRandom.singleton;
        }

        public MHRandom(int seed = 0, bool unsourced = false)
        {
            if (seed == 0)
            {
                if (unsourced)
                {
                    seed = DateTime.Now.Minute * 100000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
                }
                else if (MHRandom.seedSource == 0)
                {
                    MHRandom.seedSource = DateTime.Now.Minute * 100000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
                    MHRandom.seedSource++;
                    seed = MHRandom.seedSource;
                }
                else
                {
                    MHRandom.seedSource++;
                    seed = MHRandom.seedSource;
                }
            }
            this.random = new global::System.Random(seed);
        }

        public MHRandom(global::System.Random r)
        {
            this.random = r;
        }

        public int GetInt(int from, int to)
        {
            this.StartSemaphore();
            int result = ((to == from) ? to : ((to >= from) ? this.random.Next(from, to) : this.random.Next(to, from)));
            this.EndSemaphore();
            return result;
        }

        public int GetIntMinMax()
        {
            return this.GetInt(int.MinValue, int.MaxValue);
        }

        public int GetSuccesses(float chance, int tryCount)
        {
            int num = 0;
            this.StartSemaphore();
            for (int i = 0; i < tryCount; i++)
            {
                if (this.random.NextDouble() < (double)chance)
                {
                    num++;
                }
            }
            this.EndSemaphore();
            return num;
        }

        public float GetFloat(float from, float to)
        {
            this.StartSemaphore();
            float result;
            if (to == from)
            {
                result = from;
            }
            else if (from < to)
            {
                float num = to - from;
                result = from + (float)this.random.NextDouble() * num;
            }
            else
            {
                float num2 = from - to;
                result = to + (float)this.random.NextDouble() * num2;
            }
            this.EndSemaphore();
            return result;
        }

        public double GetDouble01()
        {
            return this.random.NextDouble();
        }

        public double GetDouble_11()
        {
            return this.random.NextDouble() * 2.0 - 1.0;
        }

        public double GetDouble(double from, double to)
        {
            this.StartSemaphore();
            double result;
            if (to == from)
            {
                result = from;
            }
            else if (from < to)
            {
                double num = to - from;
                result = from + (double)(float)this.random.NextDouble() * num;
            }
            else
            {
                double num2 = from - to;
                result = to + (double)(float)this.random.NextDouble() * num2;
            }
            this.EndSemaphore();
            return result;
        }

        private void StartSemaphore()
        {
            if (this.semaphored && !MHRandom.semaphore.WaitOne(5000))
            {
                Debug.Log("[MH NOTE!] Semaphore was not freed, some thread was never finished or terminated");
            }
        }

        private void EndSemaphore()
        {
            if (this.semaphored)
            {
                MHRandom.semaphore.Release();
            }
        }
    }
}
