using MediaLibrary.Enums;
using MediaLibrary.Exceptions;
using MediaLibrary.Extensions;
using MediaLibrary.Helpers;
using MediaLibrary.Extensions;
using System.Security.Cryptography.X509Certificates;
using MediaLibrary.Criteria;
// ==================== 7. ГЛАВНОЕ МЕНЮ ====================
MediaLibrary<MediaItem> mediaLibrary = new MediaLibrary<MediaItem>();

mediaLibrary.OnItemLended += (item) =>
{
    Console.WriteLine($"[ЛОГ {DateTime.Now:HH:mm:ss}] Выдан элемент: {item.Title}");
};

mediaLibrary.OnItemReturned += (item) =>
{
    Console.WriteLine($"[ЛОГ {DateTime.Now:HH:mm:ss}] Возвращен элемент: {item.Title}");
};

mediaLibrary.OnItemAdd += (item) =>
{
    Console.WriteLine($"[ЛОГ {DateTime.Now:HH:mm:ss}] Добавлен элемент: {item.Title}");
};

while (true)
{
    Console.WriteLine("\n=== БИБЛИОТЕКА МЕДИА ===");
    Console.WriteLine("1. Добавить книгу");
    Console.WriteLine("2. Добавить фильм");
    Console.WriteLine("3. Добавить игру");
    Console.WriteLine("4. Показать всю коллекцию");
    Console.WriteLine("5. Найти по ID");
    Console.WriteLine("6. Выдать элемент");
    Console.WriteLine("7. Вернуть элемент");
    Console.WriteLine("8. Выход");
    Console.WriteLine("9. Найти по жанру");
    Console.WriteLine("10. Показать элементы, выпущенные после указанного года");
    Console.WriteLine("11. Сортировка по году выпуска(сначала новые)");
    Console.WriteLine("12. Сортировать по названию(по алфавиту)");
    Console.WriteLine("13. Добавить объект в корзину");
    Console.WriteLine("14. Показать корзину");
    Console.WriteLine("15. Показать новинки");
    Console.WriteLine("16. Показать статистику");
    Console.WriteLine("18. Расширенный поиск");
    Console.WriteLine("======================");
    Console.Write("Выберите пункт меню: ");

    string input = Console.ReadLine();
    if (!int.TryParse(input, out int choice))
    {
        Console.WriteLine("Введите корректный номер пункта.");
        continue;
    }

    switch (choice)
    {
        case 1: // Добавить книгу
            try
            {
                string title = ConsoleHelper.ReadString("Название книги:");
                Genre genre = ConsoleHelper.ReadGenre();
                int year = ConsoleHelper.ReadInt("Год выпуска:");
                int pages = ConsoleHelper.ReadInt("Количество страниц:");

                Book book = new Book(title, genre, year, pages);
                mediaLibrary.Add(book);
                Console.WriteLine($"✓ Книга «{title}» успешно добавлена!");
            }
            catch (InvalidMediaDataException ex)
            {
                Console.WriteLine($"✗ Ошибка добавления: {ex.Message}");
            }
            break;

        case 2: // Добавить фильм
            try
            {
                string title = ConsoleHelper.ReadString("Название фильма:");
                Genre genre = ConsoleHelper.ReadGenre();
                int year = ConsoleHelper.ReadInt("Год выпуска:");
                int duration = ConsoleHelper.ReadInt("Длительность (мин):");

                Movie movie = new Movie(title, genre, year, duration);
                mediaLibrary.Add(movie);
                Console.WriteLine($"✓ Фильм «{title}» успешно добавлен!");
            }
            catch (InvalidMediaDataException ex)
            {
                Console.WriteLine($"✗ Ошибка добавления: {ex.Message}");
            }
            break;

        case 3: // Добавить игру
            try
            {
                string title = ConsoleHelper.ReadString("Название игры:");
                Genre genre = ConsoleHelper.ReadGenre();
                int year = ConsoleHelper.ReadInt("Год выпуска:");
                string platform = ConsoleHelper.ReadString("Платформа (PC, PS5, Xbox и т.д.):");

                Game game = new Game(title, genre, year, platform);
                mediaLibrary.Add(game);
                Console.WriteLine($"✓ Игра «{title}» успешно добавлена!");
            }
            catch (InvalidMediaDataException ex)
            {
                Console.WriteLine($"✗ Ошибка добавления: {ex.Message}");
            }
            break;

        case 4: // Показать всю коллекцию
            mediaLibrary.PrintAll();
            break;

        case 5: // Найти по ID
            {
                int searchId = ConsoleHelper.ReadInt("Введите ID элемента для поиска:");
                try
                {
                    var foundItem = mediaLibrary[searchId];
                    Console.WriteLine($"✓ Найдено: {foundItem.GetInfo()}");
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 6: // Выдать элемент
            int lendId = ConsoleHelper.ReadInt("Введите ID элемента для выдачи:");
            try
            {
                mediaLibrary.Lend(lendId);
                //Console.WriteLine("✓ Элемент успешно выдан!");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            catch (ItemNotAvailableException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            break;

        case 7: // Вернуть элемент
            int returnId = ConsoleHelper.ReadInt("Введите ID элемента для возврата:");
            try
            {
                mediaLibrary.Return(returnId);
                //Console.WriteLine("✓ Элемент успешно возвращён!");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            catch (ItemAlreadyReturnedException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            break;

        case 8: // Выход
            Console.WriteLine("Спасибо за использование библиотеки! До свидания.");
            return;

        case 9: // Найти по жанру
            {
                Genre selectedGenre = ConsoleHelper.ReadGenre();
                var foundItems = mediaLibrary.FindAll(item => item.Genre == selectedGenre);
                foreach (var item in foundItems)
                {
                    Console.WriteLine($"Найдено: {item.GetInfo()}");
                }
                break;
            }
        case 10: //Показать элементы, выпущенные после указанной даты
            {
                int targetYear = ConsoleHelper.ReadInt("Введите год:");
                var recentItems = mediaLibrary.FindAll(item => item.Year >= targetYear);
                foreach (var item in recentItems)
                {
                    Console.WriteLine($"Найдено: {item.GetInfo()}");
                }
                break;
            }
        case 11:
            {
                mediaLibrary.Sort((a, b) => b.Year.CompareTo(a.Year));
                mediaLibrary.PrintAll();
                break;
            }
        case 12:
            {
                mediaLibrary.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine("✓ Коллекция отсортирована по названию (по алфавиту).");
                mediaLibrary.PrintAll();
                break;
            }
        case 13:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, который хотите добавить в корзину:");
                    mediaLibrary.AddToBasket(searchId);
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 14:
            {
                mediaLibrary.PrintAllBasket();
                break;
            }
        case 15:
            {
                int? threshold = ConsoleHelper.ReadOptionalInt("За сколько последних лет считать релиз новым?");

                int years = threshold ?? 2;

                var newReleases = mediaLibrary.FindAll(item => item.IsNewRelease(years));

                if (newReleases.Count == 0)
                    Console.WriteLine($"Новинок за последние {years} лет(а) не найдено.");
                else
                {
                    Console.WriteLine($"\nНайдено новинок за последние {years} лет(а): {newReleases.Count}");
                    foreach (var item in newReleases)
                        Console.WriteLine(item.GetInfo());
                }
                break;
            }
        case 16:
            {
                var (allItems, available, issued) = mediaLibrary.LibraryStats();
                Console.WriteLine($"Всего: {allItems}, Доступно: {available}, Выдано:{issued}");
                break;
            }
        case 18:
            {
                var criteria = new MediaSearchCriteria
                {
                    Title = ConsoleHelper.ReadOptionalString("Часть названия"),
                    Genre = ConsoleHelper.ReadGenre(),
                    MinYear = ConsoleHelper.ReadOptionalInt("Минимальный год выпуска"),
                };

                var result = mediaLibrary.Search(criteria);
                if (result.Count == 0)
                {
                    Console.WriteLine("Ничего не найдено");
                }
                else
                {
                    Console.WriteLine($"Найдено {result.Count} элементов");
                    foreach (var item in result)
                    {
                        Console.WriteLine($"{item.GetInfo()}"); 
                    }
                }
                break;
            }

        default:
            Console.WriteLine("Выберите корректный пункт меню (1-18).");
            break;
    }
}