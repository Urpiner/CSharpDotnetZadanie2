using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniza.Namedays.ViewerConsoleApp;

internal class Program
{
    static public void Main(String[] args)
    {
        if (args.Length == 0) 
        {
            UserInteraction ui = new UserInteraction();
            ui.ShowMenu();
        }
        else if (args.Length == 1)
        {
            string filePath = args[0];
            if (File.Exists(filePath))
            {
                UserInteraction ui = new UserInteraction(new FileInfo(filePath));
                ui.ShowMenu();
            }
            else
            {
                Console.WriteLine("Zadaná cesta k súboru je neplatná.");
            }
        }
        else
        {
            Console.WriteLine("Neplatný počet argumentov.");
        }
        
    }


}

