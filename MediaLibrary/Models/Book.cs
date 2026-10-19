// ==================== 3. КЛАССЫ-НАСЛЕДНИКИ ====================
using MediaLibrary.Enums;
using MediaLibrary.Exceptions;

public class Book : MediaItem
{
    public int Pages { get; set; }

    public Book(string title, Genre genre, int year, int pages)
        : base(title, genre, year)
    {
        if (pages <= 0)
            throw new InvalidMediaDataException("Количество страниц должно быть больше 0");
        Pages = pages;
    }

    public override string GetInfo()
    {
        return $"[Книга] #{Id}: «{Title}», жанр: {Genre}, {Year} г., {Pages} стр., статус: {GetStatus()}";
    }
}