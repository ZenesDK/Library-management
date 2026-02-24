using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LibraryApp.Models;
using ReactiveUI;

namespace LibraryApp.ViewModels;

public class BookEditViewModel : ReactiveObject
{
    private string _title = string.Empty;
    private string _publishYear = string.Empty;
    private string _isbn = string.Empty;
    private string _quantityInStock = string.Empty;
    private readonly Book? _book;

    private ObservableCollection<Author> _availableAuthors = new();
    private ObservableCollection<Author> _selectedAuthors = new();

    private ObservableCollection<Genre> _availableGenres = new(); // все жанры
    private ObservableCollection<Genre> _selectedGenres = new(); // выбранные жанры

    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;

    public BookEditViewModel(ObservableCollection<Author> authors, ObservableCollection<Genre> genres, Book? book = null)
    {
        AvailableAuthors = authors;
        AvailableGenres = genres;
        _book = book;

        if (book != null)
        {
            Title = book.Title;
            PublishYear = book.PublishYear.ToString();
            ISBN = book.ISBN;
            QuantityInStock = book.QuantityInStock.ToString();
            WindowTitle = "Редактирование книги";

            // Загружаем выбранных авторов
            if (book.BookAuthors != null)
            {
                var selectedAuthors = book.BookAuthors.Select(ba => ba.Author).ToList();
                SelectedAuthors = new ObservableCollection<Author>(selectedAuthors);
            }

            // Загружаем выбранные жанры
            if (book.BookGenres != null)
            {
                var selectedGenres = book.BookGenres.Select(bg => bg.Genre).ToList();
                SelectedGenres = new ObservableCollection<Genre>(selectedGenres);
            }
        }
        else
        {
            WindowTitle = "Добавление книги";
            SelectedAuthors = new ObservableCollection<Author>();
            SelectedGenres = new ObservableCollection<Genre>();
        }

        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public string WindowTitle { get; }
    public ObservableCollection<Author> AvailableAuthors { get; set; }
    public ObservableCollection<Genre> AvailableGenres { get; set; }

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public string PublishYear
    {
        get => _publishYear;
        set => this.RaiseAndSetIfChanged(ref _publishYear, value);
    }

    public string ISBN
    {
        get => _isbn;
        set => this.RaiseAndSetIfChanged(ref _isbn, value);
    }

    public string QuantityInStock
    {
        get => _quantityInStock;
        set => this.RaiseAndSetIfChanged(ref _quantityInStock, value);
    }

    public ObservableCollection<Author> SelectedAuthors
    {
        get => _selectedAuthors;
        set => this.RaiseAndSetIfChanged(ref _selectedAuthors, value);
    }

    public ObservableCollection<Genre> SelectedGenres
    {
        get => _selectedGenres;
        set => this.RaiseAndSetIfChanged(ref _selectedGenres, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private bool IsValidIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return false;
        var clean = isbn.Replace("-", "").Replace(" ", "");
        if (clean.Length != 10 && clean.Length != 13) return false;
        return clean.All(char.IsDigit) || (clean.Length == 10 && clean.Substring(0, 9).All(char.IsDigit) && (clean[9] == 'X' || char.IsDigit(clean[9])));
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine("Название обязательно");
            return false;
        }

        if (SelectedAuthors.Count == 0)
        {
            Console.WriteLine("Выберите хотя бы одного автора");
            return false;
        }

        if (SelectedGenres.Count == 0)
        {
            Console.WriteLine("Выберите хотя бы один жанр");
            return false;
        }

        if (!int.TryParse(PublishYear, out var year) || year < 1800 || year > DateTime.Now.Year)
        {
            Console.WriteLine("Некорректный год издания");
            return false;
        }

        if (!IsValidIsbn(ISBN))
        {
            Console.WriteLine("Некорректный формат ISBN");
            return false;
        }

        if (!int.TryParse(QuantityInStock, out var qty) || qty < 0)
        {
            Console.WriteLine("Количество должно быть неотрицательным числом");
            return false;
        }

        return true;
    }

    private void Save()
    {
        if (!IsValid()) return;
        Console.WriteLine($"Save книга: {Title}, авторов: {SelectedAuthors.Count}, жанров: {SelectedGenres.Count}");
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }
}