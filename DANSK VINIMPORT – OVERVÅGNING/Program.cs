using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.CompilerServices;


namespace DANSK_VINIMPORT___OVERVÅGNING
{
    class Program
    {
        static void Main()
        {
            DVIService.monitorSoapClient ds = new DVIService.monitorSoapClient();

            // Ny timer object der opdatere hver 5 minut
            Timer timer = new Timer(UpdateDisplay, ds, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            

            Console.ReadLine();
        }

        static void UpdateDisplay(object state)
        {
            DVIService.monitorSoapClient ds = (DVIService.monitorSoapClient)state;

            // Opdater temperature/humidity 
            UpdateTemperatureAndHumidity(ds);

            // Opdater stock status 
            UpdateStockStatus(ds);

            Console.Clear();
        }
        static void skriv(int x, int y, string s)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(s);
        }
        static void UpdateTemperatureAndHumidity(DVIService.monitorSoapClient ds)
        {
            Console.Clear(); // Clear konsol før opdatering

            // Opsætning af Temperatur/Fugtighed
            Console.ForegroundColor = ConsoleColor.Cyan;
            skriv(30, 0, "TEMPERATUR/FUGTIGHED");

            skriv(0, 2, "TEMPERATUR OG FUGTIGHED I LAGER: ");

            skriv(50, 2, "TEMPERATUR OG FUGTIGHED UDENFOR: ");

            Console.ResetColor();

            skriv(0, 3, "Lager Temperatur: ");

            skriv(18, 3, ds.StockTemp().ToString("N2") + "°C");

            skriv(50, 3, "Temperatur Udenfor: ");

            skriv(70, 3, ds.OutdoorTemp().ToString("N") + "°C");
            
            skriv(0, 4, "Lager Fugtighed: ");
            
            skriv(17, 4, ds.StockHumidity().ToString("N2") + "%");

            skriv(50, 4, "Fugtighed Udenfor: ");

            skriv(69, 4, ds.OutdoorHumidity().ToString("N2") + "%");
            Console.ForegroundColor = ConsoleColor.Cyan;

            skriv(0, 6, "---------------------------------------------------------------------------------");
        }

        static void UpdateStockStatus(DVIService.monitorSoapClient ds)
        {
            // Opsætning af Lagerstatus
            Console.ForegroundColor = ConsoleColor.Cyan;

            skriv(30, 7, "LAGER STATUS");

            // Varer Under Minimum Opsætning
            skriv(0, 9 , "Varer Under Minimum: ");
            Console.ForegroundColor = ConsoleColor.Red;
            int y = Console.CursorTop;
            foreach (string line in ds.StockItemsUnderMin()) 
            {
                skriv(20, y, line);
                y++;
            }
          
            // Varer Over Maksimum Opsætning
            Console.ForegroundColor = ConsoleColor.Cyan;

            skriv(0, 11, "Varer Over Maksimum: ");

            Console.ForegroundColor = ConsoleColor.Green;

            foreach (string line in ds.StockItemsOverMax()) 
            {
                skriv(20, y, line);
                y++;
            }
            // Mest Solgte I Dag
            Console.ForegroundColor = ConsoleColor.Cyan;

            skriv(0, 13, "Mest Solgte I Dag: ");

            Console.ResetColor();

            foreach (string line in ds.StockItemsMostSold())
            {
                skriv(20, y, line);
                y++;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;

            skriv(0,16, "---------------------------------------------------------------------------------");
            
            // Clear only the section displaying the date/time
            int dateSectionStartLine = Console.CursorTop;

            skriv(30, 18, "DATO/TID");

            Console.ResetColor();

            // Tidszone id for hver by, Hold styr på hvilken by ligger i hvilken tidszone.
            // timeZoneId1 er til København, timeZoneId2 er til London, timeZoneId3 er til Singapore.
            string timeZoneId1 = "Central European Standard Time"; // Første by.
            string timeZoneId2 = "GMT Standard Time"; // Anden by.
            string timeZoneId3 = "Singapore Standard Time"; // Tredje by.

            while (true)
            {

                // Bevæg cursor til starten
                Console.SetCursorPosition(0, dateSectionStartLine);


                Console.ResetColor();
                

                // Finder den nuværende tid.
                DateTime now = DateTime.UtcNow;

                // Får tidszone info for den første by.
                TimeZoneInfo timeZone1 = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId1);
                DateTime byTid1 = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone1);

                // Får tidszone info for den anden by.
                TimeZoneInfo timeZone2 = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId2);
                DateTime byTid2 = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone2);

                // Får tidszone info for den tredje by.
                TimeZoneInfo timeZone3 = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId3);
                DateTime byTid3 = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone3);

                // Formaterer tidszonerne til tekst stringe
                // dddd = Dag på ugen, MMMM = Måned, dd = Dag på måneden, yyyy = År, HH = Timer, mm = Minutter og ss = Sekunder.
                string formattedDateTime1 = byTid1.ToString("dddd, dd-MMMM, yyyy, HH:mm:ss");
                string formattedDateTime2 = byTid2.ToString("dddd, dd-MMMM, yyyy, HH:mm:ss");
                string formattedDateTime3 = byTid3.ToString("dddd, dd-MMMM, yyyy, HH:mm:ss");
                // Gør det første bogstav i tekst stringen til stort.
                formattedDateTime1 = char.ToUpper(formattedDateTime1[0]) + formattedDateTime1.Substring(1);
                formattedDateTime2 = char.ToUpper(formattedDateTime2[0]) + formattedDateTime2.Substring(1);
                formattedDateTime3 = char.ToUpper(formattedDateTime3[0]) + formattedDateTime3.Substring(1);

                // Vise nuværende tider i de forskellige byer.
                skriv(0, 20, "København: " + formattedDateTime1);

                skriv(0, 22, "London: " + formattedDateTime2);

                skriv(0, 24, "Singapore: " + formattedDateTime3);

                // Venter et sekund før den opdateres.
                Thread.Sleep(1000);
            }

        }
    }
}
