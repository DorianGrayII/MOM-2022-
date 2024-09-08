using System.Collections.Generic;
using System.IO;
using System.Threading;
using MOM;
using ProtoBuf;
using UnityEngine;

namespace MHUtils.NeuralNetwork.PowerEstimation2
{
    public class NNEnvironment
    {
        public NeuralNetwork network;

        public int stopAtGeneration;

        public NNUnit[] units;

        public List<NeuralNetwork> generation;

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
            ITask[] array = this.tasks;
            this.thread = MHThread.Create(array);
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
            this.random = new MHRandom(Random.Range(int.MinValue, int.MaxValue));
            this.logResults.Clear();
            List<Multitype<BattleUnit, int>> powerValues = PowerEstimate.powerValues;
            this.units = new NNUnit[powerValues.Count];
            for (int i = 0; i < powerValues.Count; i++)
            {
                this.units[i] = new NNUnit(powerValues[i].t0, powerValues[i].t1);
            }
            this.generation = new List<NeuralNetwork>(generationSize);
            MemoryStream memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, this.network);
            for (int j = 0; j < generationSize; j++)
            {
                memoryStream.Position = 0L;
                NeuralNetwork item = Serializer.Deserialize<NeuralNetwork>(memoryStream);
                this.generation.Add(item);
            }
            memoryStream.Dispose();
            this.StartThreading(threadCount);
            Debug.Log($"Reset to network: {this.network.GetTotalNeuron()} Neurons, {this.network.neuralLayers.Count} Layers, Model: {this.network.model}, Activator: {this.network.nType} ");
        }

        public void Destroy()
        {
            this.StopThreading();
        }

        internal int GetGeneration()
        {
            if (this.network == null)
            {
                return int.MaxValue;
            }
            return this.network.generation;
        }

        internal void StartProcessing()
        {
            if (this.network != null && this.network.generation < this.stopAtGeneration)
            {
                this.taskListIndex = this.generation.Count;
                this.running = true;
                this.generationToProcess = this.network.generation;
            }
        }

        public void StopProcessing()
        {
            this.stopAtGeneration = -1;
            while (this.thread != null && (this.generationTasksDone != 0 || this.idleTasks != this.tasks.Length))
            {
                Thread.Sleep(5);
            }
        }

        public void StopThreading()
        {
            this.StopProcessing();
            this.stopThreading = true;
            while (this.thread != null && !this.thread.IsFinished())
            {
                Thread.Sleep(5);
            }
        }

        internal void StopAtGeneration(int x)
        {
            this.stopAtGeneration = x;
            this.StartProcessing();
        }
    }
}
