namespace MHUtils.NeuralNetwork
{
    using MHUtils;
    using ProtoBuf;
    using System;
    using UnityEngine;

    [ProtoContract]
    public class Neuron : INeuralMutagen
    {
        [ProtoMember(1)]
        public int neuronID;
        [ProtoMember(2)]
        public double bias;
        [ProtoMember(3)]
        public double biasChangeGradient;
        [ProtoMember(4)]
        public NeuralInputLink[] inputs;
        [ProtoMember(5)]
        public NeuronType neuronType;
        [ProtoMember(6)]
        public int lastMutationGen;
        [ProtoIgnore]
        private double premutationBias;

        public Neuron()
        {
        }

        public Neuron(NeuronType type, MHRandom random)
        {
            this.neuronType = type;
            this.bias = random.GetDouble_11();
            this.biasChangeGradient = random.GetDouble01();
        }

        public void ApplyMutation(double d)
        {
            this.bias += this.bias * d;
            this.biasChangeGradient = (this.bias * d) * 2.0;
        }

        public void CalculateValue(NeuralProcessor data)
        {
            double num = 0.0;
            foreach (NeuralInputLink link in this.inputs)
            {
                num += data.outbound[link.sourceNeuronID] * link.weight;
            }
            data.inbound[this.neuronID] = num + this.bias;
            this.Squash(data);
        }

        internal void CopyFrom(Neuron neuron)
        {
            if (this.neuronID != neuron.neuronID)
            {
                Debug.LogError("ID missmatch!");
            }
            else
            {
                this.bias = neuron.bias;
                this.biasChangeGradient = neuron.biasChangeGradient;
                this.neuronType = neuron.neuronType;
                this.lastMutationGen = neuron.lastMutationGen;
                if (this.inputs != null)
                {
                    for (int i = 0; i < this.inputs.Length; i++)
                    {
                        this.inputs[i].weight = neuron.inputs[i].weight;
                        this.inputs[i].weightChangeGradient = neuron.inputs[i].weightChangeGradient;
                        this.inputs[i].lastMutationGen = neuron.inputs[i].lastMutationGen;
                    }
                }
            }
        }

        internal void Evolve(double strength, MHRandom random)
        {
            double num = random.GetDouble01();
            if (num < strength)
            {
                double val = (this.biasChangeGradient + num) - (strength * 0.5);
                this.biasChangeGradient = NeuralUtils.Clamp(val, 0.1);
            }
            double num2 = this.bias + ((random.GetDouble01() * strength) * this.biasChangeGradient);
            this.bias = num2;
        }

        public int GetLastMutationGeneration()
        {
            return this.lastMutationGen;
        }

        public void InitializeConnection(NeuralLayer layer, MHRandom random)
        {
            this.inputs = new NeuralInputLink[layer.neurons.Count];
            for (int i = 0; i < layer.neurons.Count; i++)
            {
                this.inputs[i] = new NeuralInputLink();
                this.inputs[i].sourceNeuronID = layer.neurons[i];
                this.inputs[i].weight = random.GetDouble_11();
                this.inputs[i].weightChangeGradient = random.GetDouble01();
            }
        }

        private double LeakyReLU(double nodeValue)
        {
            return ((nodeValue <= 1.0) ? ((nodeValue < 0.0) ? (((nodeValue * 0.01) >= -1.0) ? (nodeValue * 0.01) : -1.0) : nodeValue) : 1.0);
        }

        private double Linear(double nodeValue)
        {
            return ((nodeValue <= 1.0) ? ((nodeValue >= -1.0) ? nodeValue : -1.0) : 1.0);
        }

        public void Mutate(double value)
        {
            this.premutationBias = this.bias;
            this.bias += this.bias * value;
        }

        public void RestoreFromMutation()
        {
            this.bias = this.premutationBias;
        }

        public void SetLastMutationGeneration(int gen)
        {
            this.lastMutationGen = gen;
        }

        private double Sigmoid(double nodeValue)
        {
            return (1.0 / (1.0 + Math.Exp(-nodeValue)));
        }

        public void Squash(NeuralProcessor data)
        {
            double nodeValue = data.inbound[this.neuronID];
            if (this.neuronType == NeuronType.Linear)
            {
                data.outbound[this.neuronID] = this.Linear(nodeValue);
            }
            else if (this.neuronType == NeuronType.Sigmoid)
            {
                data.outbound[this.neuronID] = this.Sigmoid(nodeValue);
            }
            else if (this.neuronType == NeuronType.TanH)
            {
                data.outbound[this.neuronID] = this.TanH(nodeValue);
            }
            else if (this.neuronType == NeuronType.LeakyReLU)
            {
                data.outbound[this.neuronID] = this.LeakyReLU(nodeValue);
            }
        }

        private double TanH(double nodeValue)
        {
            return Math.Tanh(nodeValue);
        }

        public double UnSquash(double d)
        {
            return (double) Mathf.Log((float) (d / (1.0 - d)));
        }

        public enum NeuronType
        {
            Linear,
            LeakyReLU,
            Sigmoid,
            TanH
        }
    }
}

