using MediaLibrary.Enums;
using MediaLibrary.Exceptions;

public class Movie : MediaItem
{
    public int Duration { get; set; }

    public Movie(string title, Genre genre, DateOnly releaseDate, int duration)
        : base(title, genre, releaseDate)
    {
        if (duration <= 0)
            throw new InvalidMediaDataException("Длительность должна быть больше 0");
        Duration = duration;
    }

    public override string GetInfo()
    {
        return $"[Фильм] #{Id}: «{Title}», жанр: {Genre}, {Year} г., {Duration} мин., статус: {GetStatus()}";
    }
}