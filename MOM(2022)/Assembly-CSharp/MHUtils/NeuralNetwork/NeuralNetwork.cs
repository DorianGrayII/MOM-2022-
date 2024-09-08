namespace MHUtils.NeuralNetwork
{
    using MHUtils;
    using MHUtils.NeuralNetwork.PowerEstimation2;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ProtoContract]
    public class NeuralNetwork
    {
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

        private static double CalculateError(double aE, double mE)
        {
            return (aE + (0.5 * mE));
        }

        private double Clamp(double val)
        {
            return ((val <= 1.0) ? ((val >= -1.0) ? val : -1.0) : 1.0);
        }

        internal void CopyFrom(MHUtils.NeuralNetwork.NeuralNetwork nn)
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

        public void Evolve(double strength, MHRandom random)
        {
            foreach (Neuron neuron in this.neurons)
            {
                if (neuron.inputs != null)
                {
                    NeuralInputLink[] inputs = neuron.inputs;
                    int index = 0;
                    while (true)
                    {
                        if (index >= inputs.Length)
                        {
                            neuron.Evolve(strength, random);
                            break;
                        }
                        inputs[index].Evolve(strength, random);
                        index++;
                    }
                }
            }
        }

        public double GetErrorValue()
        {
            return CalculateError(this.avError, this.maxError);
        }

        public NeuralLayer GetFinalLayer()
        {
            return this.neuralLayers[this.neuralLayers.Count - 1];
        }

        public NeuralLayer GetFirsLayer()
        {
            return this.neuralLayers[0];
        }

        public string GetID()
        {
            return (this.uniqueID.ToString() + "." + this.generation.ToString());
        }

        public Neuron GetNeuron(int id)
        {
            return this.neurons[id];
        }

        private double GetRandom(MHRandom r)
        {
            return ((r.GetDouble01() * 2.0) - 1.0);
        }

        public int GetTotalNeuron()
        {
            return this.neurons.Count;
        }

        public void InitializeBlank(int inputSize, int outputSize, Model model, Neuron.NeuronType nType)
        {
            int[] numArray;
            if (model > Model.DFF_Flat_5)
            {
                if ((model == Model.DFF_Sparse_5) || (model == Model.DFF_Sparse_7))
                {
                    int num6 = (int) (model - ((Model) 20));
                    numArray = new int[num6];
                    for (int i = 0; i < num6; i++)
                    {
                        numArray[i] = Mathf.RoundToInt(inputSize * 1.3f);
                    }
                    goto TR_0000;
                }
                else
                {
                    switch (model)
                    {
                        case Model.DFF_Wide_Sparse_6:
                        case Model.DFF_Wide_Sparse_8:
                        {
                            int num8 = (int) (model - ((Model) 30));
                            numArray = new int[num8];
                            for (int i = 0; i < num8; i++)
                            {
                                numArray[i] = Mathf.RoundToInt(inputSize * 1.5f);
                            }
                            goto TR_0000;
                        }
                        case Model.DFF_Sparse_Lerp_3:
                        case Model.DFF_Sparse_Lerp_4:
                        {
                            int num10 = (int) (model - ((Model) 40));
                            numArray = new int[num10];
                            for (int i = 0; i < num10; i++)
                            {
                                float t = ((float) (i + 1)) / ((float) (num10 + 1));
                                numArray[i] = Mathf.CeilToInt(Mathf.Lerp((float) ((int) (inputSize * 1.3f)), (float) outputSize, t));
                            }
                            goto TR_0000;
                        }
                        case Model.DFF_10x10:
                            numArray = new int[] { 10, 10 };
                            goto TR_0000;

                        case Model.DFF_5x7x5:
                            numArray = new int[] { 5, 7, 5 };
                            goto TR_0000;

                        default:
                            break;
                    }
                }
            }
            else if ((model - 1) <= Model.DFF_Lerp_3)
            {
                int num = (int) model;
                numArray = new int[num];
                for (int i = 0; i < num; i++)
                {
                    float t = ((float) (i + 1)) / ((float) (num + 1));
                    numArray[i] = Mathf.CeilToInt(Mathf.Lerp((float) inputSize, (float) outputSize, t));
                }
                goto TR_0000;
            }
            else if ((model - Model.DFF_Flat_4) <= Model.FF_Lerp_1)
            {
                int num4 = (int) (model - ((Model) 10));
                numArray = new int[num4];
                for (int i = 0; i < num4; i++)
                {
                    numArray[i] = inputSize;
                }
                goto TR_0000;
            }
            numArray = new int[] { 5 };
        TR_0000:
            this.InitializeBlank(inputSize, outputSize, numArray, nType);
            this.model = model;
            this.nType = nType;
        }

        public void InitializeBlank(int inputSize, int outputSize, int[] hiddenLayers, Neuron.NeuronType nType)
        {
            NeuralLayer layer2;
            int num = 0;
            MHRandom random = MHRandom.Get();
            this.neurons = new List<Neuron>();
            NeuralLayer item = new NeuralLayer(inputSize);
            this.neuralLayers.Add(item);
            for (int i = 0; i < inputSize; i++)
            {
                Neuron neuron = new Neuron(nType, random) {
                    neuronID = num
                };
                num++;
                this.neurons.Add(neuron);
                item.neurons.Add(neuron.neuronID);
            }
            int num3 = 1;
            while (num3 <= hiddenLayers.Length)
            {
                layer2 = item;
                item = new NeuralLayer(hiddenLayers[num3 - 1]);
                this.neuralLayers.Add(item);
                int num4 = 0;
                while (true)
                {
                    if (num4 >= hiddenLayers[num3 - 1])
                    {
                        num3++;
                        break;
                    }
                    Neuron neuron2 = new Neuron(nType, random) {
                        neuronID = num
                    };
                    num++;
                    this.neurons.Add(neuron2);
                    item.neurons.Add(neuron2.neuronID);
                    neuron2.InitializeConnection(layer2, random);
                    num4++;
                }
            }
            layer2 = item;
            item = new NeuralLayer(outputSize);
            this.neuralLayers.Add(item);
            for (int j = 0; j < outputSize; j++)
            {
                Neuron neuron3 = new Neuron(nType, random) {
                    neuronID = num
                };
                num++;
                this.neurons.Add(neuron3);
                item.neurons.Add(neuron3.neuronID);
                neuron3.InitializeConnection(layer2, random);
            }
        }

        public bool IsValid()
        {
            return ((this.neurons != null) && ((this.neurons.Count > 0) && (this.neuralLayers.Count > 2)));
        }

        public bool MutateOnce(NeuralProcessor processor, NNUnit[] units, MHRandom random)
        {
            INeuralMutagen mutant = null;
            for (int i = 0; i < 10; i++)
            {
                int @int = random.GetInt(1, this.neuralLayers.Count);
                NeuralLayer layer = this.neuralLayers[@int];
                int num3 = random.GetInt(0, layer.neurons.Count);
                Neuron neuron = this.GetNeuron(layer.neurons[num3]);
                if (random.GetDouble01() >= 0.8)
                {
                    if ((mutant == null) || (mutant.GetLastMutationGeneration() > neuron.GetLastMutationGeneration()))
                    {
                        mutant = neuron;
                    }
                }
                else
                {
                    int index = random.GetInt(0, neuron.inputs.Length);
                    NeuralInputLink link = neuron.inputs[index];
                    if ((mutant == null) || (mutant.GetLastMutationGeneration() > link.GetLastMutationGeneration()))
                    {
                        mutant = link;
                    }
                }
            }
            mutant.SetLastMutationGeneration(this.generation);
            return this.Mutation(mutant, processor, units);
        }

        private bool Mutation(INeuralMutagen mutant, NeuralProcessor processor, NNUnit[] units)
        {
            double num = 10.0;
            for (int i = 0; i < 10; i++)
            {
                mutant.Mutate(num);
                mutant.RestoreFromMutation();
                if (this.ProcessViaNetwork(processor, units, false) < this.GetErrorValue())
                {
                    mutant.ApplyMutation(num);
                    return true;
                }
                mutant.Mutate(-num);
                mutant.RestoreFromMutation();
                if (this.ProcessViaNetwork(processor, units, false) < this.GetErrorValue())
                {
                    mutant.ApplyMutation(-num);
                    return true;
                }
                num *= 0.1;
            }
            return false;
        }

        public void Process(NeuralProcessor data)
        {
            int num = 1;
            while (num < this.neuralLayers.Count)
            {
                NeuralLayer layer = this.neuralLayers[num];
                int num2 = 0;
                while (true)
                {
                    if (num2 >= layer.neurons.Count)
                    {
                        num++;
                        break;
                    }
                    this.GetNeuron(layer.neurons[num2]).CalculateValue(data);
                    num2++;
                }
            }
        }

        public double ProcessViaNetwork(NeuralProcessor processor, NNUnit[] units, bool recordResults)
        {
            double mE = 0.0;
            double num2 = 0.0;
            for (int i = 0; i < units.Length; i++)
            {
                NNUnit data = units[i];
                processor.ProcessViaNetwork(this, data);
                double num5 = ((double) Mathf.Abs((float) (processor.GetResultSingle() - data.targetValue))) / data.targetValue;
                num2 += num5;
                if (num5 > mE)
                {
                    mE = num5;
                }
            }
            double num3 = num2 / ((double) units.Length);
            if (recordResults)
            {
                this.avError = num3;
                this.maxError = mE;
            }
            return CalculateError(this.avError, mE);
        }

        public enum Model
        {
            FF_Lerp_1 = 1,
            DFF_Lerp_2 = 2,
            DFF_Lerp_3 = 3,
            DFF_Lerp_4 = 4,
            DFF_Flat_4 = 14,
            DFF_Flat_5 = 15,
            DFF_Sparse_5 = 0x19,
            DFF_Sparse_7 = 0x1b,
            DFF_Wide_Sparse_6 = 0x24,
            DFF_Wide_Sparse_8 = 0x26,
            DFF_Sparse_Lerp_3 = 0x2b,
            DFF_Sparse_Lerp_4 = 0x2c,
            DFF_10x10 = 0x2d,
            DFF_5x7x5 = 0x2e
        }
    }
}

