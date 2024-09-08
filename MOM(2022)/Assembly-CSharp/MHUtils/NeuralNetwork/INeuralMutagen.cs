namespace MHUtils.NeuralNetwork
{
    using System;

    public interface INeuralMutagen
    {
        void ApplyMutation(double value);
        int GetLastMutationGeneration();
        void Mutate(double value);
        void RestoreFromMutation();
        void SetLastMutationGeneration(int gen);
    }
}

