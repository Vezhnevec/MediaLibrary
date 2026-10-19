using MediaLibrary.Enums;
using MediaLibrary.Exceptions;

public class Game : MediaItem
{
    public string Platform { get; set; }

    public Game(string title, Genre genre, int year, string platform)
        : base(title, genre, year)
    {
        if (string.IsNullOrWhiteSpace(platform))
            throw new InvalidMediaDataException("Платформа не может быть пустой");
        Platform = platform;
    }

    public override string GetInfo()
    {
        return $"[Игра] #{Id}: «{Title}», жанр: {Genre}, {Year} г., платформа: {Platform}, статус: {GetStatus()}";
    }
}