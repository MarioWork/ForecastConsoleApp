using System.Collections.Generic;

namespace SimpleForecastConsoleApp{
    class WeatherClass{
        public List<WeatherDatesContentClass> data { get; set; }

        public int globalIdLocal { get; set; }

        public string dataUpdate { get; set; }
    }
}