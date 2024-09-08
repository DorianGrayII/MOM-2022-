namespace MHUtils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    public class MHRandom
    {
        private static Semaphore semaphore;
        private static MHRandom singleton;
        public static int seedSource;
        private System.Random random;
        private const int day = 0x5265c00;
        private const int hour = 0x36ee80;
        private const int minutes = 0xea60;
        private const int seconds = 0x3e8;
        private bool semaphored;

        public MHRandom(System.Random r)
        {
            this.random = r;
        }

        public MHRandom(int seed, bool unsourced)
        {
            if (seed == 0)
            {
                if (unsourced)
                {
                    seed = ((DateTime.Now.Minute * 0x186a0) + (DateTime.Now.Second * 0x3e8)) + DateTime.Now.Millisecond;
                }
                else if (seedSource != 0)
                {
                    seedSource++;
                    seed = seedSource;
                }
                else
                {
                    seedSource = ((DateTime.Now.Minute * 0x186a0) + (DateTime.Now.Second * 0x3e8)) + DateTime.Now.Millisecond;
                    seedSource++;
                    seed = seedSource;
                }
            }
            this.random = new System.Random(seed);
        }

        private void EndSemaphore()
        {
            if (this.semaphored)
            {
                semaphore.Release();
            }
        }

        public static MHRandom Get()
        {
            if (singleton == null)
            {
                singleton = new MHRandom(0, false);
                singleton.semaphored = true;
            }
            if (semaphore == null)
               semaphore = new Semaphore(1, 1);

            return singleton;
        }

        public double GetDouble(double from, double to)
        {
            double num;
            this.StartSemaphore();
            if (to == from)
            {
                num = from;
            }
            else if (from < to)
            {
                double num2 = to - from;
                num = from + (((float) this.random.NextDouble()) * num2);
            }
            else
            {
                double num3 = from - to;
                num = to + (((float) this.random.NextDouble()) * num3);
            }
            this.EndSemaphore();
            return num;
        }

        public double GetDouble_11()
        {
            return ((this.random.NextDouble() * 2.0) - 1.0);
        }

        public double GetDouble01()
        {
            return this.random.NextDouble();
        }

        public float GetFloat(float from, float to)
        {
            float num;
            this.StartSemaphore();
            if (to == from)
            {
                num = from;
            }
            else if (from < to)
            {
                float num2 = to - from;
                num = from + (((float) this.random.NextDouble()) * num2);
            }
            else
            {
                float num3 = from - to;
                num = to + (((float) this.random.NextDouble()) * num3);
            }
            this.EndSemaphore();
            return num;
        }

        public int GetInt(int from, int to)
        {
            this.StartSemaphore();
            int num = (to != from) ? ((to >= from) ? this.random.Next(from, to) : this.random.Next(to, from)) : to;
            this.EndSemaphore();
            return num;
        }

        public int GetIntMinMax()
        {
            return this.GetInt(-2147483648, 0x7fffffff);
        }

        public int GetSuccesses(float chance, int tryCount)
        {
            int num = 0;
            this.StartSemaphore();
            for (int i = 0; i < tryCount; i++)
            {
                if (this.random.NextDouble() < chance)
                {
                    num++;
                }
            }
            this.EndSemaphore();
            return num;
        }

        private void StartSemaphore()
        {
            if (this.semaphored && !semaphore.WaitOne(0x1388))
            {
                Debug.Log("[MH NOTE!] Semaphore was not freed, some thread was never finished or terminated");
            }
        }
    }
}

