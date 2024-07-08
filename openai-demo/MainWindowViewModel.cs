using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenAIDemo.Services;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenAIDemo;

public partial class MainWindowViewModel(OpenAIClient client) : ObservableObject
{
    [ObservableProperty]
    string systemMessage = "You are a helpful assistant";

    [ObservableProperty]
    string userMessage = "In quale città si trovano le officine Credem?";

    [ObservableProperty]
    string completion = string.Empty;

    [ObservableProperty]
    decimal? cost;

    [ObservableProperty]
    ImageSource? imageSource;

    [ObservableProperty]
    byte[]? fileContent;

    [ObservableProperty]
    double temperature;

    [ObservableProperty]
    bool jsonMode;

    partial void OnFileContentChanged(byte[]? value)
    {
        if (value is null)
        {
            ImageSource = null;
        }
        else
        {
            ImageSource = ByteToBitmapImage(value);
        }
    }

    [RelayCommand]
    async Task SendRequest()
    {
        Cost = null;

        try
        {
            (Completion, Cost) = await client.SendCompletionRequestAsync(SystemMessage, UserMessage, Temperature, JsonMode, FileContent);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Si è verificato un errore: {ex.Message}");
        }
    }

    [RelayCommand]
    void RemoveImage()
    {
        FileContent = null;
    }

    [RelayCommand]
    void Browse()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png"
        };

        if (dialog.ShowDialog() == true)
        {
            FileContent = File.ReadAllBytes(dialog.FileName);
        }
    }

    [RelayCommand]
    void ToggleJsonMode()
    {
        JsonMode = !JsonMode;
    }

    static ImageSource ByteToBitmapImage(byte[] value)
    {
        var image = new BitmapImage();
        using (var mem = new MemoryStream(value))
        {
            mem.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = null;
            image.StreamSource = mem;
            image.EndInit();
        }

        image.Freeze();
        return image;
    }
}
