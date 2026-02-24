using System;
using System.Windows.Input;
using LibraryApp.Models;
using ReactiveUI;

namespace LibraryApp.ViewModels;

public class AuthorEditViewModel : ReactiveObject
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateTime _birthDate = DateTime.Now;
    private string _country = string.Empty;

    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;

    public AuthorEditViewModel(Author? author = null)
    {
        if (author != null)
        {
            FirstName = author.FirstName;
            LastName = author.LastName;
            BirthDate = author.BirthDate;
            Country = author.Country;
            Title = "Редактирование автора";
        }
        else
        {
            Title = "Добавление автора";
        }

        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public string Title { get; }
    public string FirstName { get => _firstName; set => this.RaiseAndSetIfChanged(ref _firstName, value); }
    public string LastName { get => _lastName; set => this.RaiseAndSetIfChanged(ref _lastName, value); }
    public DateTime BirthDate { get => _birthDate; set => this.RaiseAndSetIfChanged(ref _birthDate, value); }
    public string Country { get => _country; set => this.RaiseAndSetIfChanged(ref _country, value); }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private void Save() => SaveRequested?.Invoke(this, EventArgs.Empty);
    private void Cancel() => CancelRequested?.Invoke(this, EventArgs.Empty);
}