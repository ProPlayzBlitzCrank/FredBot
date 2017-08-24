using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using static FredtheG.CactusBot.WeatherDataCurrent;

namespace FredtheG.CactusBot
{
    public class WeatherData : ModuleBase
    {
        public async Task<String> GetWeatherAsync(string city)
        {
            var httpClient = new HttpClient();
            string URL = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=55ff8dedf42f55e12479145a2e4fefa4";
            var response = await httpClient.GetAsync(URL);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        [Command("weather")]
        [Alias("Weather")]
        [Summary("Tells you weather on a location")]
        public async Task WeatherAsync([Remainder] string city = null)
        {
            if (city == null)
            {
                await ReplyAsync($"{Context.User.Mention} you need to enter a city.");
                return;
            }
            WeatherReportCurrent weather;
            weather = JsonConvert.DeserializeObject<WeatherReportCurrent>(GetWeatherAsync(city).Result);
            double longi = weather.Coord.Lon;
            double lati = weather.Coord.Lat;
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };
            embed.AddField(y =>
            {
                y.Name = "Coordinates";
                y.Value = $"Longitude: **{longi}**\nLatitude: **{lati}**";
                y.IsInline = false;
            });
            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
