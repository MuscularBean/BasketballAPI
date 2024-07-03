using BasketballAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
namespace BasketballAPI.Controllers
{
    public class BasketballController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public BasketballController(HttpClient client)
        {
            _client = client;
            _apiKey = Environment.GetEnvironmentVariable("API_KEY");
        }

        public async Task<IActionResult> Index(string season, string team_name)
        {
            if(string.IsNullOrEmpty(season) || string.IsNullOrEmpty(team_name))
            {
                return View(new List<PlayerInfo>());
            }
            
            var URL = $"https://api.sportsdata.io/v3/nba/stats/json/PlayerSeasonStatsByTeam/{season}/{team_name}?key={_apiKey}";
            var response = await _client.GetStringAsync(URL);

            if (string.IsNullOrEmpty(response))
            {
                return View(new List<PlayerInfo>());
            }

            var playersArray = JArray.Parse(response);

            List<PlayerInfo> playerInfos = new List<PlayerInfo>();

            string teamLogoUrl = "";

            foreach (var player in playersArray)
            {
                PlayerInfo playerInfo = new PlayerInfo
                {
                    Name = player["Name"].ToString(),
                    Position = player["Position"].ToString(),

                    TwoPointersMade = player["TwoPointersMade"].Value<double>(),
                    ThreePointersMade = player["ThreePointersMade"].Value<double>(),
                    Assists = player["Assists"].Value<double>(),
                    Rebounds = player["Rebounds"].Value<double>(),
                    Steals = player["Steals"].Value<double>(),
                    Turnovers = player["Turnovers"].Value<double>(),
                    Points = player["Points"].Value<double>(),
                    Games = player["Games"].Value<double>(),
                   /* TeamLogoUrl = teamLogoUrl */
                };

                playerInfos.Add(playerInfo);
            }
            return View(playerInfos);
        }
    }
}
