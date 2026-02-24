using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibraryApp.ViewModels;

namespace LibraryApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this); // Передаём this - ссылку на главное окно
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}