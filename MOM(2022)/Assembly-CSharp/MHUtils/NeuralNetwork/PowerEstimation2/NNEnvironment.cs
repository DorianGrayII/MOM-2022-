namespace MHUtils.NeuralNetwork.PowerEstimation2
{
    using MHUtils;
    using MHUtils.NeuralNetwork;
    using MOM;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using UnityEngine;

    public class NNEnvironment
    {
        public MHUtils.NeuralNetwork.NeuralNetwork network;
        public int stopAtGeneration;
        public NNUnit[] units;
        public List<MHUtils.NeuralNetwork.NeuralNetwork> generation;
        public List<LogResults> logResults = new List<LogResults>();
        private MHRandom random;
        public bool running;
        public int generationToProcess;
        private MHThread thread;
        private NeuralNetThreadTask[] tasks;
        public float survivalShare;
        public float evolutionStrength;
        public bool stopThreading;
        public int generationTasksDone;
        public int idleTasks;
        public int taskListIndex;

        public void Destroy()
        {
            this.StopThreading();
        }

        internal int GetGeneration()
        {
            return ((this.network != null) ? this.network.generation : 0x7fffffff);
        }

        internal void ResetEnvironment(PowerEstimation2Manager source)
        {
            int generationSize = source.generationSize;
            float evoShare = source.evoShare;
            float evoStrength = source.evoStrength;
            int threadCount = source.threadCount;
            this.StopThreading();
            this.thread = null;
            this.survivalShare = evoShare;
            this.evolutionStrength = evoStrength;
            this.random = new MHRandom(UnityEngine.Random.Range(-2147483648, 0x7fffffff), false);
            this.logResults.Clear();
            List<Multitype<BattleUnit, int>> powerValues = PowerEstimate.powerValues;
            this.units = new NNUnit[powerValues.Count];
            for (int i = 0; i < powerValues.Count; i++)
            {
                this.units[i] = new NNUnit(powerValues[i].t0, powerValues[i].t1);
            }
            this.generation = new List<MHUtils.NeuralNetwork.NeuralNetwork>(generationSize);
            MemoryStream destination = new MemoryStream();
            Serializer.Serialize<MHUtils.NeuralNetwork.NeuralNetwork>(destination, this.network);
            for (int j = 0; j < generationSize; j++)
            {
                destination.Position = 0L;
                MHUtils.NeuralNetwork.NeuralNetwork item = Serializer.Deserialize<MHUtils.NeuralNetwork.NeuralNetwork>(destination);
                this.generation.Add(item);
            }
            destination.Dispose();
            this.StartThreading(threadCount);
            Debug.Log(string.Format("Reset to network: {0} Neurons, {1} Layers, Model: {2}, Activator: {3} ", new object[] { this.network.GetTotalNeuron(), this.network.neuralLayers.Count, this.network.model, this.network.nType }));
        }

        internal void StartProcessing()
        {
            if ((this.network != null) && (this.network.generation < this.stopAtGeneration))
            {
                this.taskListIndex = this.generation.Count;
                this.running = true;
                this.generationToProcess = this.network.generation;
            }
        }

        private void StartThreading(int threadCount)
        {
            this.generationToProcess = -1;
            this.generationTasksDone = 0;
            this.idleTasks = threadCount;
            this.tasks = new NeuralNetThreadTask[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                this.tasks[i] = new NeuralNetThreadTask();
                this.tasks[i].index = i;
                this.tasks[i].stepSize = threadCount;
                this.tasks[i].environment = this;
            }
            this.stopThreading = false;
            this.thread = MHThread.Create(this.tasks, null, null);
        }

        internal void StopAtGeneration(int x)
        {
            this.stopAtGeneration = x;
            this.StartProcessing();
        }

        public void StopProcessing()
        {
            this.stopAtGeneration = -1;
            while ((this.thread != null) && ((this.generationTasksDone != 0) || (this.idleTasks != this.tasks.Length)))
            {
                Thread.Sleep(5);
            }
        }

        public void StopThreading()
        {
            this.StopProcessing();
            this.stopThreading = true;
            while ((this.thread != null) && !this.thread.IsFinished())
            {
                Thread.Sleep(5);
            }
        }
    }
}

