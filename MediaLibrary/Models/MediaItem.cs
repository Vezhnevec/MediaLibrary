using MediaLibrary.Enums;
using MediaLibrary.Exceptions;
using MediaLibrary.Interface;
using System;
using System.Collections.Generic;

// ==================== 2. АБСТРАКТНЫЙ БАЗОВЫЙ КЛАСС ====================
public abstract class MediaItem : IMedia, IComparable<MediaItem>
{
    private static int _nextId = 1;

    public int Id { get; }
    public string Title { get; set; }
    public Genre Genre { get; set; }
    public int Year { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateOnly? DueDate { get; set; }

    protected MediaItem(string title, Genre genre, int year)
    {
        Id = _nextId++;

        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidMediaDataException("Название не может быть пустым");
        Title = title;

        if (year < 0 || year > DateTime.Now.Year)
            throw new InvalidMediaDataException($"Год должен быть от 0 до {DateTime.Now.Year}");
        Year = year;

        Genre = genre;
    }

    public abstract string GetInfo();

    public virtual string GetStatus()
    {
        return IsAvailable ? "Доступен" : "Выдан";
    }
    public int CompareTo(MediaItem? mediaItem)
    {
        if (mediaItem is null) throw new ArgumentException("Некорректное значение параметра");
        return Year.CompareTo(mediaItem.Year);
        
    }
    public virtual object Clone() => MemberwiseClone();
    public virtual void Deconstruct(out int year, out Genre genre, out bool isAvailable)
    {
        year = Year;
        genre = Genre;
        isAvailable = IsAvailable;
    }
}