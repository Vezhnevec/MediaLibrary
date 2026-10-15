using MediaLibrary.Enums;

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
                    return input;
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
                Console.WriteLine($"  {(int)genre + 1} - {genre}"); // Сдвинули на 1, чтобы 0 был пропуском
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
    }
}