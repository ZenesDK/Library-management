using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibraryApp.ViewModels;
using LibraryApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;

namespace LibraryApp.Views;

public partial class BookEditWindow : Window
{
    private readonly BookEditViewModel _viewModel;
    private ListBox? _authorsListBox;
    private ListBox? _genresListBox;
    
    public BookEditWindow(ObservableCollection<Author> authors, ObservableCollection<Genre> genres, Book? book = null)
    {
        InitializeComponent();
        _viewModel = new BookEditViewModel(authors, genres, book);
        DataContext = _viewModel;
        
        _viewModel.SaveRequested += (s, e) => {
            // Авторы
            if (_authorsListBox?.SelectedItems != null)
            {
                var selectedAuthors = _authorsListBox.SelectedItems.OfType<Author>().ToList();
                _viewModel.SelectedAuthors.Clear();
                foreach (var author in selectedAuthors)
                    _viewModel.SelectedAuthors.Add(author);
            }
            // Жанры
            if (_genresListBox?.SelectedItems != null)
            {
                var selectedGenres = _genresListBox.SelectedItems.OfType<Genre>().ToList();
                _viewModel.SelectedGenres.Clear();
                foreach (var genre in selectedGenres)
                    _viewModel.SelectedGenres.Add(genre);
            }
            Close(true);
        };
        
        _viewModel.CancelRequested += (s, e) => Close(false);
        
        // Предустановка выбранных элементов при редактировании
        if (book != null)
        {
            // Авторы
            if (_authorsListBox != null && book.BookAuthors != null)
            {
                var authorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();
                foreach (var item in _authorsListBox.Items)
                    if (item is Author author && authorIds.Contains(author.Id))
                        _authorsListBox.SelectedItems.Add(author);
            }
            // Жанры
            if (_genresListBox != null && book.BookGenres != null)
            {
                var genreIds = book.BookGenres.Select(bg => bg.GenreId).ToHashSet();
                foreach (var item in _genresListBox.Items)
                    if (item is Genre genre && genreIds.Contains(genre.Id))
                        _genresListBox.SelectedItems.Add(genre);
            }
        }
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _authorsListBox = this.FindControl<ListBox>("AuthorsListBox");
        _genresListBox = this.FindControl<ListBox>("GenresListBox");
    }
    
    public BookEditViewModel ViewModel => _viewModel;
    public bool? Result { get; private set; }
    
    private void Close(bool result)
    {
        Result = result;
        base.Close(result);
    }
}