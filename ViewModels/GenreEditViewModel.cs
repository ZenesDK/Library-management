using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LibraryApp.Models;
using ReactiveUI;

namespace LibraryApp.ViewModels;

public class GenreEditViewModel : ReactiveObject
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private readonly Genre? _genre;
    private readonly ObservableCollection<Genre> _allGenres; // все жанры для проверки уникальности

    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;

    public GenreEditViewModel(ObservableCollection<Genre> allGenres, Genre? genre = null)
    {
        _allGenres = allGenres;
        _genre = genre;

        if (genre != null)
        {
            Name = genre.Name;
            Description = genre.Description ?? string.Empty;
            Title = "Редактирование жанра";
        }
        else
        {
            Title = "Добавление жанра";
        }

        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public string Title { get; }
    public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
    public string Description { get => _description; set => this.RaiseAndSetIfChanged(ref _description, value); }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Console.WriteLine("Название жанра обязательно");
            return false;
        }

        // Проверка уникальности (исключая текущий жанр при редактировании)
        bool nameExists = _allGenres.Any(g => 
            g.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) && 
            (_genre == null || g.Id != _genre.Id));

        if (nameExists)
        {
            Console.WriteLine("Жанр с таким названием уже существует");
            return false;
        }

        return true;
    }

    private void Save()
    {
        if (!IsValid()) return;
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }
}