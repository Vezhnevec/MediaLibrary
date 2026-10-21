using MediaLibrary.Enums;
using System.Text.RegularExpressions;

namespace MediaLibrary.Helpers
{
    public static class ConsoleHelper
    {
        public static string ReadString(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();
                Console.WriteLine("Значение не может быть пустым. Попробуйте снова.");
            }
        }

        public static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;
                Console.WriteLine("Ошибка: введите целое число.");
            }
        }

        public static Genre ReadGenre()
        {
            Console.WriteLine("Доступные жанры:");
            foreach (var genre in Enum.GetValues(typeof(Genre)))
            {
                Console.WriteLine($"  {(int)genre} - {genre}");
            }

            while (true)
            {
                Console.Write("Введите номер жанра: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int num) && Enum.IsDefined(typeof(Genre), num))
                    return (Genre)num;

                if (Enum.TryParse(input, true, out Genre parsedGenre))
                    return parsedGenre;

                Console.WriteLine("Некорректный жанр. Попробуйте снова.");
            }
        }
        public static string? ReadOptionalString(string prompt)
        {
            Console.WriteLine(prompt + " (или нажмите Enter для пропуска):");
            string? input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        public static int? ReadOptionalInt(string prompt)
        {
            Console.WriteLine(prompt + " (или нажмите Enter для пропуска):");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return null;

            if (int.TryParse(input, out int result)) return result;
            Console.WriteLine("Неверный формат, будет проигнорировано.");
            return null;
        }
        public static Genre? ReadOptionalGenre()
        {
            Console.WriteLine("Доступные жанры (или нажмите Enter для пропуска):");
            Console.WriteLine("  0 - Пропустить");
            foreach (var genre in Enum.GetValues(typeof(Genre)))
            {
                Console.WriteLine($"  {(int)genre + 1} - {genre}");
            }

            while (true)
            {
                Console.Write("Введите номер жанра: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    return null;

                if (int.TryParse(input, out int num))
                {
                    if (num == 0) return null;
                    // Учитываем сдвиг на +1 при парсинге
                    if (num >= 1 && num <= Enum.GetValues(typeof(Genre)).Length)
                        return (Genre)(num - 1);
                }

                if (Enum.TryParse(input, true, out Genre parsedGenre))
                    return parsedGenre;

                Console.WriteLine("Некорректный жанр. Попробуйте снова.");
            }
        }
        public static string ReadEmail(string prompt)
        {
            string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine().Trim();
                if (Regex.IsMatch(input, regex, RegexOptions.IgnoreCase))
                {
                    return input;
                }

                Console.WriteLine("Введен не корректный email");
            }
        }
        public static DateOnly ReadDate(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"{prompt} (формат: ДД.ММ.ГГГГ, например, 25.10.2023):");
                string input = Console.ReadLine()?.Trim();

                if (DateOnly.TryParseExact(input, "dd.MM.yyyy", out DateOnly result))
                {
                    return result;
                }

                string[] formats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "yyyyMMdd" };
                if (DateOnly.TryParseExact(input, formats, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out result))
                {
                    return result;
                }

                Console.WriteLine("Некорректный формат даты. Используйте ДД.ММ.ГГГГ.");
            }
        }
        public static TimeOnly ReadTime(string prompt)
        {
            while (true)
            {
                Console.WriteLine($"{prompt} (формат: ЧЧ:ММ, например, 12:20):");
                string input = Console.ReadLine()?.Trim();

                if (TimeOnly.TryParseExact(input, "HH:mm", out TimeOnly result))
                {
                    return result;
                }

                Console.WriteLine("Некорректный формат даты. Используйте ЧЧ:ММ.");
            }
        }
    }
}