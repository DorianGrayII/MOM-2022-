using System.Collections.Generic;
using MHUtils.NeuralNetwork.PowerEstimation2;
using ProtoBuf;
using UnityEngine;

namespace MHUtils.NeuralNetwork
{
    [ProtoContract]
    public class NeuralNetwork
    {
        public enum Model
        {
            FF_Lerp_1 = 1,
            DFF_Lerp_2 = 2,
            DFF_Lerp_3 = 3,
            DFF_Lerp_4 = 4,
            DFF_Flat_4 = 14,
            DFF_Flat_5 = 15,
            DFF_Sparse_5 = 25,
            DFF_Sparse_7 = 27,
            DFF_Wide_Sparse_6 = 36,
            DFF_Wide_Sparse_8 = 38,
            DFF_Sparse_Lerp_3 = 43,
            DFF_Sparse_Lerp_4 = 44,
            DFF_10x10 = 45,
            DFF_5x7x5 = 46
        }

        [ProtoMember(1)]
        public List<Neuron> neurons;

        [ProtoMember(2)]
        public List<NeuralLayer> neuralLayers = new List<NeuralLayer>();

        [ProtoMember(3)]
        public int generation;

        [ProtoMember(4)]
        public int uniqueID;

        [ProtoMember(5)]
        public double avError;

        [ProtoMember(6)]
        public double maxError;

        [ProtoMember(7)]
        public Model model;

        [ProtoMember(8)]
        public Neuron.NeuronType nType;

        public bool IsValid()
        {
            if (this.neurons != null && this.neurons.Count > 0)
            {
                return this.neuralLayers.Count > 2;
            }
            return false;
        }

        public NeuralLayer GetFirsLayer()
        {
            return this.neuralLayers[0];
        }

        public NeuralLayer GetFinalLayer()
        {
            return this.neuralLayers[this.neuralLayers.Count - 1];
        }

        public void Process(NeuralProcessor data)
        {
            for (int i = 1; i < this.neuralLayers.Count; i++)
            {
                NeuralLayer neuralLayer = this.neuralLayers[i];
                for (int j = 0; j < neuralLayer.neurons.Count; j++)
                {
                    this.GetNeuron(neuralLayer.neurons[j]).CalculateValue(data);
                }
            }
        }

        public void InitializeBlank(int inputSize, int outputSize, Model model, Neuron.NeuronType nType)
        {
            int[] array;
            switch (model)
            {
            case Model.FF_Lerp_1:
            case Model.DFF_Lerp_2:
            case Model.DFF_Lerp_3:
            case Model.DFF_Lerp_4:
            {
                array = new int[(int)model];
                for (int j = 0; j < (int)model; j++)
                {
                    float t2 = (float)(j + 1) / (float)(model + 1);
                    array[j] = Mathf.CeilToInt(Mathf.Lerp(inputSize, outputSize, t2));
                }
                break;
            }
            case Model.DFF_Flat_4:
            case Model.DFF_Flat_5:
            {
                int num3 = (int)(model - 10);
                array = new int[num3];
                for (int l = 0; l < num3; l++)
                {
                    array[l] = inputSize;
                }
                break;
            }
            case Model.DFF_Sparse_5:
            case Model.DFF_Sparse_7:
            {
                int num4 = (int)(model - 20);
                array = new int[num4];
                for (int m = 0; m < num4; m++)
                {
                    array[m] = Mathf.RoundToInt((float)inputSize * 1.3f);
                }
                break;
            }
            case Model.DFF_Wide_Sparse_6:
            case Model.DFF_Wide_Sparse_8:
            {
                int num2 = (int)(model - 30);
                array = new int[num2];
                for (int k = 0; k < num2; k++)
                {
                    array[k] = Mathf.RoundToInt((float)inputSize * 1.5f);
                }
                break;
            }
            case Model.DFF_Sparse_Lerp_3:
            case Model.DFF_Sparse_Lerp_4:
            {
                int num = (int)(model - 40);
                array = new int[num];
                for (int i = 0; i < num; i++)
                {
                    float t = (float)(i + 1) / (float)(num + 1);
                    array[i] = Mathf.CeilToInt(Mathf.Lerp((int)((float)inputSize * 1.3f), outputSize, t));
                }
                break;
            }
            case Model.DFF_10x10:
                array = new int[2] { 10, 10 };
                break;
            case Model.DFF_5x7x5:
                array = new int[3] { 5, 7, 5 };
                break;
            default:
                array = new int[1] { 5 };
                break;
            }
            this.InitializeBlank(inputSize, outputSize, array, nType);
            this.model = model;
            this.nType = nType;
        }

        public void InitializeBlank(int inputSize, int outputSize, int[] hiddenLayers, Neuron.NeuronType nType)
        {
            int num = 0;
            MHRandom random = MHRandom.Get();
            this.neurons = new List<Neuron>();
            NeuralLayer neuralLayer = new NeuralLayer(inputSize);
            this.neuralLayers.Add(neuralLayer);
            for (int i = 0; i < inputSize; i++)
            {
                Neuron neuron = new Neuron(nType, random);
                neuron.neuronID = num;
                num++;
                this.neurons.Add(neuron);
                neuralLayer.neurons.Add(neuron.neuronID);
            }
            NeuralLayer layer;
            for (int j = 1; j <= hiddenLayers.Length; j++)
            {
                layer = neuralLayer;
                neuralLayer = new NeuralLayer(hiddenLayers[j - 1]);
                this.neuralLayers.Add(neuralLayer);
                for (int k = 0; k < hiddenLayers[j - 1]; k++)
                {
                    Neuron neuron2 = new Neuron(nType, random);
                    neuron2.neuronID = num;
                    num++;
                    this.neurons.Add(neuron2);
                    neuralLayer.neurons.Add(neuron2.neuronID);
                    neuron2.InitializeConnection(layer, random);
                }
            }
            layer = neuralLayer;
            neuralLayer = new NeuralLayer(outputSize);
            this.neuralLayers.Add(neuralLayer);
            for (int l = 0; l < outputSize; l++)
            {
                Neuron neuron3 = new Neuron(nType, random);
                neuron3.neuronID = num;
                num++;
                this.neurons.Add(neuron3);
                neuralLayer.neurons.Add(neuron3.neuronID);
                neuron3.InitializeConnection(layer, random);
            }
        }

        public void Evolve(double strength, MHRandom random)
        {
            foreach (Neuron neuron in this.neurons)
            {
                if (neuron.inputs != null)
                {
                    NeuralInputLink[] inputs = neuron.inputs;
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        inputs[i].Evolve(strength, random);
                    }
                    neuron.Evolve(strength, random);
                }
            }
        }

        public bool MutateOnce(NeuralProcessor processor, NNUnit[] units, MHRandom random)
        {
            INeuralMutagen neuralMutagen = null;
            for (int i = 0; i < 10; i++)
            {
                int @int = random.GetInt(1, this.neuralLayers.Count);
                NeuralLayer neuralLayer = this.neuralLayers[@int];
                int int2 = random.GetInt(0, neuralLayer.neurons.Count);
                Neuron neuron = this.GetNeuron(neuralLayer.neurons[int2]);
                if (random.GetDouble01() < 0.8)
                {
                    int int3 = random.GetInt(0, neuron.inputs.Length);
                    NeuralInputLink neuralInputLink = neuron.inputs[int3];
                    if (neuralMutagen == null || neuralMutagen.GetLastMutationGeneration() > neuralInputLink.GetLastMutationGeneration())
                    {
                        neuralMutagen = neuralInputLink;
                    }
                }
                else if (neuralMutagen == null || neuralMutagen.GetLastMutationGeneration() > neuron.GetLastMutationGeneration())
                {
                    neuralMutagen = neuron;
                }
            }
            neuralMutagen.SetLastMutationGeneration(this.generation);
            return this.Mutation(neuralMutagen, processor, units);
        }

        public double ProcessViaNetwork(NeuralProcessor processor, NNUnit[] units, bool recordResults)
        {
            double num = 0.0;
            double num2 = 0.0;
            foreach (NNUnit nNUnit in units)
            {
                processor.ProcessViaNetwork(this, nNUnit);
                double num3 = (double)Mathf.Abs((float)(processor.GetResultSingle() - nNUnit.targetValue)) / nNUnit.targetValue;
                num2 += num3;
                if (num3 > num)
                {
                    num = num3;
                }
            }
            double num4 = num2 / (double)units.Length;
            if (recordResults)
            {
                this.avError = num4;
                this.maxError = num;
            }
            return NeuralNetwork.CalculateError(this.avError, num);
        }

        private bool Mutation(INeuralMutagen mutant, NeuralProcessor processor, NNUnit[] units)
        {
            double num = 10.0;
            for (int i = 0; i < 10; i++)
            {
                mutant.Mutate(num);
                double num2 = this.ProcessViaNetwork(processor, units, recordResults: false);
                mutant.RestoreFromMutation();
                if (num2 < this.GetErrorValue())
                {
                    mutant.ApplyMutation(num);
                    return true;
                }
                mutant.Mutate(0.0 - num);
                double num3 = this.ProcessViaNetwork(processor, units, recordResults: false);
                mutant.RestoreFromMutation();
                if (num3 < this.GetErrorValue())
                {
                    mutant.ApplyMutation(0.0 - num);
                    return true;
                }
                num *= 0.1;
            }
            return false;
        }

        internal void CopyFrom(NeuralNetwork nn)
        {
            for (int i = 0; i < this.neurons.Count; i++)
            {
                this.neurons[i].CopyFrom(nn.neurons[i]);
            }
            this.generation = nn.generation;
            this.uniqueID = nn.uniqueID;
            this.avError = nn.avError;
            this.maxError = nn.maxError;
        }

        private double GetRandom(MHRandom r)
        {
            return r.GetDouble01() * 2.0 - 1.0;
        }

        private double Clamp(double val)
        {
            if (val > 1.0)
            {
                return 1.0;
            }
            if (val < -1.0)
            {
                return -1.0;
            }
            return val;
        }

        public string GetID()
        {
            return this.uniqueID + "." + this.generation;
        }

        public Neuron GetNeuron(int id)
        {
            return this.neurons[id];
        }

        public int GetTotalNeuron()
        {
            return this.neurons.Count;
        }

        public double GetErrorValue()
        {
            return NeuralNetwork.CalculateError(this.avError, this.maxError);
        }

        private static double CalculateError(double aE, double mE)
        {
            return aE + 0.5 * mE;
        }
    }
}
