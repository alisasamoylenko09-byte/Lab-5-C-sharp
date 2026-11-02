using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab5Variant5
{
    // Власне виключення для перевірки даних виставки
    public class InvalidExhibitionDataException : Exception
    {
        public InvalidExhibitionDataException(string message) : base(message) { }
    }

    // Абстрактний базовий клас (Завдання 2. А)
    public abstract class Exhibition
    {
        protected string name;
        protected string artistLastName;

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidExhibitionDataException("Назва виставки не може бути порожньою.");
                name = value;
            }
        }

        public string ArtistLastName
        {
            get => artistLastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidExhibitionDataException("Прізвище художника не може бути порожнім.");
                artistLastName = value;
            }
        }

        // Конструктор базового класу
        public Exhibition(string name, string artistLastName)
        {
            Name = name;
            ArtistLastName = artistLastName;
        }

        // Віртуальний метод 
        public virtual void DisplayInfo()
        {
            Console.Write($"Виставка: {Name}, Художник: {ArtistLastName} | ");
        }

        // Абстрактні методи для індивідуального завдання (Завдання 2. Б)
        public abstract void CalculateTotalVisitors(List<Day> days);
        public abstract void FindMinVisitorsDay(List<Day> days);
        public abstract void FindCommentsWithWord(List<Day> days, string word);
    }

    // Похідний клас (Сутність "День" згідно таблиці варіанту 5)
    public class Day : Exhibition
    {
        private string dayName; // День виставки
        private int visitorsCount;
        private string comment;

        public string DayName
        {
            get => dayName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidExhibitionDataException("День виставки не може бути порожнім.");
                dayName = value;
            }
        }

        public int VisitorsCount
        {
            get => visitorsCount;
            set
            {
                if (value < 0)
                    throw new InvalidExhibitionDataException("Кількість відвідувачів не може бути від'ємною.");
                visitorsCount = value;
            }
        }

        public string Comment
        {
            get => comment;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidExhibitionDataException("Коментар не може бути порожнім.");
                comment = value;
            }
        }

        // Конструктор похідного класу з використанням base
        public Day(string name, string artistLastName, string dayName, int visitorsCount, string comment)
            : base(name, artistLastName)
        {
            DayName = dayName;
            VisitorsCount = visitorsCount;
            Comment = comment;
        }

        // Перевизначення віртуального методу
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"День: {DayName}, Відвідувачів: {VisitorsCount}, Коментар: {Comment}");
        }

        // Перетворення у рядок для збереження у файл
        public string ToFileString()
        {
            return $"{Name};{ArtistLastName};{DayName};{VisitorsCount};{Comment}";
        }

        // Створення об'єкта з рядка файлу
        public static Day FromFileString(string line)
        {
            string[] parts = line.Split(';');
            if (parts.Length == 5)
            {
                return new Day(parts[0], parts[1], parts[2], int.Parse(parts[3]), parts[4]);
            }
            throw new FormatException("Невірний формат рядка у файлі.");
        }

        // Реалізація абстрактних методів індивідуального завдання у похідному класі

        public override void CalculateTotalVisitors(List<Day> days)
        {
            int total = 0;
            foreach (var day in days)
            {
                total += day.VisitorsCount;
            }
            Console.WriteLine($"\n[Результат] Сумарна кількість відвідувачів за всі дні: {total}");
        }

        public override void FindMinVisitorsDay(List<Day> days)
        {
            if (days == null || days.Count == 0)
            {
                Console.WriteLine("Список порожній!");
                return;
            }

            Day minDay = days[0];
            foreach (var day in days)
            {
                if (day.VisitorsCount < minDay.VisitorsCount)
                {
                    minDay = day;
                }
            }
            Console.WriteLine($"[Результат] День з найменшою кількістю відвідувачів: виставка \"{minDay.Name}\" ({minDay.ArtistLastName}), день: {minDay.DayName}, відвідувачів: {minDay.VisitorsCount}, коментар: \"{minDay.Comment}\"");
        }

        public override void FindCommentsWithWord(List<Day> days, string word)
        {
            Console.WriteLine($"\n[Результат] Список коментарів, що містять слово \"{word}\":");
            bool found = false;
            foreach (var day in days)
            {
                if (day.Comment.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine($"- \"{day.Comment}\" (Виставка: {day.Name}, День: {day.DayName})");
                    found = true;
                }
            }
            if (!found)
            {
                Console.WriteLine("Коментарів з таким словом не знайдено.");
            }
        }
    }

    class Program
    {
        static string filePath = "exhibitions.txt";

        // Створення початкового файлу з 10 записами, якщо він відсутній
        static void EnsureFile()
        {
            if (!File.Exists(filePath))
            {
                StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
                sw.WriteLine("Світло і тінь;Петренко;Понеділок;120;Чудова виставка сучасного мистецтва");
                sw.WriteLine("Барви осені;Коваленко;Вівторок;45;Майже нікого не було, але атмосферно");
                sw.WriteLine("Сучасний Київ;Шевченко;Середа;310;Неймовірні емоції та картини");
                sw.WriteLine("Побут і мрії;Іванов;Четвер;85;Цікаві експонати, раджу відвідати");
                sw.WriteLine("Подих вітру;Мельник;П'ятниця;15;Дуже мало відвідувачів сьогодні");
                sw.WriteLine("Стежки Карпат;Сидоренко;Субота;190;Фантастичні пейзажі та колорит");
                sw.WriteLine("Нічне місто;Бондар;Неділя;60;Цікава гра світла і тіней на полотнах");
                sw.WriteLine("Абстракція душі;Гриценко;Понеділок;230;Незрозуміло, але дуже захопливо");
                sw.WriteLine("Морський бриз;Романюк;Вівторок;95;Нагадує про тепле літо та відпочинок");
                sw.WriteLine("Тиша ранку;Олійник;Середа;30;Спокійна атмосфера та приємна музика");
                sw.Close();
            }
        }

        // Завантаження всіх записів з файлу
        static List<Day> LoadFromFile()
        {
            List<Day> list = new List<Day>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        list.Add(Day.FromFileString(line));
                    }
                }
            }
            return list;
        }

        // Збереження всіх записів у файл (повне оновлення файлу)
        static void SaveToFile(List<Day> list)
        {
            StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
            foreach (var item in list)
            {
                sw.WriteLine(item.ToFileString());
            }
            sw.Close();
        }

        // Допоміжний метод для точного вирівнювання тексту з кирилицею в консолі
        static string AlignText(string text, int width)
        {
            if (text == null) text = "";
            if (text.Length > width)
            {
                text = text.Substring(0, width - 3) + "...";
            }

            // Рахуємо фактичну довжину з урахуванням специфіки кириличних літер в Windows
            int spacesNeeded = width - text.Length;
            if (spacesNeeded < 0) spacesNeeded = 0;

            return text + new string(' ', spacesNeeded);
        }

        // Метод виведення ідеально рівної таблиці
        static void PrintTable(List<Day> list)
        {
            int wName = 18;
            int wArtist = 14;
            int wDay = 12;
            int wVisitors = 12;
            int wComment = 42;

            int totalWidth = wName + wArtist + wDay + wVisitors + wComment + 16;

            string line = new string('-', totalWidth);

            Console.WriteLine(line);
            Console.WriteLine($"| {AlignText("Назва виставки", wName)} | {AlignText("Художник", wArtist)} | {AlignText("День", wDay)} | {AlignText("Відвідувачі", wVisitors)} | {AlignText("Коментар", wComment)} |");
            Console.WriteLine(line);

            foreach (var item in list)
            {
                Console.WriteLine($"| {AlignText(item.Name, wName)} | {AlignText(item.ArtistLastName, wArtist)} | {AlignText(item.DayName, wDay)} | {AlignText(item.VisitorsCount.ToString(), wVisitors)} | {AlignText(item.Comment, wComment)} |");
            }

            Console.WriteLine(line);
        }

        // Метод додавання запису
        static void AddRecord(List<Day> list)
        {
            Console.Write("Введіть назву виставки: ");
            string name = Console.ReadLine();
            Console.Write("Введіть прізвище художника: ");
            string artist = Console.ReadLine();
            Console.Write("Введіть день виставки (наприклад, Понеділок): ");
            string dayName = Console.ReadLine();
            Console.Write("Введіть кількість відвідувачів: ");
            int visitors = int.Parse(Console.ReadLine());
            Console.Write("Введіть коментар: ");
            string comment = Console.ReadLine();

            Day newItem = new Day(name, artist, dayName, visitors, comment);
            list.Add(newItem);
            SaveToFile(list);
            Console.WriteLine("Запис успішно додано та збережено у файл!");
        }

        // Метод редагування запису
        static void EditRecord(List<Day> list)
        {
            PrintTable(list);
            Console.Write($"Введіть номер запису для редагування (1 - {list.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= list.Count)
            {
                Console.Write("Введіть нову назву виставки: ");
                string name = Console.ReadLine();
                Console.Write("Введіть нове прізвище художника: ");
                string artist = Console.ReadLine();
                Console.Write("Введіть новий день виставки: ");
                string dayName = Console.ReadLine();
                Console.Write("Введіть нову кількість відвідувачів: ");
                int visitors = int.Parse(Console.ReadLine());
                Console.Write("Введіть новий коментар: ");
                string comment = Console.ReadLine();

                list[idx - 1] = new Day(name, artist, dayName, visitors, comment);
                SaveToFile(list);
                Console.WriteLine("Запис успішно оновлено та збережено у файл!");
            }
            else
            {
                Console.WriteLine("Невірний номер запису!");
            }
        }

        // Метод видалення запису
        static void DeleteRecord(List<Day> list)
        {
            PrintTable(list);
            Console.Write($"Введіть номер запису для видалення (1 - {list.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= list.Count)
            {
                list.RemoveAt(idx - 1);
                SaveToFile(list);
                Console.WriteLine("Запис успішно видалено та файл оновлено!");
            }
            else
            {
                Console.WriteLine("Невірний номер запису!");
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            EnsureFile();

            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=================== Миколаївська галерея ===================");
                Console.WriteLine("[п] - Переглянути записи з файлу");
                Console.WriteLine("[д] - Додати новий запис");
                Console.WriteLine("[р] - Редагувати запис");
                Console.WriteLine("[в] - Видалити запис");
                Console.WriteLine("[с] - Розрахувати сумарну кількість відвідувачів");
                Console.WriteLine("[м] - Знайти день з найменшою кількістю відвідувачів");
                Console.WriteLine("[к] - Знайти коментарі за певним словом");
                Console.WriteLine("[х] - Вихід з програми");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine().Trim().ToLower();
                Console.WriteLine();

                try
                {
                    List<Day> list = LoadFromFile();
                    Exhibition helper = list.Count > 0 ? list[0] : new Day("Тест", "Тест", "Тест", 0, "тест");

                    if (choice == "х")
                    {
                        isRunning = false;
                        Console.WriteLine("Дякую за використання програми! До побачення.");
                    }
                    else if (choice == "п")
                    {
                        PrintTable(list);
                    }
                    else if (choice == "д")
                    {
                        AddRecord(list);
                    }
                    else if (choice == "р")
                    {
                        EditRecord(list);
                    }
                    else if (choice == "в")
                    {
                        DeleteRecord(list);
                    }
                    else if (choice == "с")
                    {
                        helper.CalculateTotalVisitors(list);
                    }
                    else if (choice == "м")
                    {
                        helper.FindMinVisitorsDay(list);
                    }
                    else if (choice == "к")
                    {
                        Console.Write("Введіть слово для пошуку в коментарях: ");
                        string word = Console.ReadLine();
                        helper.FindCommentsWithWord(list, word);
                    }
                    else
                    {
                        Console.WriteLine("Невірний вибір! Використовуйте українські літери меню.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Сталася помилка: {ex.Message}");
                }
            }
        }
    }
}