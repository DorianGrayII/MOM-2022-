namespace MHUtils.NeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NeuralProcessor
    {
        public double[] outbound;
        public double[] inbound;
        public double error;

        public List<double> GetResult(MHUtils.NeuralNetwork.NeuralNetwork n)
        {
            NeuralLayer finalLayer = n.GetFinalLayer();
            List<double> list = new List<double>(finalLayer.neurons.Count);
            for (int i = 0; i < finalLayer.neurons.Count; i++)
            {
                list.Add(this.outbound[finalLayer.neurons[i]]);
            }
            return list;
        }

        public double GetResultError(List<double> result, List<double> target)
        {
            if ((result == null) || ((target == null) || (result.Count != target.Count)))
            {
                return 0.0;
            }
            double num = 0.0;
            for (int i = 0; i < result.Count; i++)
            {
                num += this.GetResultError(result[i], target[i]);
            }
            return num;
        }

        public double GetResultError(double result, double target)
        {
            return ((0.5 * (target - result)) * (target - result));
        }

        public double GetResultSingle()
        {
            return this.outbound[this.outbound.Length - 1];
        }

        public void ProcessViaNetwork(MHUtils.NeuralNetwork.NeuralNetwork n, INeuralData data)
        {
            if (!n.IsValid())
            {
                Debug.LogError("invalid network!");
            }
            else
            {
                if ((this.outbound == null) || (this.outbound.Length != n.GetTotalNeuron()))
                {
                    int totalNeuron = n.GetTotalNeuron();
                    this.outbound = new double[totalNeuron];
                    this.inbound = new double[totalNeuron];
                }
                double[] numArray = data.GetData();
                NeuralLayer firsLayer = n.GetFirsLayer();
                if (firsLayer.neurons.Count != numArray.Length)
                {
                    Debug.LogError("Network get incorrect input size  " + firsLayer.neurons.Count.ToString() + " vs " + numArray.Length.ToString());
                }
                else
                {
                    for (int i = 0; i < firsLayer.neurons.Count; i++)
                    {
                        int index = firsLayer.neurons[i];
                        this.inbound[index] = numArray[i];
                        n.GetNeuron(index).Squash(this);
                    }
                    n.Process(this);
                }
            }
        }
    }
}

