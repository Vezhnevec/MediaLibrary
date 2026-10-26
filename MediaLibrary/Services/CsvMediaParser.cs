using MediaLibrary.Enums;
using MediaLibrary.Interface;
using System.Globalization;
using System.Text;

namespace MediaLibrary.Services
{
    public class CsvMediaParser
    {
        public List<IMedia> ParseFromFile(string filePath)
        {
            var items = new List<IMedia>();
            int errorCount = 0;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                reader.ReadLine();
                string line;
                int lineNumber = 1;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var item = ParseLine(line, lineNumber);

                        items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        errorCount++;
                    }
                }
            }
            Console.WriteLine($"Импорт завершен. Успешно: {items.Count}, Ошибок: {errorCount}");
            return items;
        }

        private IMedia ParseLine(string line, int lineNumber)
        {

            string[] parts = line.Split(';');
            if (parts.Length < 5)
                throw new FormatException($"Строка {lineNumber}: недостаточно полей");

            string type = parts[0];

            string title = UnescapeCsvField(parts[1]);

            if (!DateOnly.TryParseExact(parts[2], "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly releaseDate))
            {
                throw new FormatException($"Строка {lineNumber}: неверный формат даты '{parts[2]}'");
            }

            if (!int.TryParse(parts[3].Trim(), out int pageDuration))
            {
                throw new FormatException($"Строка {lineNumber}: неверное число '{parts[3]}'");
            }

            if (!Enum.TryParse<Genre>(parts[4].Trim(), ignoreCase: true, out Genre genre)
                || !Enum.IsDefined(typeof(Genre), genre))
            {
                throw new ArgumentException($"Строка {lineNumber}: неверный жанр '{parts[4]}'");
            }

            return type.ToLower() switch
            {
                "book" => new Book(title, genre, releaseDate, pageDuration),
                "movie" => new Movie(title, genre, releaseDate, pageDuration),
                "game" => new Game(title, genre, releaseDate),
                _ => throw new FormatException($"Неизвестный тип: {type}")
            };
        }

        private string UnescapeCsvField(string field)
        {
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                return field.Substring(1, field.Length - 2).Replace("\"\"", "\"");
            }
            return field;
        }
    }
}
