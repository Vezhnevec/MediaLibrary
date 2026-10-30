using MediaLibrary.Enums;
using MediaLibrary.Exceptions;
using MediaLibrary.Extensions;
using MediaLibrary.Helpers;
using MediaLibrary.Criteria;
using MediaLibrary.Queue;
using MediaLibrary.Models;
using MediaLibrary.Interface;
using MediaLibrary.Services;
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
    Console.WriteLine("17. Показать категорию элемента");
    Console.WriteLine("18. Расширенный поиск");
    Console.WriteLine("19. Показать эпоху элемента");
    Console.WriteLine("20. Анализ корзины");
    Console.WriteLine("21. Показать приоритет элемента");
    Console.WriteLine("22. Показать количество элементов по жанрам");
    Console.WriteLine("23. Показать последние просмотренные элементы");
    Console.WriteLine("24. Показать очередь ожидания");
    Console.WriteLine("25. Оформить выдачу корзины (Чек)");
    Console.WriteLine("26. Проверить срок возврата элемента");
    Console.WriteLine("27. Добавить объект в кинопоказ");
    Console.WriteLine("28. Показать расписание кинопоказа");
    Console.WriteLine("29. Сформировать отчет за период");
    Console.WriteLine("30. Узнать точный возраст элемента");
    Console.WriteLine("31. Показать статистику");
    Console.WriteLine("32. Импорт из CSV");
    Console.WriteLine("33. Установить рейтинг");
    Console.WriteLine("34. Топ-N");
    Console.WriteLine("35. Состояние библиотеки");
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
                DateOnly releaseDate = ConsoleHelper.ReadDate("Дата релиза:");
                int pages = ConsoleHelper.ReadInt("Количество страниц:");

                Book book = new Book(title, genre, releaseDate, pages);
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
                DateOnly releaseDate = ConsoleHelper.ReadDate("Дата релиза:");
                int duration = ConsoleHelper.ReadInt("Длительность (мин):");

                Movie movie = new Movie(title, genre, releaseDate, duration);
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
                DateOnly releaseDate = ConsoleHelper.ReadDate("Дата релиза:");
                string platform = ConsoleHelper.ReadString("Платформа (PC, PS5, Xbox и т.д.):");

                Game game = new Game(title, genre, releaseDate, platform);
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
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            catch (ItemNotAvailableException ex)
            {
                string userInput = ConsoleHelper.ReadString("Элемент выдан. Хотите встать в очередь ожидания? (да/нет)");
                if (userInput.Equals("да", StringComparison.OrdinalIgnoreCase))
                {
                    var lendRequest = new LendRequest()
                    {
                        ReaderName = ConsoleHelper.ReadString("Укажите своё имя"),
                        ItemId = lendId,
                        Email = ConsoleHelper.ReadEmail("Введите email")
                    };
                    mediaLibrary.AddToWaitList(lendRequest);
                    Console.WriteLine("Вы добавлены в очередь");
                }
                //Console.WriteLine($"✗ {ex.Message}");
            }
            break;

        case 7: // Вернуть элемент
            int returnId = ConsoleHelper.ReadInt("Введите ID элемента для возврата:");
            try
            {
                mediaLibrary.Return(returnId);

                if (mediaLibrary.TryProcessWaitlist(returnId, out var satisfiedRequest))
                {
                    Console.WriteLine($"✓ Элемент возвращен. Уведомление отправлено: {satisfiedRequest.ReaderName} (ждал именно эту позицию).");
                }
                else
                {
                    Console.WriteLine("✓ Элемент успешно возвращён в общий доступ!");
                }
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
                var recentItems = mediaLibrary.FindAll(item => item.ReleaseDate.Year >= targetYear);
                foreach (var item in recentItems)
                {
                    Console.WriteLine($"Найдено: {item.GetInfo()}");
                }
                break;
            }
        case 11:
            {
                mediaLibrary.Sort((a, b) => b.ReleaseDate.CompareTo(a.ReleaseDate));
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
                    mediaLibrary.AddToCart(searchId);
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 14:
            {
                mediaLibrary.PrintAllCart();
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
        case 17:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, у которого хотите узнать категорию:");
                    Console.WriteLine(mediaLibrary.GetCategory(mediaLibrary[searchId]));
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 18:
            {
                var criteria = new MediaSearchCriteria
                {
                    Title = ConsoleHelper.ReadOptionalString("Часть названия"),
                    Genre = ConsoleHelper.ReadOptionalGenre(),
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
        case 19:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, у которого хотите узнать эпоху:");
                    var item = mediaLibrary.FindById(searchId);

                    string era = item.GetEraByYear();

                    Console.WriteLine($"'{item.Title}' ({item.ReleaseDate.Year} г.) относится к эпохе {era}");
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 20:
            {
                Console.WriteLine(mediaLibrary.GetCartSummary());
                break;
            }
        case 21:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, у которого хотите узнать приоритет:");
                    var item = mediaLibrary.FindById(searchId);

                    string priority = mediaLibrary.GetPriority(item);

                    Console.WriteLine($"'{item.Title}' имеет приоритет: {priority}");
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 22:
            {
                var stasisticGenre = mediaLibrary.GetGroupedByGenre();

                foreach (var item in stasisticGenre)
                {
                    Console.WriteLine($"Жанр: {item.Key} = {item.Value}");
                }
                break;
            }
        case 23:
            {
                var result = mediaLibrary.GetRecentlyViewed();
                foreach (var item in result)
                {
                    Console.WriteLine($"{item.GetInfo()}");
                }
                break;
            }
        case 24:
            {
                mediaLibrary.PrintWaitList();
                break;
            }
        case 25:
            {
                Console.WriteLine(mediaLibrary.GenerateCartReceipt());
                break;
            }
        case 26:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, у которого хотите проверить срок возврата:");
                    Console.WriteLine(mediaLibrary.CheckOverdueStatus(searchId));
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 27:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID фильма, коорый хотите добавить в кинопоказ:");
                    var item = mediaLibrary.FindById(searchId);

                    if (item is Movie movie)
                    {
                        var screening = new Screening
                        {
                            MovieId = movie.Id,
                            MovieTitle = movie.Title,
                            Date = ConsoleHelper.ReadDate("Введите дату начала показа"),
                            StartTime = ConsoleHelper.ReadTime("Введите время начала показа"),
                            DurationMinutes = movie.Duration
                        };
                        mediaLibrary.AddScreening(screening);
                    }
                    else
                    {
                        Console.WriteLine("Этот элемент не являеттся фильмом");
                    }

                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 28:
            {
                Console.WriteLine(mediaLibrary.PrintScreeningSchedule());
                break;
            }
        case 29:
            {
                try
                {
                    DateOnly startDate = ConsoleHelper.ReadDate("Введите начальную дату");
                    DateOnly endDate = ConsoleHelper.ReadDate("Введите конечную дату");
                    Console.WriteLine(mediaLibrary.GenerateActivityReport(startDate, endDate));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Неверный формат даты: {ex.Message}");
                }
                break;
            }
        case 30:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, у которого хотите узнать точный возраст:");
                    var item = mediaLibrary.FindById(searchId);

                    var age = item.GetDetailedAge();

                    Console.WriteLine($"Выпущено {age.Years} года, {age.Months} месяцев и {age.Days} дней назад");
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 31:
            {
                using var manualCts = new CancellationTokenSource();
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(manualCts.Token, timeoutCts.Token);

                var ct = linkedCts.Token;

                ConsoleCancelEventHandler? cancelHandler;
                cancelHandler = (sender, e) =>
                {
                    e.Cancel = true;

                    manualCts.Cancel();
                    Console.WriteLine("Получен сигнал Ctrl+C. Отмена загрузки статистики...");
                };

                try
                {
                    Console.CancelKeyPress += cancelHandler;

                    Console.WriteLine("Загрузка статистики... (нажмите 'Ctrl+C' для отмены или ждите 10 секунд)");

                    var stats = await mediaLibrary.ShowStatisticsAsync(ct);

                    Console.WriteLine(stats);
                }
                catch (OperationCanceledException)
                {
                    if (timeoutCts.IsCancellationRequested)
                        Console.WriteLine("Время ожидания истекло. Операция прервана");
                    else
                        Console.WriteLine("Загрузка статистики отменена пользователем");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
                finally
                {
                    Console.CancelKeyPress -= cancelHandler;
                }
                break;
            }
        case 32:
            {
                using var cts = new CancellationTokenSource();
                var ct = cts.Token;

                int addedCount = 0;
                int skippedCount = 0;
                int errorCount = 0;
                int count = 0;

                ConsoleCancelEventHandler cancelHandler = (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                    Console.WriteLine("Отмена импорта");
                };

                try
                {
                    var parser = new CsvMediaParser();

                    string filePath = ConsoleHelper.ReadString("Введите путь до файла");
                    Console.CancelKeyPress += cancelHandler;

                    await foreach (var result in parser.ParseFromFileAsync(filePath, ct))
                    {
                        count++;

                        if (result.IsSuccess)
                        {
                            if (result.Item is MediaItem mediaItem)
                            {
                                if (mediaLibrary.TryAdd(mediaItem))
                                {
                                    addedCount++;                                    
                                }
                                else
                                {
                                    Console.WriteLine($"Элемент «{mediaItem.Title}» уже есть в библиотеке — пропущен");
                                    skippedCount++;
                                }
                            }
                            else if (result.Item is not null)
                            {
                                Console.WriteLine($"Пропущен элемент '{result.Item.Title}', не является элементом библиотеки");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Строка {result.LineNumber}: {result.ErrorMessage}");
                            errorCount++;
                        }
                        if (count % 10 == 0)
                        {
                            Console.WriteLine($"Обработано {count} элементов...");

                        }
                    }

                    Console.WriteLine($"\nИмпорт завершен. Добавлено: {addedCount}, Пропущено: {skippedCount}, Ошибок: {errorCount}");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Импорт отменён. Добавлено: {addedCount}, Пропущено: {skippedCount}, Ошибок: {errorCount}");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Файл не найден");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка! Текст ошибки: {ex.Message}");
                }
                finally
                {
                    Console.CancelKeyPress -= cancelHandler;
                }
                break;
            }
        case 33:
            {
                try
                {
                    int searchId = ConsoleHelper.ReadInt("Введите ID элемента, у которого хотите изменить рейтинг(от 1 до 10):");
                    double rating = ConsoleHelper.ReadDouble("Какой рейтинг хотите установить?");
                    if (mediaLibrary.SetRating(searchId, rating))
                    {
                        Console.WriteLine("Рейттинг установлен");
                    }
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"✗ {ex.Message}");
                }
                break;
            }
        case 34:
            {
                int count = ConsoleHelper.ReadInt("Топ из скольки позиций хотите отобразить?");
                Console.WriteLine(mediaLibrary.GetTopRated(count));
                break;
            }
        case 35:
            {
                Console.WriteLine(mediaLibrary.PrintLibraryHealth());
                break;
            }
        default:
            Console.WriteLine("Выберите корректный пункт меню (1-32).");
            break;
    }
}