using MediaLibrary.Enums;
using MediaLibrary.Interface;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using MediaLibrary.Models;

namespace MediaLibrary.Services
{
    public class CsvMediaParser
    {
        public async IAsyncEnumerable<ImportResult> ParseFromFileAsync(string filePath, [EnumeratorCancellation] CancellationToken ct = default)
        {
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                await reader.ReadLineAsync(ct);

                string? line;
                int lineNumber = 1;

                while ((line = await reader.ReadLineAsync(ct)) != null)
                {
                    lineNumber++;
                    ct.ThrowIfCancellationRequested();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var result = TryParseLineDetailed(line, lineNumber);

                    yield return result;
                }
            }
        }

        private ImportResult TryParseLineDetailed(string line, int lineNumber)
        {

            string[] parts = line.Split(';');
            if (parts.Length < 5)
                return ImportResult.Failure(lineNumber, "Недостаточно полей");

            string type = parts[0].Trim();
            string title = UnescapeCsvField(parts[1]);

            if (!DateOnly.TryParseExact(parts[2], "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly releaseDate))
                return ImportResult.Failure(lineNumber, $"Неверный формат даты: {parts[2]}");

            if (!int.TryParse(parts[3].Trim(), out int pageDuration))
                return ImportResult.Failure(lineNumber, $"Неверное число: {parts[3]}");

            if (!Enum.TryParse<Genre>(parts[4].Trim(), ignoreCase: true, out Genre genre)
                || !Enum.IsDefined(typeof(Genre), genre))
                return ImportResult.Failure(lineNumber, $"Неизвестный жанр: {parts[4]}");

            try
            {
                IMedia item = type.ToLowerInvariant() switch
                {
                    "book" => new Book(title, genre, releaseDate, pageDuration),
                    "movie" => new Movie(title, genre, releaseDate, pageDuration),
                    "game" => new Game(title, genre, releaseDate),
                    _ => throw new FormatException($"Неизвестный тип: {type}")
                };

                return ImportResult.Success(lineNumber, item);
            }
            catch (Exception ex)
            {
                return ImportResult.Failure(lineNumber, ex.Message);
            }
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
