using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MHUtils.NeuralNetwork.PowerEstimation2
{
    public class NeuralNetThreadTask : ITask
    {
        private const int itemsPerRequest = 10;

        public int index;

        public int stepSize;

        public NNEnvironment environment;

        private NeuralProcessor processor = new NeuralProcessor();

        private MHRandom random;

        private MHTimer timer;

        public object Execute()
        {
            DateTime now = DateTime.Now;
            this.random = new MHRandom(this.index * 500 + now.Millisecond);
            int num = -1;
            double num2 = 0.0;
            while (!this.environment.stopThreading)
            {
                if (this.environment.running)
                {
                    if (num != this.environment.generationToProcess)
                    {
                        Interlocked.Decrement(ref this.environment.idleTasks);
                        num = this.environment.generationToProcess;
                        if (num == -1)
                        {
                            continue;
                        }
                        if (this.index == 0)
                        {
                            num2 = this.environment.network.GetErrorValue();
                        }
                        NNUnit[] units = this.environment.units;
                        List<NeuralNetwork> generation = this.environment.generation;
                        int num3 = Mathf.CeilToInt((float)generation.Count * this.environment.survivalShare);
                        while (true)
                        {
                            int num4 = Interlocked.Add(ref this.environment.taskListIndex, -10);
                            int num5 = num4 + 10;
                            if (num5 <= 0)
                            {
                                break;
                            }
                            int num6 = num5 - 1;
                            while (num6 >= 0 && num6 >= num4)
                            {
                                bool flag = false;
                                NeuralNetwork neuralNetwork = generation[num6];
                                if (num6 >= num3)
                                {
                                    int num7 = (num6 - num3) % num3;
                                    if ((double)num6 > (double)generation.Count * 0.9)
                                    {
                                        num7 = 0;
                                        flag = true;
                                    }
                                    NeuralNetwork nn = generation[num7];
                                    neuralNetwork.CopyFrom(nn);
                                }
                                if (flag)
                                {
                                    if (!neuralNetwork.MutateOnce(this.processor, units, this.random))
                                    {
                                        neuralNetwork.Evolve(this.environment.evolutionStrength * (float)num6 / (float)generation.Count, this.random);
                                    }
                                    else
                                    {
                                        neuralNetwork.MutateOnce(this.processor, units, this.random);
                                        neuralNetwork.MutateOnce(this.processor, units, this.random);
                                    }
                                }
                                else
                                {
                                    neuralNetwork.Evolve(this.environment.evolutionStrength * (float)num6 / (float)generation.Count, this.random);
                                }
                                neuralNetwork.ProcessViaNetwork(this.processor, units, recordResults: true);
                                neuralNetwork.generation++;
                                num6--;
                            }
                        }
                        Interlocked.Increment(ref this.environment.idleTasks);
                        Interlocked.Increment(ref this.environment.generationTasksDone);
                        continue;
                    }
                    if (this.index == 0 && this.environment.idleTasks == this.stepSize && this.environment.generationTasksDone == this.stepSize)
                    {
                        if (this.timer == null)
                        {
                            this.timer = MHTimer.StartNew();
                        }
                        float time = this.timer.GetTime();
                        this.environment.generation.Sort((NeuralNetwork a, NeuralNetwork b) => a.GetErrorValue().CompareTo(b.GetErrorValue()));
                        float time2 = this.timer.GetTime();
                        this.environment.network = this.environment.generation[0];
                        if (num2 != this.environment.network.GetErrorValue())
                        {
                            double valueOrDefault = (this.environment?.network?.GetErrorValue()).GetValueOrDefault();
                            if (num2 != valueOrDefault)
                            {
                                NeuralNetwork network = this.environment.network;
                                double valueOrDefault2 = (this.environment?.network?.maxError).GetValueOrDefault();
                                double valueOrDefault3 = (this.environment?.network?.avError).GetValueOrDefault();
                                num2 = valueOrDefault;
                                PowerEstimation2Manager.SaveCurentNetwork(network);
                                float time3 = this.timer.GetTime();
                                Debug.Log($"Generation {network.generation} have averageError: {valueOrDefault3}, maximum error: {valueOrDefault2}" + $"\n Sorting: {time2 - time}, Saving: {time3 - time2}");
                            }
                        }
                        if ((this.environment?.network?.generation).GetValueOrDefault() % 10 == 0 && this.environment.running)
                        {
                            Debug.Log("Network evolving generation " + this.environment.network.generation + " current error " + this.environment.network.GetErrorValue());
                        }
                        if (this.environment.stopAtGeneration > this.environment.network.generation)
                        {
                            this.environment.generationToProcess = this.environment.network.generation;
                            this.environment.taskListIndex = this.environment.generation.Count;
                        }
                        else
                        {
                            this.environment.running = false;
                            Debug.Log("Network evolving stopped at generation " + this.environment.network.generation + " current error " + this.environment.network.GetErrorValue());
                        }
                        this.environment.generationTasksDone = 0;
                    }
                    Thread.Sleep(1);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
            return null;
        }
    }
}
