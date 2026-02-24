using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibraryApp.ViewModels;
using LibraryApp.Models;
using System.Collections.ObjectModel;

namespace LibraryApp.Views;

public partial class GenreEditWindow : Window
{
    private readonly GenreEditViewModel _viewModel;
    
    public GenreEditWindow(ObservableCollection<Genre> allGenres, Genre? genre = null)
    {
        InitializeComponent();
        _viewModel = new GenreEditViewModel(allGenres, genre);
        DataContext = _viewModel;
        
        _viewModel.SaveRequested += (s, e) => Close(true);
        _viewModel.CancelRequested += (s, e) => Close(false);
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public GenreEditViewModel ViewModel => _viewModel;
    public bool? Result { get; private set; }
    
    private void Close(bool result)
    {
        Result = result;
        base.Close(result);
    }
}