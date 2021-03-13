using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using dotnetcorebot.Modules.infoclasses;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using LiteDB;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SilverCraftBot.Modules;
using SIlverCraftBot;
using SIlverCraftBot.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SilverBotData;
using SilverBotLiteData;

namespace dotnetcorebot.Modules
{
    public partial class Commands : InteractiveBase
    {
        private static Giphy giphy = new Giphy();

        public static string GetUserAvatarUrl(SocketUser user)
        {
            if (string.IsNullOrEmpty(user.GetAvatarUrl()))
            {
                return user.GetDefaultAvatarUrl();
            }
            return user.GetAvatarUrl();
        }

        [Command("eval", RunMode = RunMode.Async)]
        public async Task Eval([Remainder] string fekc)
        {
            if (dconfig.Botowners.Contains(Context.User.Id))
            {
                object thing = await CSharpScript.EvaluateAsync(fekc);
                await ReplyAsync(thing.GetType().FullName);
                await ReplyAsync("```" + thing.ToString() + "```");
            }
        }

        [Command("evald", RunMode = RunMode.Async)]
        public async Task Evals([Remainder] string fekc)
        {
            if (dconfig.Botowners.Contains(Context.User.Id))
            {
                ScriptState<object> script = await CSharpScript.RunAsync(fekc, ScriptOptions.Default.WithReferences("System.Net.Http"));

                await ReplyAsync(script.ReturnValue.GetType().FullName);
                await ReplyAsync("```" + script.ReturnValue.ToString() + "```");
            }
        }

        [Command("git", RunMode = RunMode.Async), Alias("github", "the hub", "source code", "code", "DEVELOPER SHIT")]
        public async Task Github()
        {
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://silverdimond.tk/silvercraftbot/version-info.txt");
            string _content = await rm.Content.ReadAsStringAsync();
            string[] strings = _content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length != 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oh oh someone made an oopsie making the strings not 3. they are curently " + strings.Length);
                Console.ResetColor();
            }
            if (strings.Length >= 3)
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle($"Github");
                b.WithDescription(strings[2]);
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("welcomechannel", RunMode = RunMode.Async)]
        public async Task Welcomechannel(SocketChannel e)
        {
            bool isatsilvercraft = Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(Context.User.Id);
                isbotadmin = Is_bot_admin(socketGuildus);
            }
            if (isbotadmin)
            {
                SilverBotData.Serverthing newserverthing = new SilverBotData.Serverthing
                {
                    ServerId = Context.Guild.Id,
                    ChannelId = e.Id
                };
                SilverBotLiteData.Serverthing.Insert(newserverthing);
                EmbedBuilder b = new EmbedBuilder();
                await ReplyAsync(embed: b.Build());
            }
        }

        public static void Giphymodule(Giphy dgiphy)
        {
            giphy = dgiphy;
        }

        [Command("dukt hosting", RunMode = RunMode.Async), Alias("dukthosting", "ducthosting", "duct hosting", "duck hosting", "duckhosting")]
        public async Task Dukt()
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            b.WithTitle(lang.Silverhosting_joke_title);
            b.WithDescription(lang.Silverhosting_joke_description);
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("spawn griefer jesus", RunMode = RunMode.Async), Alias("spawn extreme griefer jesus", "griefer jesus", "extreme griefer jesus", "our god is an awsome god", "jesus")]
        public async Task Jesus()
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("Our God is an Awesome God");
            b.WithUrl("https://www.youtube.com/watch?v=Wdo4Eyw4DqM");
            b.WithDescription("Our God is an awesome God" + Environment.NewLine + "He reigns from heaven above" + Environment.NewLine + "With wisdom, power, and love" + Environment.NewLine + "Our God is an awesome God");
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("c#errcode", RunMode = RunMode.Async)]
        public async Task Csharperror(string error)
        {
            bool NuGetError = false;
            bool DotNetError = false;
            bool CsharpError = false;
            bool FsharpError = false;
            bool VbError = false;
            try
            {
                NuGetError = Regex.IsMatch(error, @"NU[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                DotNetError = Regex.IsMatch(error, @"CA[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                CsharpError = Regex.IsMatch(error, @"CS[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                FsharpError = Regex.IsMatch(error, @"FS[0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            try
            {
                VbError = Regex.IsMatch(error, @"BC[0-9][0-9][0-9][0-9][0-9]");
            }
            catch (RegexMatchTimeoutException)
            {
            }
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("test");
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            if (NuGetError)
            {
                b.WithDescription("https://docs.microsoft.com/en-us/nuget/reference/errors-and-warnings/" + error);

                await ReplyAsync(embed: b.Build());
                return;
            }
            if (DotNetError)
            {
                b.WithDescription("https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/" + error);
                await ReplyAsync(embed: b.Build());
                return;
            }
            if (CsharpError)
            {
                b.WithDescription("https://docs.microsoft.com/en-us/dotnet/csharp/misc/" + error);
                await ReplyAsync(embed: b.Build());
                return;
            }
            if (FsharpError)
            {
                b.WithDescription("Your using f# lmao so the docs kinda suck you should google the error sorry man https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/compiler-messages/" + error);
                await ReplyAsync(embed: b.Build());
                return;
            }
            if (VbError)
            {
                b.WithDescription("https://docs.microsoft.com/en-us/dotnet/visual-basic/misc/" + error);
                await ReplyAsync(embed: b.Build());
                return;
            }
        }

        [Command("test", RunMode = RunMode.Async)]
        public async Task Test(string file)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("test");
            Language language = Language.Get(file);
            b.WithDescription(language.Test);
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("languagetemplate", RunMode = RunMode.Async)]
        [Alias("langtemplate")]
        public async Task Langtemplate()
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("Ok");
            Language language = new Language();
            JsonSerializerOptions f = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            Stream e = new MemoryStream(Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(language, f)));
            b.WithDescription("silverbot language support is like not that important and will probalbly not be implemented until 2069 or something");
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await Context.Channel.SendFileAsync(e, "languagetemplate.json", embed: b.Build());
        }

        public static Language GetLanguage(ulong gid)
        {
            using (LiteDatabase db = new LiteDatabase(@"Filename=serverlang.db; Connection=shared"))
            {
                ILiteCollection<ServerLanguage> col = db.GetCollection<ServerLanguage>();
                col.EnsureIndex(x => x.GuildID);
                ServerLanguage thign = col.FindOne(x => x.GuildID == gid)!;
                if (thign != null)
                {
                    return Language.Get(thign.ISO_Code);
                }
            }
            return new Language();
        }

        public static string RandomString(int length)
        {
            using RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            byte[] tokenData = new byte[length];
            rng.GetBytes(tokenData);

            return Convert.ToBase64String(tokenData);
        }

        [Command("warning", RunMode = RunMode.Async)]
        public async Task Viewwarning([Remainder] string id)
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            using LiteDatabase db = new LiteDatabase(@"Filename=infraction.db; Connection=shared");
            ILiteCollection<ServerInfractions> col = db.GetCollection<ServerInfractions>();
            col.EnsureIndex(x => x.Server_id);
            ServerInfractions thign = col.FindOne(x => x.Server_id == Context.Guild.Id)!;
            if (thign != null)
            {
                Infraction find = thign.Infractions.Find(x => x.Id == id);
                if (find != null)
                {
                    b.WithTitle("Warning string" + find.Id);
                    b.WithDescription("<@!" + find.User_id + "> was warned by string <@!" + find.Punisher_id + ">");
                    b.AddField("reason string", find.Reason, true);
                    b.AddField("with attachment num string", find.Attachment_urls.Count);
                    foreach (string attachement in find.Attachment_urls)
                    {
                        b.AddField("attachemt", attachement);
                    }
                    b.AddField("date string", find.Time.ToShortDateString());
                    await ReplyAsync(embed: b.Build());
                }
                else
                {
                    b.WithTitle("no warning found with id string");
                    await ReplyAsync(embed: b.Build());
                }
            }
            else
            {
                b.WithTitle("no warnings string");
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("warn", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireContext(ContextType.Guild)]
        public async Task Warn(SocketGuildUser e, string reason)
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));

            using LiteDatabase db = new LiteDatabase(@"Filename=infraction.db; Connection=shared");
            ILiteCollection<ServerInfractions> col = db.GetCollection<ServerInfractions>();
            col.EnsureIndex(x => x.Server_id);
            ServerInfractions thign = col.FindOne(x => x.Server_id == Context.Guild.Id)!;
            if (thign != null)
            {
                Infraction infraction = new Infraction
                {
                    Attachment_urls = new List<string>()
                };
                foreach (Attachment attachment in Context.Message.Attachments)
                {
                    infraction.Attachment_urls.Add(attachment.ProxyUrl);
                }
                infraction.Punisher_id = Context.User.Id;
                infraction.Reason = reason;
                string oi = RandomString(7);
                try
                {
                    oi = Regex.Replace(oi, @"[^\w]", "",
                                          RegexOptions.None, TimeSpan.FromSeconds(1.5));
                }
                // If we timeout when replacing invalid characters,
                // we should return Empty.
                catch (RegexMatchTimeoutException)
                {
                    oi = string.Empty;
                }
                infraction.Id = oi;
                infraction.User_id = e.Id;
                infraction.Time = DateTime.UtcNow;
                thign.Infractions.Add(infraction);
                b.WithTitle(lang.Warn_title);
                b.WithDescription(e.Mention + lang.Warn_desc_middle + Context.User.Mention);
                b.AddField(lang.Warn_field_reason, reason, true);
                b.WithCurrentTimestamp();
                b.AddField(lang.Warn_field_id, "`" + infraction.Id + "`", true);
                await ReplyAsync(embed: b.Build());
                col.Update(thign);
            }
            else
            {
                ServerInfractions inf = new ServerInfractions
                {
                    Server_id = Context.Guild.Id,
                    Infractions = new List<Infraction>()
                };
                Infraction infraction = new Infraction
                {
                    Attachment_urls = new List<string>()
                };
                foreach (Attachment attachment in Context.Message.Attachments)
                {
                    infraction.Attachment_urls.Add(attachment.ProxyUrl);
                }
                infraction.Punisher_id = Context.User.Id;
                infraction.Reason = reason;
                infraction.User_id = e.Id;
                string oi = RandomString(7);
                try
                {
                    oi = Regex.Replace(oi, @"[^\w]", "",
                                          RegexOptions.None, TimeSpan.FromSeconds(1.5));
                }
                // If we timeout when replacing invalid characters,
                // we should return Empty.
                catch (RegexMatchTimeoutException)
                {
                    oi = string.Empty;
                }
                infraction.Id = oi;
                infraction.Time = DateTime.UtcNow;
                inf.Infractions.Add(infraction);
                b.WithTitle(lang.Warn_title);
                b.WithDescription(e.Mention + lang.Warn_desc_middle + Context.User.Mention);
                b.AddField(lang.Warn_field_reason, reason, true);
                b.WithCurrentTimestamp();
                b.AddField(lang.Warn_field_id, "`" + infraction.Id + "`", true);
                await ReplyAsync(embed: b.Build());
                col.Insert(inf);
            }
        }

        [Command("warnings", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireContext(ContextType.Guild)]
        public async Task Warnings()
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            Language lang = GetLanguage(Context.Guild.Id);
            using LiteDatabase db = new LiteDatabase(@"Filename=infraction.db; Connection=shared");
            ILiteCollection<ServerInfractions> col = db.GetCollection<ServerInfractions>();
            col.EnsureIndex(x => x.Server_id);
            ServerInfractions thign = col.FindOne(x => x.Server_id == Context.Guild.Id)!;
            if (thign != null)
            {
                List<string> pages = new List<string>();
                PaginatedMessage paginatedMessage = new PaginatedMessage
                {
                    Title = lang.All_server_warns,
                    Author = new EmbedAuthorBuilder
                    {
                        Name = GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username,
                        IconUrl = GetUserAvatarUrl(Context.User)
                    }
                };
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Infraction infraction in thign.Infractions)
                {
                    if (Range.Contains(stringBuilder.Length))
                    {
                        pages.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                    }
                    else
                    {
                        stringBuilder.Append(infraction.Id + ".  " + "<@!" + infraction.Punisher_id + ">" + lang.Inf_warned + "<@!" + infraction.User_id + ">" + lang.Inf_at + infraction.Time.ToString() + Environment.NewLine);
                    }
                }

                pages.Add(stringBuilder.ToString());
                paginatedMessage.Pages = pages;
                await PagedReplyAsync(paginatedMessage);
            }
            else
            {
                b.WithTitle(lang.No_inf_on_server);
            }
        }

        [Command("languages", RunMode = RunMode.Async)]
        public async Task Languages()
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            b.WithTitle(lang.Availible_languages);
            List<string> e = new List<string>();
            if (Directory.Exists(Environment.CurrentDirectory + @"\languages\"))
            {
                foreach (string u in Directory.GetFiles(Environment.CurrentDirectory + @"\languages\"))
                {
                    e.Add(Path.GetFileNameWithoutExtension(u));
                }
                b.WithDescription(string.Join("\n", e));
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("setlanguage", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Setlanguage(string langcode)
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            Console.WriteLine(Environment.CurrentDirectory + @"\languages\" + langcode + ".json");
            if (Directory.Exists(Environment.CurrentDirectory + @"\languages\"))
            {
                if (File.Exists(Environment.CurrentDirectory + @"\languages\" + langcode + ".json"))
                {
                    using LiteDatabase db = new LiteDatabase(@"Filename=serverlang.db; Connection=shared");
                    ILiteCollection<ServerLanguage> col = db.GetCollection<ServerLanguage>();
                    col.EnsureIndex(x => x.GuildID);
                    ServerLanguage thign = col.FindOne(x => x.GuildID == Context.Guild.Id)!;
                    if (thign != null)
                    {
                        thign.ISO_Code = langcode;
                        thign.GuildID = Context.Guild.Id;
                        col.Update(thign);
                    }
                    else
                    {
                        ServerLanguage e = new ServerLanguage
                        {
                            ISO_Code = langcode,
                            GuildID = Context.Guild.Id
                        };

                        col.Insert(e);
                    }
                }
                else
                {
                    b = new EmbedBuilder();
                    b.WithTitle(lang.Availible_languages);
                    List<string> e = new List<string>();
                    foreach (string u in Directory.GetFiles(Environment.CurrentDirectory + @"\languages\"))
                    {
                        e.Add(Path.GetFileNameWithoutExtension(u));
                    }
                    b.WithDescription(string.Join("\n", e));
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                }
            }
        }

        [Command("send", RunMode = RunMode.Async), Alias("sudo")]
        public async Task Send([Remainder] string CuNt)
        {
            bool isatsilvercraft = Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(Context.User.Id);
                isbotadmin = Is_bot_admin(socketGuildus);
            }
            if (isbotadmin)
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithDescription(CuNt);
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("add comand", RunMode = RunMode.Async), Alias("add command", "addcommand", "add splash", "addsplash", "support")]
        public async Task Addcomand()
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            b.WithTitle(lang.Support_Title);
            b.WithDescription(lang.Support_Description);
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("copyright", RunMode = RunMode.Async), Alias("©", "copy right", "credits", "maxisabitch", "hey cunt", "heycunt", ":copyright:", "©️")]
        public async Task Credits()
        {
            Language lang = GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle(lang.Bot_Name);
            b.WithDescription(lang.Credits);
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("invite", RunMode = RunMode.Async), Alias("botinvite", "bot")]
        public async Task Invite()
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            b.WithTitle(lang.Invite_title);
            b.WithDescription(lang.Invite_description);
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("hi", RunMode = RunMode.Async)]
        public async Task Ming()
        {
            Language lang = GetLanguage(Context.Guild.Id);
            await ReplyAsync(lang.Hi);
        }

        [Command("emote", RunMode = RunMode.Async)]
        public async Task Midsdsdsadng(string emoten)
        {
            Language lang = GetLanguage(Context.Guild.Id);
            List<GuildEmote> emotes = new List<GuildEmote>();
            using (LiteDatabase db = new LiteDatabase(@"Filename=optin.db; Connection=shared"))
            {
                ILiteCollection<Serveroptin> col = db.GetCollection<Serveroptin>();
                col.EnsureIndex(x => x.ServerId);
                foreach (SocketGuild a in Program.GetClient().Guilds)
                {
                    Serveroptin thing = col.FindOne(x => x.ServerId == a.Id);
                    if (thing != null && thing.optedin == true)
                    {
                        foreach (GuildEmote emotee in a.Emotes)
                        {
                            if (emoten == ":" + emotee.Name + ":")
                            {
                                emotes.Add(emotee);
                            }
                        }
                    }
                }
            }

            if (emotes.Count == 0)
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle(lang.No_emotes_found);
                b.WithDescription(lang.Searched_for + emoten + lang.Searched_for_back);
                b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
            else if (emotes.Count == 1)
            {
                await Context.Message.DeleteAsync();
                if (emotes[0].Animated)
                {
                    await ReplyAsync("<a:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                }
                else
                {
                    await ReplyAsync("<:" + emotes[0].Name + ":" + emotes[0].Id + ">");
                }
            }
            else if (emotes.Count > 1)
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle(lang.Multiple_emotes_found);
                StringBuilder builder = new StringBuilder();
                foreach (GuildEmote e in emotes)
                {
                    if (e.Animated)
                    {
                        builder.Append("<a:");
                        builder.Append(e.Name);
                        builder.Append(':');
                        builder.Append(e.Id);
                        builder.Append('>');
                        builder.AppendLine();
                    }
                    else
                    {
                        builder.Append("<:");
                        builder.Append(e.Name);
                        builder.Append(':');
                        builder.Append(e.Id);
                        builder.Append('>');
                        builder.AppendLine();
                    }
                }
                b.WithDescription(builder.ToString());
                b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("optintoemotes", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Optin()
        {
            Language lang = GetLanguage(Context.Guild.Id);
            using (LiteDatabase db = new LiteDatabase(@"Filename=optin.db; Connection=shared"))
            {
                ILiteCollection<Serveroptin> col = db.GetCollection<Serveroptin>();
                col.EnsureIndex(x => x.ServerId);
                Serveroptin thing = col.FindOne(x => x.ServerId == Context.Guild.Id);
                if (thing != null && thing.optedin == true)
                {
                    EmbedBuilder Bob = new EmbedBuilder();
                    Bob.WithTitle(lang.Already_opted_in);
                    Bob.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync("", false, Bob.Build());
                    return;
                }
                Serveroptin newserverthing = new Serveroptin
                {
                    ServerId = Context.Guild.Id,
                    optedin = true
                };
                col.Insert(newserverthing);
            }
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle(lang.Opted_in);
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("allemotes", RunMode = RunMode.Async)]
        public async Task Midsdsdewerfweergergsadng()
        {
            Language lang = GetLanguage(Context.Guild.Id);
            StringBuilder e = new StringBuilder();
            foreach (SocketGuild a in Program.GetClient().Guilds)
            {
                using LiteDatabase db = new LiteDatabase(@"Filename=optin.db; Connection=shared");
                ILiteCollection<Serveroptin> col = db.GetCollection<Serveroptin>();
                col.EnsureIndex(x => x.ServerId);
                Serveroptin thing = col.FindOne(x => x.ServerId == a.Id);
                if (thing != null && thing.optedin == true)
                {
                    e.Append(lang.Allemotes_Guild);
                    e.Append(a.Name);
                    e.AppendLine();
                    foreach (GuildEmote emotee in a.Emotes)
                    {
                        if (emotee.Animated)
                        {
                            e.Append("<a:");
                            e.Append(emotee.Name);
                            e.Append(':');
                            e.Append(emotee.Id);
                            e.Append('>');
                            e.AppendLine();
                        }
                        else
                        {
                            e.Append("<:");
                            e.Append(emotee.Name);
                            e.Append(':');
                            e.Append(emotee.Id);
                            e.Append('>');
                            e.AppendLine();
                        }
                    }
                }
            }

            List<string> pages = new List<string>();
            PaginatedMessage paginatedMessage = new PaginatedMessage
            {
                Title = lang.All_availible_emotes,
                Author = new EmbedAuthorBuilder
                {
                    Name = GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username,
                    IconUrl = GetUserAvatarUrl(Context.User)
                }
            };
            string[] splitemojis = e.ToString().Split('\n');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string line in splitemojis)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    pages.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.Append(line);
                }
            }
            pages.Add(stringBuilder.ToString());
            paginatedMessage.Pages = pages;
            await PagedReplyAsync(paginatedMessage);
        }

        public static string Bultstring(bool b)
        {
            if (b)
            {
                return ":white_check_mark:";
            }
            else
            {
                return ":x:";
            }
        }

        public static bool Is_at_silvercraft(SocketUser a)
        {
            bool is_at_silvercraft = false;
            SocketGuild e = Program.GetClient().GetGuild(dconfig.Server_id);
            if (e.Users.FirstOrDefault(b => b.Id == a.Id) != null)
            {
                is_at_silvercraft = true;
            }
            return is_at_silvercraft;
        }

        public static bool Is_bot_admin(SocketUser a)
        {
            bool isbotadmin = false;
            SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(a.Id);
            foreach (SocketRole role in socketGuildus.Roles)
            {
                if (role.Id == dconfig.Bot_admin_role_id)
                {
                    isbotadmin = true;
                }
            }
            return isbotadmin;
        }

        [Command("user", RunMode = RunMode.Async)]
        public async Task Usercmd(SocketUser a)
        {
            Language lang = GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            try
            {
                SocketUser user = Context.User;
                bool isatsilvercraft = Is_at_silvercraft(a);
                bool isbotadmin = false;
                if (isatsilvercraft)
                {
                    isbotadmin = Is_bot_admin(a);
                }
                b.WithTitle(lang.User + a.Username);
                b.ThumbnailUrl = GetUserAvatarUrl(a);
                b.WithDescription(lang.User + a.Mention + "");
                b.AddField(lang.Userid, a.Id, true);
                b.AddField(lang.Has_joined_support, Bultstring(isatsilvercraft), true);
                b.AddField(lang.Is_admin, Bultstring(isbotadmin), true);
                if (AudioModule.Isdj(a as SocketGuildUser) != 3)
                {
                    b.AddField(lang.Is_a_dj, Bultstring(AudioModule.Isdj(a as SocketGuildUser) == 1), true);
                }

                b.AddField(lang.Is_an_onwer, Bultstring(dconfig.Botowners.Contains(a.Id)), true);
                if (a.Id == 208691453973495808)
                {
                    b.AddField(lang.Is_a_bot, "see https://discord.com/channels/714154158969716780/714154159590473801/767829209052872724", true);
                }
                else
                {
                    b.AddField(lang.Is_a_bot, Bultstring(a.IsBot), true);
                }
                b.WithFooter(lang.Requested_by + user.Username, user.GetAvatarUrl());
                await ReplyAsync(embed: b.Build());
            }
            catch (Exception e)
            {
                SocketTextChannel _channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at command user");
                b.WithDescription("Error at command user:\n" + e);
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await _channel.SendMessageAsync("", false, b.Build());
                b.WithTitle(lang.Error);
                b.WithDescription(lang.Error_sent_to_logs);
                await ReplyAsync(embed: b.Build());

                throw;
            }
        }

        [Command("user", RunMode = RunMode.Async)]
        public async Task Usercmd()
        {
            await Usercmd(Context.User);
        }

        [Command("uselessfact", RunMode = RunMode.Async)]
        public async Task Kindsffeefergergrg()
        {
            Language lang = GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://uselessfacts.jsph.pl/random.md?language=" + lang.Lang_code_for_useless_facts);
            b.WithTitle(lang.Useless_fact);
            b.WithDescription(await rm.Content.ReadAsStringAsync());
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("bot", RunMode = RunMode.Async)]
        public async Task WhoIsBot(SocketUser user)
        {
            EmbedBuilder b = new EmbedBuilder();
            try
            {
                HttpClient client = Webclient.Get();
                HttpResponseMessage rm = await client.GetAsync("https://silverdimond.tk/silvercraftbot/bots/" + user.Id);
                b.WithAuthor(user);
                b.WithThumbnailUrl(user.GetAvatarUrl());
                if (rm.StatusCode == HttpStatusCode.OK)
                {
                    b.WithDescription(await rm.Content.ReadAsStringAsync());
                }
                else
                {
                    b.WithDescription(rm.StatusCode.ToString());
                }
                if (user.Id == 159985870458322944)
                {
                    b.WithThumbnailUrl("https://media.discordapp.net/attachments/728360861483401240/775156721206558741/scary_mee6.png");
                }
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
            catch (HttpRequestException e)
            {
                b = new EmbedBuilder();
                b.WithTitle("WebException");
                b.WithDescription(e.Message);
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("random gif", RunMode = RunMode.Async)]
        public async Task Kindsffeefergergrgfdfdsgfdfg()
        {
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            GiphyDotNet.Model.Results.GiphyRandomResult gifresult = await giphy.RandomGif(new RandomParameter
            {
                Rating = Rating.Pg
            });
            b.WithDescription(lang.Random_GIF + gifresult.Data.Url);
            b.WithAuthor(lang.Powered_by_GIPHY, "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif", "https://developers.giphy.com/");
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            b.WithImageUrl(gifresult.Data.ImageUrl);
            await ReplyAsync(embed: b.Build());
        }

        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        [Command("dm", RunMode = RunMode.Async)]
        public async Task Dm(SocketUser e, [Remainder] string thing)
        {
            bool isatsilvercraft = Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(Context.User.Id);
                isbotadmin = Is_bot_admin(socketGuildus);
            }
            if (isbotadmin)
            {
                try
                {
                    await e.SendMessageAsync(thing);
                }
                catch (Discord.Net.HttpException t)
                {
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithDescription("something has gone wrong code:" + t.HttpCode);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                }
            }
        }

        private static Config dconfig = new Config();

        public static void SetConfig(Config GetConfig)
        {
            dconfig = GetConfig;
        }

        [Command("help", RunMode = RunMode.Async)]
        public async Task King()
        {
            EmbedBuilder b = new EmbedBuilder();
            SocketUser user = Context.User;
            Language lang = GetLanguage(Context.Guild.Id);
            b.WithTitle(lang.SilverBot_Commands);
            b.WithDescription("https://silverbot.cf");
            b.WithFooter(lang.Requested_by + user.Username, user.GetAvatarUrl());
            await ReplyAsync(embed: b.Build());
        }

        public class Linkers
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("link")]
            public string Link { get; set; }
        }

        [Command("shorten", RunMode = RunMode.Async)]
        [Alias("s")]
        public async Task Shorten(string max, string bitch)
        {
            bool isatsilvercraft = Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(Context.User.Id);
                isbotadmin = Is_bot_admin(socketGuildus);
            }
            if (isbotadmin)
            {
                string url = "https://silvershort.herokuapp.com/D4XNACYf7BRf6AzqH5v6k8sRuWS35wfHU5YdKw4vh2ugKWWaKx6JcBE92Du8G8D4LhAxJ";
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                Linkers datacs = new Linkers()
                {
                    Link = bitch,
                    Name = max
                };
                string data = System.Text.Json.JsonSerializer.Serialize(datacs);
                using (StreamWriter streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                try
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    string result;
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        EmbedBuilder b = new EmbedBuilder();
                        SocketUser user = Context.User;
                        b.WithTitle("EPIC silvershort accepted our request");
                        b.WithDescription(@"https://short.silverdimond.tk/" + max);
                        b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + user.Username, user.GetAvatarUrl());
                        await ReplyAsync(embed: b.Build());
                    }
                    if (httpResponse.StatusCode == HttpStatusCode.Conflict)
                    {
                        EmbedBuilder b = new EmbedBuilder();
                        SocketUser user = Context.User;
                        b.WithTitle("fuck a conflict");
                        b.WithDescription(result);
                        b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + user.Username, user.GetAvatarUrl());
                        await ReplyAsync(embed: b.Build());
                    }
                }
                catch (WebException ex)

                {
                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Conflict)
                    {
                        EmbedBuilder b = new EmbedBuilder();
                        SocketUser user = Context.User;
                        b.WithTitle("fuck a conflict in a fucking exeption");
                        b.WithDescription(new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()).ReadToEnd());
                        b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + user.Username, user.GetAvatarUrl());
                        await ReplyAsync(embed: b.Build());
                    }
                }
            }
        }

        [Command("shorten", RunMode = RunMode.Async)]
        [Alias("s")]
        public async Task Shorten(string bitch)
        {
            bool isatsilvercraft = Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(Context.User.Id);
                isbotadmin = Is_bot_admin(socketGuildus);
            }
            if (isbotadmin)
            {
                string oi = RandomString(7);
                try
                {
                    oi = Regex.Replace(oi, @"[^\w]", "",
                                          RegexOptions.None, TimeSpan.FromSeconds(1.5));
                }
                // If we timeout when replacing invalid characters,
                // we should return Empty.
                catch (RegexMatchTimeoutException)
                {
                    oi = string.Empty;
                }
                await Shorten(oi, bitch);
            }
        }

        [Command("ban", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser user, [Remainder] string why)
        {
            Language lang = GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            SocketGuildUser thebanner = Context.User as SocketGuildUser;
            if (thebanner.GuildPermissions.BanMembers)
            {
                if (!(thebanner.Hierarchy > user.Hierarchy))
                {
                    b = new EmbedBuilder();
                    b.WithTitle(lang.User_has_lower_role + lang.Ban);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                    return;
                }
                if (!(Context.Guild.CurrentUser.Hierarchy > user.Hierarchy))
                {
                    b = new EmbedBuilder();
                    b.WithTitle(lang.Bot_has_lower_role + lang.Ban);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                    return;
                }
                try
                {
                    b.WithTitle(lang.Ban_dm_front + Context.Guild.Name);
                    b.AddField(lang.Warn_field_reason, why);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await user.SendMessageAsync("", false, b.Build());
                }
                catch (Discord.Net.HttpException wex)
                {
                    if (wex.DiscordCode == 50007)
                    {
                        b.WithTitle(lang.Unable_to_send_dm);
                        b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                        await ReplyAsync(embed: b.Build());
                    }
                }
                await user.BanAsync(0, why);

                b = new EmbedBuilder();
                Random random = new Random();
                b.WithTitle(user.Username + " " + lang.Banstrings[random.Next(0, lang.Banstrings.Length)] + " " + thebanner.Username);
                b.AddField(lang.Warn_field_reason, why);
                b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
            else
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.User_Doesnt_Have_BanMembers);
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("ban", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser user)
        {
            await Ban(user, "The ban hammer has spoken");
        }

        [Command("kick", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user, [Remainder] string why)
        {
            Language lang = GetLanguage(Context.Guild.Id);
            SocketGuildUser thebanner = Context.User as SocketGuildUser;
            EmbedBuilder b = new EmbedBuilder();
            if (thebanner.GuildPermissions.KickMembers)
            {
                if (!(thebanner.Hierarchy > user.Hierarchy))
                {
                    b = new EmbedBuilder();
                    b.WithTitle(lang.User_has_lower_role + lang.Kick);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                    return;
                }
                if (!(Context.Guild.CurrentUser.Hierarchy > user.Hierarchy))
                {
                    b = new EmbedBuilder();
                    b.WithTitle(lang.Bot_has_lower_role + lang.Kick);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                    return;
                }
                try
                {
                    b.WithTitle(lang.Kick_dm_front + Context.Guild.Name);
                    b.AddField(lang.Warn_field_reason, why);
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    await user.SendMessageAsync("", false, b.Build());
                }
                catch (Discord.Net.HttpException wex)
                {
                    if (wex.DiscordCode == 50007)
                    {
                        b = new EmbedBuilder();
                        b.WithTitle(lang.Unable_to_send_dm);
                        b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                        await ReplyAsync(embed: b.Build());
                    }
                }
                await user.KickAsync(why);
                b = new EmbedBuilder();
                Random random = new Random();
                b.WithTitle(user.Username + " " + lang.Banstrings[random.Next(0, lang.Banstrings.Length)] + " " + thebanner.Username);
                b.AddField(lang.Warn_field_reason, why);
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
            else
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.User_Doesnt_Have_KickMembers);
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("kick", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user)
        {
            await Kick(user, "The kick sniper rifle has spoken");
        }

        [Command("guilds", RunMode = RunMode.Async)]
        public async Task See()
        {
            bool isatsilvercraft = Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                SocketGuildUser socketGuildus = Program.GetClient().GetGuild(dconfig.Server_id).GetUser(Context.User.Id);
                isbotadmin = Is_bot_admin(socketGuildus);
            }
            if (isbotadmin)
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle(GetLanguage(Context.Guild.Id).Bot_guilds);

                StringBuilder e = new StringBuilder();

                foreach (SocketGuild a in Program.GetClient().Guilds)
                {
                    e.Append(a.Name);
                    e.Append(" | ");
                    e.Append(a.Owner);
                    e.Append(" | ");
                    e.Append(a.OwnerId);
                    e.Append(" | ");
                    e.AppendLine();
                }
                b.WithDescription(e.ToString());
                b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("unban", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task UnbanTask(ulong userId)
        {
            await Context.Guild.RemoveBanAsync(userId);
        }

        [Command("purge", RunMode = RunMode.Async)]
        [Alias("clean", "clear")]
        [Summary("Downloads and removes X messages from the current channel.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(int amount)
        {
            if (amount <= 0)
            {
                await ReplyAsync(GetLanguage(Context.Guild.Id).Purge_number_negative);
                return;
            }
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount).FlattenAsync();
            IEnumerable<IMessage> filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
            int count = filteredMessages.Count();
            if (count == 0)
            {
                await ReplyAsync(GetLanguage(Context.Guild.Id).Purge_nothing_to_delete);
            }
            else
            {
                await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
                Language lang = GetLanguage(Context.Guild.Id);
                await ReplyAsync(lang.Purge_removed_front + count + (count > 1 ? lang.Purge_removed_plural : lang.Purge_removed_single));
            }
        }

        [Command("botstatus", RunMode = RunMode.Async)]
        public async Task UnbanTasetfk()
        {
            EmbedBuilder b = new EmbedBuilder();
            Process proc = Process.GetCurrentProcess();
            b.WithTitle("Bot status");
            b.AddField("Shard", Context.Client.ShardId, true);
            b.AddField("Latency", Context.Client.Latency, true);
            b.AddField("CPU Architecture", System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"), true);
            b.AddField("Amount of allocated ram", (proc.PrivateMemorySize64 / 1000000) + "MB", true);
            b.AddField("Amount of Guilds", Context.Client.Guilds.Count, true);
            b.AddField("Amount of GroupChannels", Context.Client.GroupChannels.Count, true);
            b.AddField("Amount of DM Channels", Context.Client.DMChannels.Count, true);
            b.AddField("Version #", SIlverCraftBot.Version.vnumber, true);
            b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("gif", RunMode = RunMode.Async)]
        public async Task Kindsffeefergergrgfdfdsgfdgfdsfgdfgfdfdghdfg([Remainder] string term)
        {
            GiphyDotNet.Model.Results.GiphySearchResult gifResult;
            int page = 0;
            EmbedBuilder b = new EmbedBuilder();
            Language lang = GetLanguage(Context.Guild.Id);
            SearchParameter searchParameter = new SearchParameter
            {
                Query = term,
                Rating = Rating.Pg,
            };
            gifResult = await giphy.GifSearch(searchParameter);
            b.WithDescription(string.Format(lang.Search_Results_for_term, term) + " : " + gifResult.Data[0].Url + "\n" + string.Format(lang.Page_Gif, 1, 25));
            b.WithAuthor(lang.Powered_by_GIPHY, "https://cdn.discordapp.com/attachments/728360861483401240/747894851814817863/Poweredby_640px_Badge.gif", "https://developers.giphy.com/");
            b.WithFooter(lang.Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
            b.WithImageUrl(gifResult.Data[0].Images.Original.Url);
            IUserMessage AmIthisDumb = await ReplyAsync(embed: b.Build());

        Wait:
            SocketMessage response = await NextMessageAsync(timeout: fivemin);
            if (response != null)
            {
                if (response.Content == "next")
                {
                    page++;
                    if (page > 24)
                    {
                        page = 0;
                    }

                    b.WithDescription(string.Format(lang.Search_Results_for_term, term) + " : " + gifResult.Data[page].Url + "\n" + string.Format(lang.Page_Gif, page + 1, 25));
                    b.WithAuthor(lang.Powered_by_GIPHY, "https://giphy.com/static/img/giphy_logo_square_social.png", "https://developers.giphy.com/");
                    b.WithFooter(GetLanguage(Context.Guild.Id).Requested_by + Context.User.Username, GetUserAvatarUrl(Context.User));
                    b.WithImageUrl(gifResult.Data[page].Images.Original.Url);
                    await AmIthisDumb.ModifyAsync(x =>
                    {
                        x.Embed = b.Build();
                    });
                    goto Wait;
                }
                else
                {
                    goto Wait;
                }
            }
            else
            {
                await AmIthisDumb.ModifyAsync(x =>
                {
                    x.Content = lang.Period_Expired;
                });
            }
        }

        private readonly ulong[] badpeople = { 764975814814466058 };

        private bool Isbannedfromsilvercraftbotsocial(ulong id)
        {
            return badpeople.Contains(id);
        }

        [Command("bancheck", RunMode = RunMode.Async)]
        public async Task Bancheck(/*[Remainder]string text*/)
        {
            Language lang = GetLanguage(Context.Guild.Id);
            SocketUser user = Context.User;
            EmbedBuilder b = new EmbedBuilder();
            if (Isbannedfromsilvercraftbotsocial(user.Id))
            {
                b.WithDescription(lang.User_is_banned_from_silversocial);
            }
            else
            {
                b.WithDescription(lang.User_is_not_banned_from_silversocial);
            }

            b.WithFooter(lang.Requested_by + user.Username, user.GetAvatarUrl());
            await ReplyAsync(embed: b.Build());
        }

        private static readonly TimeSpan fivemin = new TimeSpan(0, 5, 10);

        [Command("stealemoji", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageEmojis)]
        [RequireUserPermission(GuildPermission.ManageEmojis)]
        public async Task stealemoji(string name, string url)
        {
            Language lang = GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            if (Context.Guild.Emotes.FirstOrDefault(x => x.Name == name) == null)
            {
                HttpClient client = Webclient.Get();
                HttpResponseMessage rm = await client.GetAsync(url);
                GuildEmote e = await Context.Guild.CreateEmoteAsync(name, new Image(await rm.Content.ReadAsStreamAsync()));
                b.WithTitle("success epic gamers");
                b.WithDescription(e.ToString());
                b.WithFooter(lang.Requested_by + Context.User.Username, Context.User.GetAvatarUrl());
                await ReplyAsync(embed: b.Build());
            }
            else
            {
                b.WithDescription("huston we has problem, someone stealed name before");
                b.WithFooter(lang.Requested_by + Context.User.Username, Context.User.GetAvatarUrl());
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("time", RunMode = RunMode.Async)]
        public async Task TimeBing()
        {
            Language lang = GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            b.WithDescription(lang.Time_in_utc + DateTime.UtcNow.ToString(lang.Time_format));
            b.WithFooter(lang.Requested_by + Context.User.Username, Context.User.GetAvatarUrl());
            await ReplyAsync(embed: b.Build());
        }
    }
}