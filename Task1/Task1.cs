using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }

    public void Print()
    {
        Console.WriteLine( Country + " " +
                           Name + " " +
                           Description + " " +
                           Temp );
    }
}
class Program
{
    static string getString( string ob, string key )
    {
        int Num = ob.IndexOf(key);
        if (Num == -1) return "";
        int Num2 = Num + key.Length + 3;
        int Num3 = ob.IndexOf('\"', Num2);
        return ob.Substring( Num2, Num3 - Num2 );
    }
    static double getDouble( string ob, string key )
    {
        int Num = ob.IndexOf(key);
        if (Num == -1) return 0.0;
        int Num2 = Num + key.Length + 2;
        char[] buf = { ',', '}' };
        int Num3 = ob.IndexOfAny(buf, Num2);
        double rez;
        double.TryParse( ob.Substring( Num2, Num3 - Num2 ),
                         NumberStyles.Any,
                         CultureInfo.InvariantCulture,
                         out rez );
        return rez;
    }
    static void Main( string[] args )
    {
        Random random = new Random();
        var Data = new List<Weather>();
        for (int counter = 0; counter < 20;)
        {
            string url = "https://api.openweathermap.org/data/2.5/weather?lat="
                       + (random.NextDouble() * 180 - 90)
                       + "&lon=" + (random.NextDouble() * 360 - 180)
                       + "&appid=<Your API Token>"; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            httpRequest.Accept = "application/json";

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader( httpResponse.GetResponseStream() ))
            {
                var result = streamReader.ReadToEnd();
                string country = getString(result, "country");
                string name = getString(result, "name");

                if (country == "" || name == "") continue;

                string description = getString(result, "description");
                double temp = getDouble(result, "temp");
                Weather weather = new Weather();
                weather.Country = country;
                weather.Name = name;
                weather.Description = description;
                weather.Temp = temp - 273;
                weather.Print();
                Data.Add( weather );
                counter++;
            }
        }

        //  Ищем максимальную температуру 
        var max = Data.Max(M => M.Temp);
        Console.WriteLine( "\nМаксимальная температура: " + max );
        var High = from M in Data
                   where M.Temp == max
                   select M;
        foreach (var M in High)
        {
            Console.WriteLine( M.Country + " " + M.Name );
        }

        //  Ищем минимальную температуру
        var min = Data.Min(m => m.Temp);
        Console.WriteLine( "\nМинимальная температура: " + min );
        var Low = from m in Data
                  where m.Temp == min
                  select m;
        foreach (var m in Low)
        {
            Console.WriteLine( m.Country + " " + m.Name );
        }

        //  Вычисляем количество уникальных стран
        var nameNotEq = Data.DistinctBy( x => x.Country );
        int count = nameNotEq.Count();
        Console.WriteLine( "\nКоличество уникальных стран: " + count );

        //  Вычисляем среднюю температуру
        var average = Data.Average( x => x.Temp );
        Console.WriteLine( "\nСредняя температура: " + average );

        //  Проверяем наличие элемента с облачностью clear sky
        bool flag = false;
        foreach (var x in Data)
            if (x.Description == "clear sky") flag = true;

        if (flag == true)
        {
            var first = Data.First( x => x.Description == "clear sky");
            Console.WriteLine( "\nПервый элемент с облачностью clear sky: " );
            first.Print();
        }
        else
        {
            Console.WriteLine( "\nЭлемент с облачностью clear sky отсутствует " );
        }
    }
}