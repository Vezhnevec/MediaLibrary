// ==================== 5. ОБОБЩЁННЫЙ РЕПОЗИТОРИЙ ====================
using MediaLibrary.Exceptions;
using MediaLibrary.Interface;
using MediaLibrary.Extensions;
using MediaLibrary.Criteria;
using System.Threading.Channels;
using MediaLibrary.Enums;

public class MediaLibrary<T> where T : class, IMedia
{
    public event Action<T> OnItemLended;
    public event Action<T> OnItemReturned;
    public event Action<T> OnItemAdd;
    private readonly List<T> _items = new List<T>();
    private readonly List<T> _cart = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
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
        var item = _items.Find(p => p.Id == id);
        if (item == null)
            throw new ItemNotFoundException(id);
        return item;
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
        OnItemLended?.Invoke(item);
        return true;
    }

    public bool Return(int id)
    {
        var item = FindById(id);

        if (item.IsAvailable)
            throw new ItemAlreadyReturnedException(id);

        item.IsAvailable = true;
        OnItemReturned?.Invoke(item);
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
        Game { Platform: "PC" } => "ПК-эксклюзив",
        Game { Platform: "PS5" or "Xbox" } => "Консольный эксклюзив",
        _ when item.Year <= DateTime.Now.Year - 50 => "Классика",
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
    
}