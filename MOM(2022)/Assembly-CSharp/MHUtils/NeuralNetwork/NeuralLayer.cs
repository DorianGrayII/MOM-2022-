namespace MHUtils.NeuralNetwork
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

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

