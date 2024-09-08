namespace MHUtils.NeuralNetwork.PowerEstimation2
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LogResults
    {
        public int generation;
        public List<double> logs;
    }
}

