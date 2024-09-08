namespace MHUtils.NeuralNetwork
{
    using MHUtils;
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class NeuralInputLink : INeuralMutagen
    {
        [ProtoMember(1)]
        public int sourceNeuronID;
        [ProtoMember(2)]
        public double weight;
        [ProtoMember(3)]
        public double weightChangeGradient;
        [ProtoMember(4)]
        public int lastMutationGen;
        [ProtoIgnore]
        private double premutationWeight;

        public void ApplyMutation(double d)
        {
            this.weight += this.weight * d;
            this.weightChangeGradient = (this.weight * d) * 2.0;
        }

        public void Evolve(double strength, MHRandom random)
        {
            double num = random.GetDouble01();
            if (num < strength)
            {
                double val = (this.weightChangeGradient + num) - (strength * 0.5);
                this.weightChangeGradient = NeuralUtils.Clamp(val, 0.1);
            }
            double num2 = this.weight + ((random.GetDouble01() * strength) * this.weightChangeGradient);
            this.weight = num2;
        }

        public int GetLastMutationGeneration()
        {
            return this.lastMutationGen;
        }

        public Neuron GetNeuron(MHUtils.NeuralNetwork.NeuralNetwork nn)
        {
            return nn.GetNeuron(this.sourceNeuronID);
        }

        public void Mutate(double value)
        {
            this.premutationWeight = this.weight;
            this.weight += this.weight * value;
        }

        public void RestoreFromMutation()
        {
            this.weight = this.premutationWeight;
        }

        public void SetLastMutationGeneration(int gen)
        {
            this.lastMutationGen = gen;
        }
    }
}

