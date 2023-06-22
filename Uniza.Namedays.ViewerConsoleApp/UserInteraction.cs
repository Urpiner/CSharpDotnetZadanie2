using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace Uniza.Namedays.ViewerConsoleApp
{
    internal class UserInteraction
    {
        private NamedayCalendar _calendar;

        public UserInteraction()
        {
            _calendar = new NamedayCalendar();
            _calendar.Load(new FileInfo(@"..\..\..\..\Uniza.Namedays\data\namedays-sk.csv"));
        }

        public UserInteraction(FileInfo csvFile)
        {
            _calendar = new NamedayCalendar();
            _calendar.Load(new FileInfo(csvFile.FullName));
        }

        public void ShowMenu()
        {
            Console.Clear();
            PrintBasicInfo();

            int functionalitiesCount = 6;
            Console.WriteLine("Menu");
            Console.WriteLine("1 - načítať kalendár");
            Console.WriteLine("2 - zobraziť štatistiku");
            Console.WriteLine("3 - vyhľadať mená");
            Console.WriteLine("4 - vyhľadať mená podľa dátumu");
            Console.WriteLine("5 - zobraziť kalendár mien v mesiaci");
            Console.WriteLine("6 | Escape - koniec");
            Console.Write("Vaša voľba: ");

            int choice = GetChoiceFromUser(functionalitiesCount);

            switch (choice)
            {
                case 1:
                    LoadCalendar();
                    break;
                case 2:
                    ShowStatistics();
                    break;
                case 3:
                    FindNames();
                    break;
                case 4:
                    FindNamesByDate();
                    break;
                case 5:
                    ShowCalendar();
                    break;
                case 6:
                    //koniec programu
                    break;
                default:
                    break;
            }
        }

        private void PrintBasicInfo()
        {
            string[] todayNamedays = _calendar[DateTime.Now];
            string[] tomorrowNamedays = _calendar[DateTime.Now.AddDays(1)];

            Console.WriteLine("KALENDÁR MIEN");
            
            if (todayNamedays.Length == 0)
            {
                Console.Write($"Dnes {DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year} nemá nikto meniny.");
            }
            if (todayNamedays.Length > 0 )
            {
                Console.Write($"Dnes má {DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year} meniny: ");
                for (int i = 0; i < todayNamedays.Length; i++)
                {
                    if (i == todayNamedays.Length - 1)
                    {
                        Console.WriteLine($"{todayNamedays[i]}");
                    }
                    else
                    {
                        Console.Write($"{todayNamedays[i]}, ");
                    }
                }
            }

            if (tomorrowNamedays.Length == 0)
            {
                Console.Write($"Zajtra {DateTime.Now.AddDays(1).Day}.{DateTime.Now.AddDays(1).Month}.{DateTime.Now.AddDays(1).Year} nemá nikto meniny.");
            }
            if (tomorrowNamedays.Length > 0)
            {
                Console.Write($"Zajtra má {DateTime.Now.AddDays(1).Day}.{DateTime.Now.AddDays(1).Month}.{DateTime.Now.AddDays(1).Year} meniny: ");
                for (int i = 0; i < tomorrowNamedays.Length; i++)
                {
                    if (i == tomorrowNamedays.Length - 1)
                    {
                        Console.WriteLine($"{tomorrowNamedays[i]}");
                    }
                    else
                    {
                        Console.Write($"{tomorrowNamedays[i]}, ");
                    }
                }
            }

        }

        private int GetChoiceFromUser(int choicesCount)
        {
            int choice;
            string? input = ReadInputFromConsole();
            if (input == null) //uzivatel stlacil escape
            {
                input = "6";
            }

            while (!int.TryParse(input, out choice) || choice < 1 || choice > choicesCount)
            {
                Console.Write("Nesprávny input. Zadajte číslo od 1-6: ");
                input = ReadInputFromConsole();
                if (input == null)
                {
                    input = "6";
                }
            }

            return choice;
        }

        // specialne znaky:
        // Escape (signal na koniec programu) - metoda vrati null
        // Backspace - vymaze 1 znak
        private string? ReadInputFromConsole()
        {
            string input = "";
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return null;
                }

                // backspace vymaze pismeno z user inputu (aby sa to spravalo podobne ako Console.Readline())
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }

                    continue;
                }

                char pressedCharacter = keyInfo.KeyChar;
                Console.Write(pressedCharacter);
                input += pressedCharacter;
            }

            Console.WriteLine();
            return input;
        }

        private void LoadCalendar()
        {
            Console.Clear();
            Console.WriteLine("OTVORENIE");
            Console.WriteLine("Zadajte cestu k súboru kalendára mien alebo stlačte Enter pre ukončenie.");
            Console.Write("Zadajte cestu k CSV súboru: ");

            string? input = Console.ReadLine();
            if (input == "")
            {
                ShowMenu();
            }
            string? filePath = input;

            while (!File.Exists(filePath))
            {
                Console.WriteLine($"Zadaný súbor {filePath} neexistuje!");
                Console.Write("Zadajte cestu k CSV súboru: ");
                input = Console.ReadLine();
                if (input == "")
                {
                    ShowMenu();
                    break;
                }
                filePath = input;
            }

            if (filePath != null)
            {
                _calendar.Load(new FileInfo(filePath));
            }
            Console.WriteLine("Súbor kalendára bol načítaný.");
            Console.WriteLine("Pre pokračovanie stlačte Enter");
            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }

            ShowMenu();
        }

        private void ShowStatistics()
        {
            Console.Clear();
            Console.WriteLine("ŠTATISTIKA");
            Console.WriteLine($"Celkový počet mien v kalendári: {_calendar.NameCount}");

            Console.WriteLine($"Celkový počet dní obsahujúci mená v kalendári: {_calendar.DayCount}");

            Console.WriteLine("Celkový počet mien v jednotlivých mesiacoch: ");
            Console.WriteLine($"Január: {_calendar.GetNamedays(1).Count()}");
            Console.WriteLine($"Február: {_calendar.GetNamedays(2).Count()}");
            Console.WriteLine($"Marec: {_calendar.GetNamedays(3).Count()}");
            Console.WriteLine($"Apríl: {_calendar.GetNamedays(4).Count()}");
            Console.WriteLine($"Máj: {_calendar.GetNamedays(5).Count()}");
            Console.WriteLine($"Jún: {_calendar.GetNamedays(6).Count()}");
            Console.WriteLine($"Júl: {_calendar.GetNamedays(7).Count()}");
            Console.WriteLine($"August: {_calendar.GetNamedays(8).Count()}");
            Console.WriteLine($"September: {_calendar.GetNamedays(9).Count()}");
            Console.WriteLine($"Október: {_calendar.GetNamedays(10).Count()}");
            Console.WriteLine($"November: {_calendar.GetNamedays(11).Count()}");
            Console.WriteLine($"December: {_calendar.GetNamedays(12).Count()}");

            Console.WriteLine("Počet mien podľa začiatočných písmen: ");
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                Console.WriteLine($"{letter}: {_calendar.GetNamedays($"^{letter}").Count()}");
            }

            Console.WriteLine("Počet mien podľa dĺžky znakov: ");
            Dictionary<int, int> statistics = new Dictionary<int, int>(); //dlzka retazca -> Key ... pocet retazcov s danou dlzkou -> Value
            foreach (Nameday nameday in _calendar.GetNamedays())
            {
                int length = nameday.Name.Length;
                if (statistics.ContainsKey(length))
                {
                    statistics[length]++;
                }
                else
                {
                    statistics[length] = 1; //este dlzka retazca nebola zaregistrovana, vlozim ju ako Key a nastavim Value na 1
                }
            }
            foreach (var item in statistics.OrderBy(entry => entry.Key))
            {
                int length = item.Key;
                int count = item.Value;
                Console.WriteLine($"{length}: {count}");
            }

            Console.WriteLine("Pre pokračovanie stlačte Enter");
            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
            }
            ShowMenu();
        }

        private void FindNames()
        {
            Console.Clear();
            Console.WriteLine("VYHĽADÁVANIE MIEN");
            Console.WriteLine("Pre ukončenie stlačte Enter");

            string? input = "";
            while (true)
            {
                Console.Write("Zadajte meno (alebo regex): ");

                input = Console.ReadLine();

                if (input == null || input == "")
                {
                    ShowMenu();
                    break;
                }
                else if (_calendar.GetNamedays(input).Count() == 0)
                {
                    Console.WriteLine("Neboli nájdené žiadne mená");
                }
                else
                {
                    int index = 1;
                    foreach (var item in _calendar.GetNamedays(input))
                    {
                        Console.WriteLine($"{index}. {item.Name} ({item.DayMonth.Day}. {item.DayMonth.Month})");
                        index++;
                    }
                }
            }
        }

        private void FindNamesByDate()
        {
            Console.Clear();
            Console.WriteLine("VYHĽADÁVANIE MIEN PODĽA DÁTUMU");
            Console.WriteLine("Pre ukončenie stlačte Enter");
            Console.WriteLine("Formát dátumu: [deň].[mesiac] (npr.: 24.2)");


            string? input = "";
            while (true)
            {
                Console.Write("Zadajte dátum: ");
                input = Console.ReadLine();
                if (input == null || input == "")
                {
                    ShowMenu();
                    break;
                }

                string format = "d.M";
                DateTime date;
                if (DateTime.TryParseExact(input, format, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    if (_calendar[date].Count() == 0)
                    {
                        Console.WriteLine("Neboli nájdené žiadne mená");
                    }
                    else
                    {
                        int index = 1;
                        foreach (var name in _calendar[date])
                        {
                            Console.WriteLine($"{index}. {name}");
                            index++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Nesprávny formát dátumu.");
                }
            }
        }

        private void ShowCalendar()
        {
            DateTime date = DateTime.Now;
            PrintNamedaysInMonth(date.Month);

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                ConsoleKey pressedKey = keyInfo.Key;

                if (pressedKey == ConsoleKey.Home || pressedKey == ConsoleKey.D)
                {
                    date = DateTime.Now;
                    PrintNamedaysInMonth(date.Month);
                }
                else if (pressedKey == ConsoleKey.Enter)
                {
                    ShowMenu();
                    break;
                }
                else if (pressedKey == ConsoleKey.LeftArrow)
                {
                    date = date.AddMonths(-1);
                    PrintNamedaysInMonth(date.Month);
                }
                else if (pressedKey == ConsoleKey.RightArrow)
                {
                    date = date.AddMonths(1);
                    PrintNamedaysInMonth(date.Month);
                }
            }

        }

        private void PrintNamedaysInMonth(int month)
        {
            Console.Clear();
            Console.WriteLine("KALENDÁR MENÍN");
            Console.WriteLine("Šípka doľava / doprava - mesiac dopredu / dozadu");
            Console.WriteLine("Kláves Home alebo D - aktuálny deň");
            Console.WriteLine("Pre ukončenie stlačte Enter");

            foreach (var nameday in _calendar.GetNamedays(month))
            {
                string dayOfWeek = nameday.DayMonth.ToDateTime().ToString("dddd", new CultureInfo("sk-SK"));
                string abbreviatedDay = dayOfWeek.Substring(0, 2);
                if (nameday.DayMonth.Day == DateTime.Now.Day && nameday.DayMonth.Month == DateTime.Now.Month)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{nameday.DayMonth.Day}.{nameday.DayMonth.Month} {abbreviatedDay} {nameday.Name}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (abbreviatedDay == "so" || abbreviatedDay == "ne")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{nameday.DayMonth.Day}.{nameday.DayMonth.Month} {abbreviatedDay} {nameday.Name}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine($"{nameday.DayMonth.Day}.{nameday.DayMonth.Month} {abbreviatedDay} {nameday.Name}");
                }
            }
        }
    }
}
