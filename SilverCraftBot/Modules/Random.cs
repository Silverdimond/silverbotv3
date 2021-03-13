using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using dotnetcorebot.Modules;
using SIlverCraftBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverCraftBot.Modules
{
    internal class RandomCommands : InteractiveBase
    {
        [Command("someone", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.MentionEveryone)]
        public async Task Someone()
        {
            Random rand = new Random();
            await ReplyAsync(Context.Guild.Users.ElementAt(rand.Next(0, Context.Guild.MemberCount)).Mention);
        }

        [Command("random string e", RunMode = RunMode.Async)]
        public async Task Rstrge(int lenght)
        {
            EmbedBuilder b = new EmbedBuilder();
            string oi = Commands.RandomString(lenght);
            string oie;
            try
            {
                oie = Regex.Replace(oi, @"[^\w]", "",
                                      RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                oie = string.Empty;
            }
            b.WithFooter(Commands.GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
            b.WithTitle("RSTRGE");
            b.WithDescription(oie);
            await ReplyAsync(oi, embed: b.Build());
        }

        [Command("random string", RunMode = RunMode.Async)]
        public async Task Rstrg(int lenght)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithFooter(Commands.GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
            b.WithTitle("RSTRG");
            b.WithDescription(Commands.RandomString(lenght));
            await ReplyAsync(embed: b.Build());
        }

        [Command("random tld", RunMode = RunMode.Async)]
        public async Task Rdomain(string ee)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithFooter(Commands.GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
            b.WithTitle("Random tld");
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://data.iana.org/TLD/tlds-alpha-by-domain.txt");
            string[] vs = (await rm.Content.ReadAsStringAsync()).Split('\n');
            RandomGenerator randomGenerator = new RandomGenerator();
            try
            {
                b.WithDescription(ee + "." + vs[randomGenerator.Next(1, vs.Length)]);
            }
            catch (ArgumentOutOfRangeException e)
            {
                b.WithDescription(e.Message);
            }
            await ReplyAsync(embed: b.Build());
        }

        [Command("random int", RunMode = RunMode.Async)]
        public async Task RINT(int min, int max)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithFooter(Commands.GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
            b.WithTitle("RINT");
            RandomGenerator randomGenerator = new RandomGenerator();
            try
            {
                b.WithDescription(randomGenerator.Next(min, max).ToString());
            }
            catch (ArgumentOutOfRangeException e)
            {
                b.WithDescription(e.Message);
            }
            await ReplyAsync(embed: b.Build());
        }
    }
}