using MediaLibrary.Enums;
using MediaLibrary.Exceptions;

public class Movie : MediaItem
{
    public int Duration { get; set; }

    public Movie(string title, Genre genre, int year, int duration)
        : base(title, genre, year)
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