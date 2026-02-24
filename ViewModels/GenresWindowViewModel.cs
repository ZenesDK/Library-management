using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using LibraryApp.Data;
using LibraryApp.Models;
using LibraryApp.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace LibraryApp.ViewModels;

public class GenresWindowViewModel : ReactiveObject
{
    private readonly LibraryContext _context;
    private readonly Window _parentWindow;
    private ObservableCollection<Genre> _genres = new();
    private Genre? _selectedGenre;

    public GenresWindowViewModel(LibraryContext context, Window parentWindow)
    {
        _context = context;
        _parentWindow = parentWindow;
        LoadGenres();
        
        AddCommand = ReactiveCommand.Create(AddGenre);
        EditCommand = ReactiveCommand.Create(EditGenre);
        DeleteCommand = ReactiveCommand.Create(DeleteGenre);
        CloseCommand = ReactiveCommand.Create(() => CloseRequested?.Invoke(this, EventArgs.Empty));
    }

    public ObservableCollection<Genre> Genres
    {
        get => _genres;
        set => this.RaiseAndSetIfChanged(ref _genres, value);
    }

    public Genre? SelectedGenre
    {
        get => _selectedGenre;
        set => this.RaiseAndSetIfChanged(ref _selectedGenre, value);
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CloseCommand { get; }

    public event EventHandler? CloseRequested;

    private void LoadGenres()
    {
        try
        {
            _genres.Clear();
            var genresFromDb = _context.Genres.ToList();
            foreach (var genre in genresFromDb)
                _genres.Add(genre);
            
            Console.WriteLine($"LoadGenres: загружено {genresFromDb.Count} жанров");
            
            foreach (var g in genresFromDb)
            {
                Console.WriteLine($" - {g.Name} (ID: {g.Id})");
            }
            
            this.RaisePropertyChanged(nameof(Genres));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке жанров: {ex.Message}");
        }
    }

    private async void AddGenre()
    {
        try
        {
            // Передаём копию всех жанров (или можно использовать _genres напрямую)
            var window = new GenreEditWindow(_genres);
            await window.ShowDialog(_parentWindow);
            
            if (window.Result == true)
            {
                var newGenre = new Genre 
                { 
                    Name = window.ViewModel.Name,
                    Description = window.ViewModel.Description
                };
                
                _context.Genres.Add(newGenre);
                await _context.SaveChangesAsync();
                LoadGenres();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private async void EditGenre()
    {
        if (SelectedGenre == null) return;
        
        try
        {
            // Передаём все жанры для проверки уникальности, исключая текущий при редактировании
            var window = new GenreEditWindow(_genres, SelectedGenre);
            await window.ShowDialog(_parentWindow);
            
            if (window.Result == true)
            {
                var genreToUpdate = _context.Genres.Find(SelectedGenre.Id);
                if (genreToUpdate != null)
                {
                    genreToUpdate.Name = window.ViewModel.Name;
                    genreToUpdate.Description = window.ViewModel.Description;
                    
                    await _context.SaveChangesAsync();
                    LoadGenres();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private void DeleteGenre()
    {
        if (SelectedGenre == null) 
        {
            Console.WriteLine("Жанр не выбран для удаления");
            return;
        }
        
        try
        {
            Console.WriteLine($"Удаление жанра ID: {SelectedGenre.Id}");
            
            _context.Genres.Remove(SelectedGenre);
            int deleted = _context.SaveChanges();
            
            Console.WriteLine($"Удалено записей: {deleted}");
            
            if (deleted > 0)
            {
                Console.WriteLine($"Жанр удалён, ID: {SelectedGenre.Id}");
                LoadGenres();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА при удалении жанра: {ex.Message}");
        }
    }
}