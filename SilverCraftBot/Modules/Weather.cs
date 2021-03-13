using Awesomio.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverCraftBot.Modules
{
    internal class Weather
    {
        private static WeatherClient client;

        public static WeatherClient GetClient()
        {
            return client;
        }

        public static void SetClient(WeatherClient wclient)
        {
            client = wclient;
        }
    }
}