using System.Collections;

namespace OpenCVDemo.Models;

public class GenePool : IReadOnlyList<Gene>
{
    readonly List<Gene> _genesList;
    readonly Dictionary<string, Gene> _genes;
    public GenePool(params Gene[] genes)
    {
        _genes = genes.ToDictionary(g => g.Name, g => g);
        _genesList = genes.ToList();
    }

    public int Count => _genesList.Count;

    public Gene this[int index]
    {
        get
        {
            return _genesList[index];
        }
    }

    public Gene this[string name]
    {
        get
        {
            return _genes[name];
        }

        set
        {
            if (name != value.Name)
            {
                throw new InvalidOperationException("Name does not match");
            }

            _genes[name] = value;
        }
    }
    

    public IEnumerator<Gene> GetEnumerator()
    {
        return _genesList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _genesList.GetEnumerator();
    }
}