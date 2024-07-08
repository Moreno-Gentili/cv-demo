using GeneticSharp;

namespace OpenCVDemo.Models;

internal class GeneticTermination(MainWindowViewModel vm, Action<MainWindowViewModel, int, double> reportGeneration) : ITermination
{
    public bool HasReached(IGeneticAlgorithm geneticAlgorithm)
    {
        if (vm.FitToken.IsCancellationRequested)
        {
            return true;
        }

        reportGeneration(vm, geneticAlgorithm.GenerationsNumber, geneticAlgorithm.BestChromosome?.Fitness ?? 0);
        return geneticAlgorithm.GenerationsNumber >= vm.Generations;
    }
}
