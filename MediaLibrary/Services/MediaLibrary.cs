// ==================== 5. ОБОБЩЁННЫЙ РЕПОЗИТОРИЙ ====================
using MediaLibrary.Criteria;
using MediaLibrary.Enums;
using MediaLibrary.Exceptions;
using MediaLibrary.Extensions;
using MediaLibrary.Interface;
using MediaLibrary.Models;
using MediaLibrary.Queue;
using System.Globalization;
using System.Text;
using System.Threading.Channels;

public class MediaLibrary<T> where T : class, IMedia
{
    public event Action<T> OnItemLended;
    public event Action<T> OnItemReturned;
    public event Action<T> OnItemAdd;
    private readonly List<T> _items = new List<T>();
    private readonly List<T> _cart = new List<T>();
    private readonly Dictionary<int, T> _itemsById = new Dictionary<int, T>();
    private readonly Stack<T> _recentlyViewed = new Stack<T>();
    private readonly List<LendRequest> _waitlist = new List<LendRequest>();
    private readonly List<Screening> _screening = new List<Screening>();
    private Lazy<string> _statsCache;

    public MediaLibrary()
    {
        InvalidateCache();
    }

    public void Add(T item)
    {
        _items.Add(item);
        _itemsById.Add(item.Id, item);
        InvalidateCache();
    }

    public void PrintAll()
    {
        if (_items.Count == 0)
        {
            Console.WriteLine("Библиотека пуста.");
            return;
        }
        foreach (var item in _items)
        {
            Console.WriteLine(item.GetInfo());
        }
    }

    public T FindById(int id)
    {
        if (_itemsById.TryGetValue(id, out T? value))
        {
            if (_recentlyViewed.Count > 0 && _recentlyViewed.Peek().Id == value.Id)
                return value;

            if (_recentlyViewed.Count >= 5)
                _recentlyViewed.Pop();

            _recentlyViewed.Push(value);
            return value;
        }
        throw new ItemNotFoundException(id);
    }

    public T this[int id]
    {
        get => FindById(id);
    }

    public bool Lend(int id)
    {
        var item = FindById(id);

        if (!item.IsAvailable)
            throw new ItemNotAvailableException(id);

        item.IsAvailable = false;
        item.DueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(14);
        OnItemLended?.Invoke(item);
        InvalidateCache();
        return true;
    }

    public bool Return(int id)
    {
        var item = FindById(id);

        if (item.IsAvailable)
            throw new ItemAlreadyReturnedException(id);

        item.IsAvailable = true;
        item.DueDate = null;
        OnItemReturned?.Invoke(item);
        InvalidateCache();
        return true;
    }

    public List<T> FindAll(Func<T, bool> condition)
    {
        List<T> result = new List<T>();

        foreach (T item in _items)
        {
            if (condition(item))
            {
                result.Add(item);
            }

        }
        return result;
    }

    public void Sort(Comparison<T> comparer)
    {
        _items.Sort(comparer);
    }

    public void AddToCart(int id)
    {
        var item = FindById(id);

        T itemCopy = (T)item.Clone();

        _cart.Add(itemCopy);
        OnItemAdd?.Invoke(itemCopy);
    }
    public void PrintAllCart()
    {
        if (_cart.Count == 0)
        {
            Console.WriteLine("Корзина пуста");
            return;
        }
        foreach (var item in _cart)
        {
            Console.WriteLine($"{item.GetInfo()}");
        }
    }

    public (int AllItems, int Available, int Issued) LibraryStats()
    {
        int allItems = _items.Count;
        int available = 0;
        int issued = 0;
        foreach (var item in _items)
        {
            if (item.IsAvailable)
            {
                available++;
            }
            else
            {
                issued++;
            }
        }
        return (allItems, available, issued);
    }
    public List<T> Search(MediaSearchCriteria criteria)
    {
        var results = new List<T>();

        foreach (var item in _items)
        {
            if (criteria.Matches(item))
            {
                if (_recentlyViewed.Count >= 5)
                {
                    _recentlyViewed.Pop();
                    _recentlyViewed.Push(item);
                }
                else
                {
                    _recentlyViewed.Push(item);
                }
                results.Add(item);
            }
        }

        return results;
    }
    public string GetCategory(T item) => item switch
    {
        Book { Pages: > 800 } => "Эпическое произведение",
        Book { Pages: < 100 } => "Легкое чтение",
        Movie { Duration: > 180 } => "Длинный метр",
        Movie { Duration: < 90 } => "Короткометражка",
        Game g when g.Platform.Equals("PC", StringComparison.OrdinalIgnoreCase)
        => "ПК-эксклюзив",

        Game g when g.Platform.Equals("PS5", StringComparison.OrdinalIgnoreCase) ||
                    g.Platform.Equals("XBOX", StringComparison.OrdinalIgnoreCase)
            => "Консольный эксклюзив",
        _ when item.ReleaseDate.Year <= DateTime.Now.Year - 50 => "Классика",
        _ => "Стандартный элемент"
    };
    public string GetCartSummary()
    {
        return (_cart) switch
        {
            [] => "Корзина пуста",
            [_,] => "Один предмет в корзине",
            [_, _] => "Два предмета в корзине",
            [_, _, ..] when _cart.All(item => item is Book) => "Коллекция книг",
            [_, _, ..] when _cart.All(item => item is Movie) => "Коллекция фильмов",
            [_, _, ..] when _cart.All(item => item is Game) => "Коллекция игр",
            [_, _, ..] => "Разнообразная подборка"
        };
    }
    public string GetPriority(T item) => item switch
    {
        ( > 2015, Genre.Fantasy or Genre.SciFi, _) => "Высокий приоритет",
        (_, _, false) => "Высокий приоритет",
        ( > 2000 and < 2015, Genre.Drama or Genre.Action, _) => "Средний приоритет",
        (_, Genre.RPG or Genre.Strategy, true) => "Средний приоритет",
        _ => "Низкий приоритет"
    };

    public Dictionary<Genre, int> GetGroupedByGenre()
    {
        var genrCounts = new Dictionary<Genre, int>();

        foreach (var item in _items)
        {
            Genre genre = item.Genre;

            if (!genrCounts.ContainsKey(genre))
            {
                genrCounts[genre] = 0;
            }
            genrCounts[genre]++;
        }
        return genrCounts;
    }
    public IEnumerable<T> GetRecentlyViewed()
    {
        return _recentlyViewed;
    }

    public void AddToWaitList(LendRequest lendRequest)
    {
        _waitlist.Add(lendRequest);
    }
    public void PrintWaitList()
    {
        if (_waitlist.Count == 0)
        {
            Console.WriteLine("Лист ожидания пустой");
            return;
        }
        foreach (var item in _waitlist)
        {
            Console.WriteLine($"Id позиции: {item.ItemId}, Имя читателя: {item.ReaderName}, Email: {item.Email}");
        }
    }

    public IEnumerable<T> Filter(Func<T, bool> predicate)
    {
        foreach (T item in _items)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }
    public List<T> GetLastAdded(int count)
    {
        int actualCount = Math.Min(count, _items.Count);

        if (actualCount == 0) return new List<T>();

        return _items[^actualCount..].ToList();
    }
    public bool TryProcessWaitlist(int returnedItemId, out LendRequest? satisfiedRequest)
    {
        satisfiedRequest = null;

        var request = _waitlist.FirstOrDefault(r => r.ItemId == returnedItemId);

        if (request != null)
        {
            satisfiedRequest = request;
            _waitlist.Remove(request);
            return true;
        }

        return false;
    }
    public string GenerateCartReceipt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{DateTime.Now:dd.MM.yyyy HH:mm}");
        sb.AppendLine("========================");
        sb.AppendLine($"{"ID",-5} | {"Жанр",-10} | {"Название",-25} | {"Год",-6}");

        foreach (var item in _cart)
        {
            sb.AppendLine($"{item.Id,-5} | {item.Genre,-10} | {item.Title,-25} | {item.Year,-6}");
        }
        sb.AppendLine("========================");
        sb.AppendLine($"Всего позиций: {_cart.Count}");
        return sb.ToString();
    }
    public string CheckOverdueStatus(int id)
    {
        var item = FindById(id);

        if (item.IsAvailable)
            return "Элемент доступен и не находится на руках";

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        if (item.DueDate.HasValue)
        {
            DateOnly dueDate = item.DueDate.Value;

            int diff = today.DayNumber - dueDate.DayNumber;

            if (diff > 0)
                return $"Просрочено на {diff} дн. (Срок был до {dueDate:dd.MM.yyyy})";

            if (diff == 0)
                return $"Срок возврата ИСТЕКАЕТ СЕГОДНЯ ({dueDate:dd.MM.yyyy})";

            return $"Элемент на руках. Срок возврата: {dueDate:dd.MM.yyyy}, осталось {Math.Abs(diff)} дн.";
        }
        return "Элемент на руках, но срок возврата не установлен";
    }
    public void AddScreening(Screening item)
    {
        _screening.Add(item);
    }
    public string PrintScreeningSchedule()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in _screening)
        {
            TimeOnly endTime = item.StartTime.Add(TimeSpan.FromMinutes(item.DurationMinutes));

            sb.AppendLine($"Фильм: '{item.MovieTitle}' | {item.Date:dd.MM.yyyy} | Начало: {item.StartTime:HH:mm} | Конец: {endTime:HH:mm}");
        }
        return sb.ToString();
    }
    public string GenerateActivityReport(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
        {
            return "Стартовая дата больше конечной";
        }

        StringBuilder sb = new StringBuilder();

        List<T> filtered = new List<T>(_items);

        filtered.RemoveAll(item => item.ReleaseDate < startDate || item.ReleaseDate > endDate);

        filtered.Sort((a, b) => a.ReleaseDate.CompareTo(b.ReleaseDate));
        if (filtered.Count == 0) return "Нет элементов за указанный период.";
        foreach (var item in filtered)
        {
            sb.AppendLine($"{item.ReleaseDate.ToString("dddd, dd MMMM yyyy г.", new CultureInfo("ru-RU"))} Название элемента: {item.Title}");
        }
        sb.AppendLine($"Найдено {filtered.Count} элементов за период с {startDate} по {endDate}");
        return sb.ToString();
    }
    private string BuildStatistics()
    {
        int available = 0;
        int issued = 0;
        int averageAge = 0;
        double sum = 0;

        foreach (var item in _items)
        {
            if (item.IsAvailable)
            {
                available++;
            }
            else
            {
                issued++;
            }
            sum += DateTime.Now.Year - item.Year;
        }
        averageAge = (int)(_items.Count > 0 ? Math.Round(sum / _items.Count) : 0);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Количество элементов: {_items.Count}");
        sb.AppendLine($"Доступно элементов: {available}");
        sb.AppendLine($"Выдано элементов: {issued}");
        sb.AppendLine($"Средний возраст библиотеки: {averageAge}");

        Thread.Sleep(1000);

        return sb.ToString();
    }
    public string ShowStatistics()
    {
        return _statsCache.Value;
    }
    private void InvalidateCache()
    {
        _statsCache = new Lazy<string>(() => BuildStatistics());
    }

}