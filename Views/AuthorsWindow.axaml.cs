using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibraryApp.ViewModels;
using LibraryApp.Data;

namespace LibraryApp.Views;

public partial class AuthorsWindow : Window
{
    private readonly AuthorsWindowViewModel _viewModel;
    
    public AuthorsWindow(LibraryContext context)
    {
        InitializeComponent();
        
        _viewModel = new AuthorsWindowViewModel(context, this);
        DataContext = _viewModel;
        
        _viewModel.CloseRequested += (s, e) => Close();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}