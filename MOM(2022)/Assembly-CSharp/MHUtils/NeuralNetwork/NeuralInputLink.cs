using ProtoBuf;

namespace MHUtils.NeuralNetwork
{
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

        public Neuron GetNeuron(NeuralNetwork nn)
        {
            return nn.GetNeuron(this.sourceNeuronID);
        }

        public void Evolve(double strength, MHRandom random)
        {
            double @double = random.GetDouble01();
            if (@double < strength)
            {
                double val = this.weightChangeGradient + @double - strength * 0.5;
                this.weightChangeGradient = NeuralUtils.Clamp(val, 0.1);
            }
            double num = this.weight + random.GetDouble01() * strength * this.weightChangeGradient;
            this.weight = num;
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

        public void ApplyMutation(double d)
        {
            this.weight += this.weight * d;
            this.weightChangeGradient = this.weight * d * 2.0;
        }

        public int GetLastMutationGeneration()
        {
            return this.lastMutationGen;
        }

        public void SetLastMutationGeneration(int gen)
        {
            this.lastMutationGen = gen;
        }
    }
}
