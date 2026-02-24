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

public class AuthorsWindowViewModel : ReactiveObject
{
    private readonly LibraryContext _context;
    private readonly Window _parentWindow;
    private ObservableCollection<Author> _authors = new();
    private Author? _selectedAuthor;

    public AuthorsWindowViewModel(LibraryContext context, Window parentWindow)
    {
        _context = context;
        _parentWindow = parentWindow;
        LoadAuthors();
        
        AddCommand = ReactiveCommand.Create(AddAuthor);
        EditCommand = ReactiveCommand.Create(EditAuthor);
        DeleteCommand = ReactiveCommand.Create(DeleteAuthor);
        CloseCommand = ReactiveCommand.Create(() => CloseRequested?.Invoke(this, EventArgs.Empty));
    }

    public ObservableCollection<Author> Authors
    {
        get => _authors;
        set => this.RaiseAndSetIfChanged(ref _authors, value);
    }

    public Author? SelectedAuthor
    {
        get => _selectedAuthor;
        set => this.RaiseAndSetIfChanged(ref _selectedAuthor, value);
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CloseCommand { get; }

    public event EventHandler? CloseRequested;

    private void LoadAuthors()
    {
        try
        {
            _authors.Clear();
            var authorsFromDb = _context.Authors.ToList();
            foreach (var author in authorsFromDb)
                _authors.Add(author);
            
            Console.WriteLine($"LoadAuthors: загружено {authorsFromDb.Count} авторов");
            
            foreach (var a in authorsFromDb)
            {
                Console.WriteLine($" - {a.FirstName} {a.LastName} (ID: {a.Id})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке авторов: {ex.Message}");
        }
    }

    private async void AddAuthor()
    {
        try
        {
            Console.WriteLine("Нажата кнопка Добавить автора");
            
            var window = new AuthorEditWindow();
            await window.ShowDialog(_parentWindow);
            
            // Проверяем результат через наше свойство
            if (window.Result == true)
            {
                Console.WriteLine("Окно закрыто с Save");
                
                var newAuthor = new Author 
                { 
                    FirstName = window.ViewModel.FirstName,
                    LastName = window.ViewModel.LastName,
                    BirthDate = window.ViewModel.BirthDate,
                    Country = window.ViewModel.Country
                };
                
                _context.Authors.Add(newAuthor);
                int saved = _context.SaveChanges();
                
                Console.WriteLine($"Сохранено: {saved}");
                
                if (saved > 0)
                {
                    LoadAuthors();
                }
            }
            else
            {
                Console.WriteLine("Окно закрыто без Save");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА: {ex.Message}");
        }
    }
    private async void EditAuthor()
    {
        if (SelectedAuthor == null) return;
        
        try
        {
            var window = new AuthorEditWindow(SelectedAuthor);
            var result = await window.ShowDialog<bool?>(_parentWindow);
            
            if (result == true)
            {
                var authorToUpdate = _context.Authors.Find(SelectedAuthor.Id);
                if (authorToUpdate != null)
                {
                    authorToUpdate.FirstName = window.ViewModel.FirstName;
                    authorToUpdate.LastName = window.ViewModel.LastName;
                    authorToUpdate.BirthDate = window.ViewModel.BirthDate;
                    authorToUpdate.Country = window.ViewModel.Country;
                    
                    _context.SaveChanges();
                    Console.WriteLine($"Автор обновлён, ID: {SelectedAuthor.Id}");
                    LoadAuthors();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при редактировании автора: {ex.Message}");
        }
    }

    private void DeleteAuthor()
    {
        if (SelectedAuthor == null) return;
        
        try
        {
            _context.Authors.Remove(SelectedAuthor);
            int deleted = _context.SaveChanges();
            
            if (deleted > 0)
            {
                Console.WriteLine($"Автор удалён, ID: {SelectedAuthor.Id}");
                LoadAuthors();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении автора: {ex.Message}");
        }
    }
}