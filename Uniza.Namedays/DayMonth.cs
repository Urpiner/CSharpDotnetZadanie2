using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public record struct DayMonth
{
    public int Day { get; init; }
    public int Month { get; init; }

    public DayMonth(int day, int month)
    {
        Day = day;
        Month = month;
    }

    public DayMonth()
    {
        Day = 1;
        Month = 1;
    }

    public DateTime ToDateTime()
    {
        return new DateTime(DateTime.Now.Year, Month, Day);
    }
}

