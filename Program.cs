using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpleForecastConsoleApp
{
    class Program
    {
        static GlobalDistrictsClass DistrictsData;
        
        static void Main(string[] args)
        {
            GetLocationIds().Wait();
            Console.WriteLine("\nChoose an ID for the desired location forecast: ");
            string locationId = Console.ReadLine();
            GetWeather5DaysForecast(locationId).Wait();
        }


        static async Task GetLocationIds()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.ipma.pt/open-data/distrits-islands.json");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("|----------------------------------------------|");

                HttpResponseMessage response = await client.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    DistrictsData = await response.Content.ReadAsAsync<GlobalDistrictsClass>();

                    foreach (var item in DistrictsData.data)
                    {
                        Console.WriteLine($"\nID: {item.globalIdLocal} Location: {item.local}");
                    }
                    Console.WriteLine("\n|----------------------------------------------|");

                }
            }
        }



        static async Task GetWeather5DaysForecast(string locationId)
        {
            WeatherTypeClass weatherTypeList = null;

            //Get the WeatherTypes
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.ipma.pt/open-data/weather-type-classe.json");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("");
                if (response.IsSuccessStatusCode)
                {
                    weatherTypeList = await response.Content.ReadAsAsync<WeatherTypeClass>();
                }
            }

            //Get the forecast for 5 days
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.ipma.pt/open-data/forecast/meteorology/cities/daily/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"{locationId}.json");
                if (response.IsSuccessStatusCode)
                {
                    WeatherClass weatherData = await response.Content.ReadAsAsync<WeatherClass>();

                    Console.WriteLine("\n|----------------------------------------------|");

                    Console.WriteLine($"\nLocation: {DistrictsData.data.Find(x => x.globalIdLocal.Equals(weatherData.globalIdLocal)).local} \nLast Updated: {weatherData.dataUpdate}");

                    foreach (var item in weatherData.data)
                    {
                        Console.WriteLine($"\nDate: {item.forecastDate}"
                            + $"\n Min Temp: {item.tMin}\n Max Temp: {item.tMax}"
                            + $"\n Weather type: {weatherTypeList.data[item.idWeatherType].descIdWeatherTypeEN}"
                            + $"\n Wind Direction: {item.predWindDir}");
                    }

                    Console.WriteLine("\n|----------------------------------------------|");

                }
            }
        }
    }
}
