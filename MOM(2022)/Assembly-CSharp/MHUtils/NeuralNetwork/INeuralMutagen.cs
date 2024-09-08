namespace MHUtils.NeuralNetwork
{
    public interface INeuralMutagen
    {
        void Mutate(double value);

        void RestoreFromMutation();

        void ApplyMutation(double value);

        int GetLastMutationGeneration();

        void SetLastMutationGeneration(int gen);
    }
}
