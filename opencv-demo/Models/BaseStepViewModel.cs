using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using System.Windows.Input;

namespace OpenCVDemo.Models;

public partial class BaseStepViewModel : ObservableObject, IStep
{
    readonly BaseStep _step;
    readonly ICommand _deleteCommand;
    public BaseStepViewModel(BaseStep step, ICommand deleteCommand)
    {
        _step = step;
        _deleteCommand = deleteCommand;
        _step.PropertyChanged += (sender, args) => OnPropertyChanged(args);
    }

    [ObservableProperty]
    bool isEnabled = true;

    public string Name => _step.StepName;

    public BaseStep Step => _step;

    public ICommand DeleteCommand => _deleteCommand;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        if (IsEnabled)
        {
            return _step.Process(image, path, genes);
        }
        else
        {
            return new(image);
        }
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
