using Discord.Addons.Interactive;
using Discord.Commands;
using Fortnite_API;
using Fortnite_API.Objects.V1;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SilverCraftBot.Modules
{
    internal class Fortnite : InteractiveBase
    {
        private static FortniteApi api;

        /// <summary>
        /// fortite  mor lioke fartnite am i righe or am i righe
        /// </summary>
        /// <param name="apie"></param>
        public static void Setapi(FortniteApi apie)
        {
            api = apie;
        }

        [Command("fortstats", RunMode = RunMode.Async)]
        public async Task Stats([Remainder] string name)
        {
            Fortnite_API.Objects.ApiResponse<BrStatsV2V1> statsV2V1 = await api.V1.Stats.GetBrV2Async(x =>
            {
                //x.AccountId = "4735ce9132924caf8a5b17789b40f79c";
                x.Name = name;
                x.ImagePlatform = BrStatsV2V1ImagePlatform.All;
            });

            await ReplyAsync(statsV2V1.Data.Image.ToString());
        }

        [Command("fortbrnews", RunMode = RunMode.Async)]
        public async Task Brnews()
        {
            Fortnite_API.Objects.ApiResponse<Fortnite_API.Objects.V2.NewsV2Combined> newsV2 = await api.V2.News.GetAsync();

            await ReplyAsync(newsV2.Data.Br.Image.ToString());
        }

        [Command("fortcrnews", RunMode = RunMode.Async)]
        public async Task Crnews()
        {
            Fortnite_API.Objects.ApiResponse<Fortnite_API.Objects.V2.NewsV2Combined> newsV2 = await api.V2.News.GetAsync();

            await ReplyAsync(newsV2.Data.Creative.Image.ToString());
        }

        [Command("fortstwnews", RunMode = RunMode.Async)]
        public async Task Stwnews()
        {
            Fortnite_API.Objects.ApiResponse<Fortnite_API.Objects.V2.NewsV2Combined> newsV2 = await api.V2.News.GetAsync();

            await ReplyAsync(newsV2.Data.Stw.Image.ToString());
        }

        [Command("fortitms", RunMode = RunMode.Async)]
        public async Task Itm()
        {
            Fortnite_API.Objects.ApiResponse<Fortnite_API.Objects.V2.BrShopV2Combined> shop = await api.V2.Shop.GetBrCombinedAsync();
            StringBuilder sb = new StringBuilder();
            foreach (Fortnite_API.Objects.V2.BrShopV2StoreFrontEntry thing in shop.Data.Daily.Entries)
            {
                sb.Append(thing.DevName + thing.DisplayAssetPath + Environment.NewLine);
            }
            await ReplyAsync(sb.ToString());
        }
    }
}