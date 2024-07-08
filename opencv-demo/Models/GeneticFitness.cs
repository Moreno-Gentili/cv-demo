using GeneticSharp;

namespace OpenCVDemo.Models;

internal class GeneticFitness : IFitness
{
    readonly IReadOnlyList<GenePool> _genePools;
    readonly IReadOnlyList<IStep> _steps;
    readonly MainWindowViewModel _vm;
    readonly Func<IReadOnlyList<GenePool>, IReadOnlyList<IStep>, MainWindowViewModel, double> _fitnessStrategy;

    public GeneticFitness(MainWindowViewModel vm, Func<IReadOnlyList<GenePool>, IReadOnlyList<IStep>, MainWindowViewModel, double> fitnessStrategy)
    {
        _genePools = vm.Steps.Select(s => s.Genes).ToList();
        _steps = vm.Steps;
        _vm = vm;
        _fitnessStrategy = fitnessStrategy;
    }

    public double Evaluate(IChromosome chromosome)
    {
        List<GenePool> editedGenePools = ChromosomeToGenePools(chromosome);
        return _fitnessStrategy.Invoke(editedGenePools, _steps, _vm);
    }

    public List<GenePool> ChromosomeToGenePools(IChromosome chromosome)
    {
        int[] values = ((FloatingPointChromosome)chromosome).ToFloatingPoints().Select(Convert.ToInt32).ToArray();
        int c = 0;

        List<GenePool> editedGenePools = new();
        for (int i = 0; i < _genePools.Count; i++)
        {
            GenePool genePool = _genePools[i];
            List<Gene> genes = new();
            foreach (Gene gene in genePool)
            {
                int currentValue = values[c];
                Gene editedGene = gene with { Current = currentValue };
                genes.Add(editedGene);
                c++;
            }

            GenePool editedGenePool = new(genes.ToArray());
            editedGenePools.Add(editedGenePool);
        }

        return editedGenePools;
    }
}
