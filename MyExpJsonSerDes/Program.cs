using System;
using System.Collections.Generic;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace JsonExp
{
    class Program
    {
        public class WeatherForecast
        {
            public DateTimeOffset Date { get; set; }
            public int TemperatureCelsius { get; set; }
            public string? Summary { get; set; }

            private bool _is_cool;

            public WeatherForecast(DateTimeOffset date, int TemperatureCelsius, string Summary) 
            {
                this.Date = date;
                this.TemperatureCelsius = TemperatureCelsius;
                this.Summary = Summary;
                _is_cool = this.TemperatureCelsius < 22;
            }
        }


        static void Main(string[] args)
        {
            var weatherForecast = new WeatherForecast(DateTime.Parse("2025-08-23"), 30, "Hot");

            string jsonString = JsonSerializer.Serialize(weatherForecast, new JsonSerializerOptions { WriteIndented = true });

            const string PATH = "weather.json";

            if (!File.Exists(PATH))
            {
                File.Create(PATH);
            }

            File.WriteAllText(PATH, jsonString);

            string from_file = File.ReadAllText(PATH);
            if (!string.IsNullOrEmpty(from_file))
            {
                WeatherForecast weather = JsonSerializer.Deserialize<WeatherForecast>(from_file);
            }
        }
    }
}

