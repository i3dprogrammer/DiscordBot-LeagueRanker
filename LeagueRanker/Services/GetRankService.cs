using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;

namespace LeagueRanker.Services
{
    class GetRankService
    {
        private RiotApi _api;
        private HttpClient _client;
        private string _leaguesUrl = "https://eun1.api.riotgames.com/lol/league/v3/positions/by-summoner/{0}?api_key={1}";
        private JsonSerializer _serializer;
        private string _api_key;

        public GetRankService()
        {
            _api_key = ConfigurationManager.AppSettings["riot_api_key"];
            _api = RiotApi.GetDevelopmentInstance(_api_key);
            _client = new HttpClient();
            _serializer = new JsonSerializer();
        }

        public async Task<string> GetSummonerRank(string summonerName)
        {
            try
            {
                var summoner = await _api.GetSummonerByNameAsync(RiotSharp.Misc.Region.eune, summonerName);
                // BUGGED
                //var leagues = await _api.GetLeaguesAsync(RiotSharp.Misc.Region.eune, summoner.Id);

                // MANUALL
                var data = await _client.GetStringAsync(string.Format(_leaguesUrl, summoner.Id, _api_key));
                var leagues = JsonConvert.DeserializeObject<List<RiotSharp.LeagueEndpoint.League>>(data);

                // Get highest rank in flex, solo, 3v3
                var retValue = "";
                foreach (var league in leagues)
                {
                    if (retValue == "" || (LeagueTiers)Enum.Parse(typeof(LeagueTiers), league.Tier.ToString()) > (LeagueTiers)Enum.Parse(typeof(LeagueTiers), retValue))
                    {
                        retValue = league.Tier.ToString();
                    }
                }
                return retValue;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            return "";
        }

        private enum LeagueTiers
        {
            Bronze,
            Silver,
            Gold,
            Platinum,
            Diamond,
            Master,
            Challenger
        }
    }
}
