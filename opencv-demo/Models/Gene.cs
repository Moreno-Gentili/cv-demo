namespace OpenCVDemo.Models;

public record Gene
{
    public Gene(string name, int min, int max, int current)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(nameof(name));
        }

        if (min < 0 || min > max)
        {
            throw new ArgumentException(nameof(min));
        }

        if (current < min || current > max)
        {
            throw new ArgumentException(nameof(max));
        }

        Name = name;
        Min = min;
        Max = max;
        Current = current;
    }

    public string Name { get; }
    public int Min { get; }
    public int Max { get; }
    public int Current { get; init; }

    public static implicit operator int(Gene gene) => gene.Current;

    public int Bits
    {
        get
        {
            for (int i = 0; i < 32; i++)
            {
                if (Max <= Math.Pow(2, i))
                {
                    return i + 1;
                }
            }

            return 32;
        }
    }
}