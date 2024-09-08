using System.Collections.Generic;
using ProtoBuf;

namespace MHUtils.NeuralNetwork
{
    [ProtoContract]
    public class NeuralLayer
    {
        [ProtoMember(1)]
        public List<int> neurons;

        public NeuralLayer()
        {
        }

        public NeuralLayer(int size)
        {
            this.neurons = new List<int>(size);
        }
    }
}
