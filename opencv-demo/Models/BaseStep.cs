using OpenCvSharp;
using System.ComponentModel;
using System.Windows.Controls;

namespace OpenCVDemo.Models;

public abstract class BaseStep : UserControl, IStep
{
    readonly IStep _step;
    protected BaseStep(IStep stepViewModel)
    {
        _step = stepViewModel;
        DataContext = stepViewModel;
        stepViewModel.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(sender, args);
    }

    public virtual string StepName => GetType().Name;

    protected IStep Step => _step;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        return Step.Process(image, path, genes);
    }

    public GenePool Genes
    {
        get
        {
            return _step.Genes;
        }

        set
        {
            _step.Genes = value;
        }
    }
}
