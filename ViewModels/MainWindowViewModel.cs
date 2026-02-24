using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using LibraryApp.Data;
using LibraryApp.Models;
using LibraryApp.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace LibraryApp.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly LibraryContext _context;
    private readonly Window _mainWindow;
    private ObservableCollection<Book> _books = new();
    private ObservableCollection<Author> _authors = new();
    private ObservableCollection<Genre> _genres = new();
    private ObservableCollection<Book> _filteredBooks = new();
    private string _searchText = string.Empty;
    private Author? _selectedAuthor;
    private Genre? _selectedGenre;
    private Book? _selectedBook;

    public MainWindowViewModel(Window mainWindow)
    {
        _mainWindow = mainWindow;
        _context = new LibraryContext();
        _context.Database.EnsureCreated();

        AddTestDataIfNeeded();
        _ = LoadDataAsync();

        AddBookCommand = ReactiveCommand.Create(AddBook);
        EditBookCommand = ReactiveCommand.Create(EditBook);
        DeleteBookCommand = ReactiveCommand.Create(DeleteBook);
        ManageAuthorsCommand = ReactiveCommand.Create(ManageAuthors);
        ManageGenresCommand = ReactiveCommand.Create(ManageGenres);
        ResetFiltersCommand = ReactiveCommand.Create(ResetFilters);
        ForceRefreshCommand = ReactiveCommand.Create(() => _ = LoadDataAsync());
    }

    private void AddTestDataIfNeeded()
    {
        try
        {
            if (!_context.Authors.Any())
            {
                var author1 = new Author { FirstName = "Лев", LastName = "Толстой", BirthDate = new DateTime(1828, 9, 9), Country = "Россия" };
                var author2 = new Author { FirstName = "Фёдор", LastName = "Достоевский", BirthDate = new DateTime(1821, 11, 11), Country = "Россия" };
                var author3 = new Author { FirstName = "Антон", LastName = "Чехов", BirthDate = new DateTime(1860, 1, 29), Country = "Россия" };
                _context.Authors.AddRange(author1, author2, author3);
                _context.SaveChanges();
            }

            if (!_context.Genres.Any())
            {
                var genre1 = new Genre { Name = "Роман", Description = "Художественная литература" };
                var genre2 = new Genre { Name = "Драма", Description = "Драматические произведения" };
                var genre3 = new Genre { Name = "Рассказ", Description = "Короткая проза" };
                _context.Genres.AddRange(genre1, genre2, genre3);
                _context.SaveChanges();
            }

            var authors = _context.Authors.ToList();
            var genres = _context.Genres.ToList();

            if (!_context.Books.Any() && authors.Any() && genres.Any())
            {
                var author1 = authors.FirstOrDefault(a => a.LastName == "Толстой");
                var author2 = authors.FirstOrDefault(a => a.LastName == "Достоевский");
                var author3 = authors.FirstOrDefault(a => a.LastName == "Чехов");
                var genre1 = genres.FirstOrDefault(g => g.Name == "Роман");
                var genre2 = genres.FirstOrDefault(g => g.Name == "Рассказ");

                if (author1 != null && genre1 != null)
                {
                    var book1 = new Book
                    {
                        Title = "Война и мир",
                        PublishYear = 1869,
                        ISBN = "978-5-699-12014-7",
                        QuantityInStock = 5
                    };
                    _context.Books.Add(book1);
                    _context.SaveChanges();
                    _context.BookAuthors.Add(new BookAuthor { BookId = book1.Id, AuthorId = author1.Id });
                    _context.BookGenres.Add(new BookGenre { BookId = book1.Id, GenreId = genre1.Id });
                }

                if (author2 != null && genre1 != null)
                {
                    var book2 = new Book
                    {
                        Title = "Преступление и наказание",
                        PublishYear = 1866,
                        ISBN = "978-5-699-12015-4",
                        QuantityInStock = 3
                    };
                    _context.Books.Add(book2);
                    _context.SaveChanges();
                    _context.BookAuthors.Add(new BookAuthor { BookId = book2.Id, AuthorId = author2.Id });
                    _context.BookGenres.Add(new BookGenre { BookId = book2.Id, GenreId = genre1.Id });
                }

                if (author3 != null && genre2 != null)
                {
                    var book3 = new Book
                    {
                        Title = "Дама с собачкой",
                        PublishYear = 1899,
                        ISBN = "978-5-699-12016-1",
                        QuantityInStock = 2
                    };
                    _context.Books.Add(book3);
                    _context.SaveChanges();
                    _context.BookAuthors.Add(new BookAuthor { BookId = book3.Id, AuthorId = author3.Id });
                    _context.BookGenres.Add(new BookGenre { BookId = book3.Id, GenreId = genre2.Id });
                }

                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении тестовых данных: {ex.Message}");
        }
    }

    public ObservableCollection<Book> Books
    {
        get => _books;
        set => this.RaiseAndSetIfChanged(ref _books, value);
    }

    public ObservableCollection<Author> Authors
    {
        get => _authors;
        set => this.RaiseAndSetIfChanged(ref _authors, value);
    }

    public ObservableCollection<Genre> Genres
    {
        get => _genres;
        set => this.RaiseAndSetIfChanged(ref _genres, value);
    }

    public ObservableCollection<Book> FilteredBooks
    {
        get => _filteredBooks;
        set => this.RaiseAndSetIfChanged(ref _filteredBooks, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            FilterBooks();
        }
    }

    public Author? SelectedAuthor
    {
        get => _selectedAuthor;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAuthor, value);
            FilterBooks();
        }
    }

    public Genre? SelectedGenre
    {
        get => _selectedGenre;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedGenre, value);
            FilterBooks();
        }
    }

    public Book? SelectedBook
    {
        get => _selectedBook;
        set => this.RaiseAndSetIfChanged(ref _selectedBook, value);
    }

    public ICommand AddBookCommand { get; }
    public ICommand EditBookCommand { get; }
    public ICommand DeleteBookCommand { get; }
    public ICommand ManageAuthorsCommand { get; }
    public ICommand ManageGenresCommand { get; }
    public ICommand ResetFiltersCommand { get; }
    public ICommand ForceRefreshCommand { get; }

    private async Task LoadDataAsync()
    {
        try
        {
            _authors.Clear();
            var authorsFromDb = await _context.Authors.ToListAsync();
            foreach (var a in authorsFromDb) _authors.Add(a);

            _genres.Clear();
            var genresFromDb = await _context.Genres.ToListAsync();
            foreach (var g in genresFromDb) _genres.Add(g);

            _books.Clear();
            var booksFromDb = await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .ToListAsync();
            foreach (var b in booksFromDb) _books.Add(b);

            FilterBooks();

            this.RaisePropertyChanged(nameof(Authors));
            this.RaisePropertyChanged(nameof(Genres));
            this.RaisePropertyChanged(nameof(FilteredBooks));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
    }

    private void FilterBooks()
    {
        var query = _books.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            query = query.Where(b => b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        if (SelectedAuthor != null)
            query = query.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == SelectedAuthor.Id));

        if (SelectedGenre != null)
            query = query.Where(b => b.BookGenres.Any(bg => bg.GenreId == SelectedGenre.Id));

        FilteredBooks.Clear();
        foreach (var book in query)
            FilteredBooks.Add(book);

        this.RaisePropertyChanged(nameof(FilteredBooks));
    }

    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedAuthor = null;
        SelectedGenre = null;
    }

    private async void AddBook()
    {
        try
        {
            var authorsObs = new ObservableCollection<Author>(_authors);
            var genresObs = new ObservableCollection<Genre>(_genres);
            var window = new BookEditWindow(authorsObs, genresObs);
            await window.ShowDialog(_mainWindow);

            if (window.Result == true)
            {
                var vm = window.ViewModel;
                var newBook = new Book
                {
                    Title = vm.Title,
                    PublishYear = int.Parse(vm.PublishYear),
                    ISBN = vm.ISBN,
                    QuantityInStock = int.Parse(vm.QuantityInStock)
                };

                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();

                foreach (var author in vm.SelectedAuthors)
                {
                    _context.BookAuthors.Add(new BookAuthor
                    {
                        BookId = newBook.Id,
                        AuthorId = author.Id
                    });
                }

                foreach (var genre in vm.SelectedGenres)
                {
                    _context.BookGenres.Add(new BookGenre
                    {
                        BookId = newBook.Id,
                        GenreId = genre.Id
                    });
                }

                await _context.SaveChangesAsync();
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении книги: {ex.Message}");
        }
    }

    private async void EditBook()
    {
        if (SelectedBook == null) return;

        try
        {
            var authorsObs = new ObservableCollection<Author>(_authors);
            var genresObs = new ObservableCollection<Genre>(_genres);
            var window = new BookEditWindow(authorsObs, genresObs, SelectedBook);
            await window.ShowDialog(_mainWindow);

            if (window.Result == true)
            {
                var bookToUpdate = await _context.Books
                    .Include(b => b.BookAuthors)
                    .Include(b => b.BookGenres)
                    .FirstOrDefaultAsync(b => b.Id == SelectedBook.Id);

                if (bookToUpdate != null)
                {
                    var vm = window.ViewModel;
                    bookToUpdate.Title = vm.Title;
                    bookToUpdate.PublishYear = int.Parse(vm.PublishYear);
                    bookToUpdate.ISBN = vm.ISBN;
                    bookToUpdate.QuantityInStock = int.Parse(vm.QuantityInStock);

                    _context.BookAuthors.RemoveRange(bookToUpdate.BookAuthors);
                    foreach (var author in vm.SelectedAuthors)
                    {
                        _context.BookAuthors.Add(new BookAuthor
                        {
                            BookId = bookToUpdate.Id,
                            AuthorId = author.Id
                        });
                    }

                    _context.BookGenres.RemoveRange(bookToUpdate.BookGenres);
                    foreach (var genre in vm.SelectedGenres)
                    {
                        _context.BookGenres.Add(new BookGenre
                        {
                            BookId = bookToUpdate.Id,
                            GenreId = genre.Id
                        });
                    }

                    await _context.SaveChangesAsync();
                    await LoadDataAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при редактировании книги: {ex.Message}");
        }
    }

    private async void DeleteBook()
    {
        if (SelectedBook == null) return;

        try
        {
            var bookToDelete = await _context.Books.FindAsync(SelectedBook.Id);
            if (bookToDelete != null)
            {
                _context.Books.Remove(bookToDelete);
                await _context.SaveChangesAsync();
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении книги: {ex.Message}");
        }
    }

    private async void ManageAuthors()
    {
        var window = new AuthorsWindow(_context);
        await window.ShowDialog(_mainWindow);
        await LoadDataAsync();
    }

    private async void ManageGenres()
    {
        var window = new GenresWindow(_context);
        await window.ShowDialog(_mainWindow);
        await LoadDataAsync();
    }
}