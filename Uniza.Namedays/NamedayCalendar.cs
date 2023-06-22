using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

public class NamedayCalendar : IEnumerable<Nameday>
{
    private List<Nameday> _namedays = new List<Nameday>();

    public int NameCount => _namedays.Count;
    public int DayCount => _namedays.Select(n => n.DayMonth).Distinct().Count();

    public DayMonth? this[string name]
    {
        get
        {
            foreach (Nameday nameday in _namedays)
            {
                if (nameday.Name == name)
                {
                    return nameday.DayMonth;
                }
            }

            return null;
        }
    }

    public string[] this[DayMonth dayMonth] => GetNamesForDayMonth(dayMonth).ToArray();
    public string[] this[DateTime dateTime] => GetNamesForDayMonth(new DayMonth(dateTime.Day, dateTime.Month)).ToArray();
    public string[] this[DateOnly date] => GetNamesForDayMonth(new DayMonth(date.Day, date.Month)).ToArray();
    public string[] this[int day, int month] => GetNamesForDayMonth(new DayMonth(day, month)).ToArray();
    public string[] this[int dayOfYear] => GetNamesForDayMonth(GetDayMonthFromDayOfYear(dayOfYear)).ToArray();

    public IEnumerator<Nameday> GetEnumerator() => _namedays.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<Nameday> GetNamedays() => _namedays;

    public IEnumerable<Nameday> GetNamedays(int month) => _namedays.Where(n => n.DayMonth.Month == month);

    public IEnumerable<Nameday> GetNamedays(string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return _namedays.Where(n => regex.IsMatch(n.Name));
    }

    //metoda prida nameday do _namedays tak, aby _namedays bolo usporiadane podla datumu
    public void Add(Nameday nameday)
    {
        if (_namedays.Any(n => n.Name == nameday.Name))
        {
            throw new ArgumentException($"Nameday for {nameday.Name} already exists.");
        }

        int index = 0;
        while (index < _namedays.Count && _namedays[index].DayMonth.Month < nameday.DayMonth.Month)
        {
            index++;
        }
        while (index < _namedays.Count && _namedays[index].DayMonth.Day <= nameday.DayMonth.Day)
        {
            index++;
        }

        _namedays.Insert(index, nameday);
    }

    public void Add(int day, int month, params string[] names)
    {
        var dayMonth = new DayMonth(day, month);
        foreach (var name in names)
        {
            Add(new Nameday(name, dayMonth));
        }
    }

    public void Add(DayMonth dayMonth, params string[] names)
    {
        foreach (var name in names)
        {
            Add(new Nameday(name, dayMonth));
        }
    }

    public bool Remove(string name)
    {
        Nameday nameday = new Nameday();
        bool found = false;
        foreach (Nameday item in _namedays)
        {
            if (item.Name == name)
            {
                nameday = item;
                found = true;
            }
        }

        if (found)
        {
            _namedays.Remove(nameday);
            return true;
        }

        return false;
    }

    public bool Contains(string name) => _namedays.Any(n => n.Name == name);

    private IEnumerable<string> GetNamesForDayMonth(DayMonth dayMonth)
    {
        return _namedays.Where(n => n.DayMonth == dayMonth).Select(n => n.Name);
    }

    private DayMonth GetDayMonthFromDayOfYear(int dayOfYear)
    {
        return new DayMonth(DateTime.MinValue.AddDays(dayOfYear - 1).Day, DateTime.MinValue.AddDays(dayOfYear - 1).Month);
    }

    public void Load(FileInfo csvFile)
    {
        Clear();

        using (var reader = new StreamReader(csvFile.FullName))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var columns = line.Split(';');
                if (columns.Length > 1)
                {
                    var date = DateTime.ParseExact(columns[0], "d. M.", CultureInfo.InvariantCulture);
                    DayMonth dayMonth = new DayMonth(date.Day, date.Month);

                    var names = columns.Skip(1).Where(n => n != "-" && n != "").Select(n => n.Trim()).ToArray();
                    foreach (var name in names)
                    {
                        var nameday = new Nameday(name, dayMonth);
                        _namedays.Add(nameday);
                    }                 
                    
                }

            }
        }
    }
    public void Save(FileInfo csvFile)
    {
        using (var writer = new StreamWriter(csvFile.FullName))
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);

            //pre kazdy den naplnim pole namesCurrDate s menami a tie sa ulozia do jedneho riadku v csvFile
            int index = 0;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                List<string> namesCurrDate = new List<string>(); 
                int offset = 0;
                while (index + offset < _namedays.Count() && date.Month == _namedays.ElementAt(index + offset).DayMonth.Month && date.Day == _namedays.ElementAt(index + offset).DayMonth.Day)
                {
                    namesCurrDate.Add(_namedays.ElementAt(index + offset).Name);
                    offset++;
                }
                index = index + offset;


                if (namesCurrDate.Count() > 0)
                {
                    string dateStr = $"{date.Day}. {date.Month}.";
                    string namesStr = string.Join(";", namesCurrDate.ToArray());
                    writer.WriteLine($"{dateStr};{namesStr}");
                }
                else //ak nikto nema v aktualnom dni meniny, tak sa ulozi riadok: [datum];-
                {
                    string dateStr = $"{date.Day}. {date.Month}.";
                    writer.WriteLine($"{dateStr};-");
                }


            }
        }
    }

    public void Clear()
    {
        _namedays.Clear();
    }





}


