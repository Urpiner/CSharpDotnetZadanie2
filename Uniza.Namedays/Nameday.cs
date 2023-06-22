using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public record struct Nameday
{
    public string Name { get; init; }
    public DayMonth DayMonth { get; init; }

    public Nameday(string name, DayMonth dayMonth)
    {
        Name = name;
        DayMonth = dayMonth;
    }

    public Nameday()
    {
        Name = "invalidName";
        DayMonth = new DayMonth();
    }

  


}

