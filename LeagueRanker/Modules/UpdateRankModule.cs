using Discord.Commands;
using LeagueRanker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueRanker.Modules
{
    [DontAutoLoad]
    class UpdateRankModule : ModuleBase<SocketCommandContext>
    {
        private GetRankService _rankService;

        public UpdateRankModule(GetRankService _rankService)
        {
            this._rankService = _rankService;
        }

        //[Command("say")]
        //[Summary("Echos a message")]
        //public async Task SayAsync([Remainder] [Summary("The text you want to echo")] string echo)
        //{
        //    await ReplyAsync(echo);
        //}

        //[Command("magdonia")]
        //[Summary("Magdy lives")]
        //public async Task MagdoniaAsync()
        //{
        //    Console.WriteLine(Context.Channel.Id);
        //    await ReplyAsync("All hail magdonia");
        //}

        //[Command("boosted")]
        //[Summary("boosted lives")]
        //public async Task BoostedAsync()
        //{
        //    await ReplyAsync("A list of everyone who objects to Magdonia's regime!");
        //    await ReplyAsync("1. Alaa aka (Sak0ra)");
        //    await ReplyAsync("2. Vinnie aka (Fatgum)");
        //    await ReplyAsync("3. Plutinmo aka (C9 Meruem)");
        //}

        //[Command("rules")]
        //[Summary("Rules")]
        //public async Task RulesAsync()
        //{
        //    await ReplyAsync("Rules you must follow to be allowed under Magdonia's rule!");
        //    await ReplyAsync("1. NO PICS");
        //    await ReplyAsync("2. PLEASE");
        //}

        [Command("rank")]
        [Summary("Get summoner name league rank")]
        public async Task GetRankAsync([Remainder] [Summary("The summoner you want to get the rank for")] string summonerName)
        {
            await ReplyAsync(await _rankService.GetSummonerRank(summonerName));
        }


    }
}
