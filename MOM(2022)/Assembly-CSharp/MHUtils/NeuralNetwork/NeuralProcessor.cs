using System.Collections.Generic;
using UnityEngine;

namespace MHUtils.NeuralNetwork
{
    public class NeuralProcessor
    {
        public double[] outbound;

        public double[] inbound;

        public double error;

        public void ProcessViaNetwork(NeuralNetwork n, INeuralData data)
        {
            if (!n.IsValid())
            {
                Debug.LogError("invalid network!");
                return;
            }
            if (this.outbound == null || this.outbound.Length != n.GetTotalNeuron())
            {
                int totalNeuron = n.GetTotalNeuron();
                this.outbound = new double[totalNeuron];
                this.inbound = new double[totalNeuron];
            }
            double[] data2 = data.GetData();
            NeuralLayer firsLayer = n.GetFirsLayer();
            if (firsLayer.neurons.Count != data2.Length)
            {
                Debug.LogError("Network get incorrect input size  " + firsLayer.neurons.Count + " vs " + data2.Length);
                return;
            }
            for (int i = 0; i < firsLayer.neurons.Count; i++)
            {
                int num = firsLayer.neurons[i];
                this.inbound[num] = data2[i];
                n.GetNeuron(num).Squash(this);
            }
            n.Process(this);
        }

        public double GetResultSingle()
        {
            return this.outbound[this.outbound.Length - 1];
        }

        public List<double> GetResult(NeuralNetwork n)
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
            if (result == null || target == null || result.Count != target.Count)
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
            return 0.5 * (target - result) * (target - result);
        }
    }
}
