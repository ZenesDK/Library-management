using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibraryApp.ViewModels;
using LibraryApp.Models;

namespace LibraryApp.Views;

public partial class AuthorEditWindow : Window
{
    private readonly AuthorEditViewModel _viewModel;
    
    public AuthorEditWindow(Author? author = null)
    {
        InitializeComponent();
        _viewModel = new AuthorEditViewModel(author);
        DataContext = _viewModel;
        
        _viewModel.SaveRequested += (s, e) => Close(true);
        _viewModel.CancelRequested += (s, e) => Close(false);
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public AuthorEditViewModel ViewModel => _viewModel;
    public bool? Result { get; private set; }
    
    private void Close(bool result)
    {
        Result = result;
        base.Close(result);
    }
}