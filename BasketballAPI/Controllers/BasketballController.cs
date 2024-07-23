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
        private readonly string _apiHost;

        public BasketballController(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _apiKey = configuration["ApiSettings:RapidApiKey"]; 
            _apiHost = configuration["ApiSettings:RapidApiHost"];
			//Log your API key and host key in your appsettings.json file
        }

        public async Task<IActionResult> Index(string season, string team_code)
        {
			//If the api does not have info for a season or a team it will return an empty list instead of logging an error.
            if(string.IsNullOrEmpty(season) || string.IsNullOrEmpty(team_code))
            {
                return View(new List<PlayerInfo>());
            }

            int teamId;
            try
            {
				//Gets the teamId from its repective team_code in the TeamMappings method
                teamId = TeamMappings.GetTeamID(team_code);
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<PlayerInfo>());
            } //If team code is not valid an error will be shown to users

			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"https://api-nba-v1.p.rapidapi.com/players/statistics?team={teamId}&season={season}"),
				Headers =
	            {
		            { "x-rapidapi-key", _apiKey },
		            { "x-rapidapi-host", _apiHost },
	            },
			};
			//Grabs the api results and stores it inside of the response variable to be parsed and manipulated. 
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
			var jsonResponse = JObject.Parse(responseBody);
            var playersArray = jsonResponse["response"] as JArray;

            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
			HashSet<string> processedPlayer = new HashSet<string>(); //Made to keep track of processed players

            foreach (var player in playersArray)
            {
                if (player["player"] == null || player["game"] == null)
                { continue; }

				if (player["fgm"] == null || player["tpm"] == null || player["assists"] == null || player["totReb"] == null || player["steals"] == null || player["turnovers"] == null || player["points"] == null)
				{ continue; } //skips over player if there is no significant data

				string playerName = player["player"]["firstname"].ToString() + " " + player["player"]["lastname"].ToString();
				if(processedPlayer.Contains(playerName) )
				{
					continue; //Skips over player if it has already been processed
				}

				PlayerInfo playerInfo = new PlayerInfo
				{
					Name = playerName ?? "Unknown",
					Position = player["pos"].ToString() ?? "Unknown",

					TwoPointersMade = player["fgm"].Value<double?>() ?? 0,
					ThreePointersMade = player["tpm"].Value<double?>() ?? 0,
					Assists = player["assists"].Value<double?>() ?? 0,
					Rebounds = player["totReb"].Value<double?>() ?? 0,
					Steals = player["steals"].Value<double?>() ?? 0,
					Turnovers = player["turnovers"].Value<double?>() ?? 0,
					Points = player["points"].Value<double?>() ?? 0,
					//Attempts to check if there are any null values, if there are the result will be 0 to avoid any errors.
				};

				if (playerInfo.Points > 0 || playerInfo.Assists > 0 || playerInfo.Rebounds > 0 || playerInfo.Steals > 0 || playerInfo.Turnovers > 0 || playerInfo.TwoPointersMade > 0 || playerInfo.ThreePointersMade > 0) //If any of the player stats is more than 0. They will be shown on list. 
				{
					playerInfos.Add(playerInfo);
					processedPlayer.Add(playerName); //Adds player after being processed
				}
			}
            return View(playerInfos);
        }
    }

    public static class TeamMappings
    {
        public static readonly Dictionary<string, int> TeamNameToId = new Dictionary<string, int>
        {
            {"ATL", 1},
            {"BOS", 2},
			{"BKN", 4},
			{"CHA", 5},
			{"CHI", 6},
			{"CLE", 7},
			{"DAL", 8},
			{"DEN", 9},
            {"DET",10},
			{"GS", 11},
			{"HOU", 14},
			{"IND", 15},
			{"LAC", 16},
			{"LAL", 17},
			{"MEM", 19},
			{"MIA", 20},
			{"MIL", 21},
			{"MIN", 22},
			{"NOP", 23},
			{"NYK", 24},
			{"OKC", 25},
            {"ORL",26},
			{"PHI", 27},
			{"PHX", 28},
			{"POR", 29},
			{"SAC", 30},
			{"SAS", 31},
			{"TOR", 38},
			{"UTAH", 40},
			{"WAS", 41},
		}; //User chooses which team they would like to see and their respective team identifier will be put into the API call "team_code"

        public static int GetTeamID(string teamCode)
        {
            if(TeamNameToId.TryGetValue(teamCode, out int teamId))
            {
                return teamId;
            }
            throw new ArgumentException($"This team code is not vaild: {teamCode}");
        }
    }

}
