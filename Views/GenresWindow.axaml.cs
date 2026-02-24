using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibraryApp.ViewModels;
using LibraryApp.Data;

namespace LibraryApp.Views;

public partial class GenresWindow : Window
{
    private readonly GenresWindowViewModel _viewModel;
    
    public GenresWindow(LibraryContext context)
    {
        InitializeComponent();
        
        _viewModel = new GenresWindowViewModel(context, this);
        DataContext = _viewModel;
        
        _viewModel.CloseRequested += (s, e) => Close();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}