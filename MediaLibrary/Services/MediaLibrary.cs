// ==================== 5. ОБОБЩЁННЫЙ РЕПОЗИТОРИЙ ====================
using MediaLibrary.Exceptions;
using MediaLibrary.Interface;
using MediaLibrary.Extensions;
using MediaLibrary.Criteria;

public class MediaLibrary<T> where T : class, IMedia
{
    public event Action<T> OnItemLended;
    public event Action<T> OnItemReturned;
    public event Action<T> OnItemAdd;
    private readonly List<T> _items = new List<T>();
    private readonly List<T> _basket = new List<T>();

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

    public void AddToBasket(int id)
    {
        var item = FindById(id);

        T itemCopy = (T)item.Clone();

        _basket.Add(itemCopy);
        OnItemAdd?.Invoke(itemCopy);
    }
    public void PrintAllBasket()
    {
        if (_basket.Count == 0)
        {
            Console.WriteLine("Корзина пуста");
            return;
        }
        foreach (var item in _basket)
        {
            Console.WriteLine($"{item.GetInfo()}");
        }
    }
    public void FindNewRealese()
    {        
        foreach ( T item in _items)
        {
            if (item.IsNewRelease())
            {
                Console.WriteLine($"{item.GetInfo()}");
            }
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
}