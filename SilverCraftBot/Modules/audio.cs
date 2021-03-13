using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using dotnetcorebot.Modules.infoclasses;
using SilverCraftBot.Modules;
using SIlverCraftBot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace dotnetcorebot.Modules
{
    public class AudioModule : InteractiveBase
    {
        private static LavaNode _lavaNode;
        private static readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();

        public static int Isdj(SocketGuildUser a)
        {
            ushort isbotadmin = 3;
            foreach (SocketRole role in a.Guild.Roles)
            {
                if (role.Name.ToLower().Contains("dj"))
                {
                    isbotadmin = 0;
                }
            }

            foreach (SocketRole role in a.Roles)
            {
                if (role.Name.ToLower().Contains("dj"))
                {
                    isbotadmin = 1;
                }
            }
            return isbotadmin;
        }

        public static void Musicmodule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
            _lavaNode.OnTrackEnded += OnTrackEnded;
            _lavaNode.OnTrackStarted += OnTrackStarted;
        }

        [Command("Join")]
        public async Task JoinAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync(lang.Already_connected);
                return;
            }

            IVoiceState voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync(lang.User_not_connected);
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync(lang.Joined_before + voiceState.VoiceChannel.Name + lang.Joined_after);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
                throw;
            }
        }

        private static Config dconfig = new Config();

        public static void SetConfig(Config GetConfig)
        {
            dconfig = GetConfig;
        }

        private async Task Exeptionhandle(Exception exception, string command)
        {
            EmbedBuilder b = new EmbedBuilder();
            SocketTextChannel channel = Program.GetClient().GetChannel(dconfig.Log_channel) as SocketTextChannel;
            b.WithTitle("Error at executing " + command);
            b.WithDescription("More detailed explination below\nError at command " + command + ":\n" + exception);
            b.AddField("Exception Message", exception.Message);
            b.AddField("Exeption StackTrace", exception.StackTrace);
            b.AddField("Message content", Context.Message.Content);
            b.AddField("Message url", Context.Message.GetJumpUrl());
            b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
            b.AddField("Command", command);
            b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
            await channel.SendMessageAsync("", false, b.Build());
            b = new EmbedBuilder();
            b.WithTitle("Error");
            b.WithDescription("Complete error sent to Bot logs");
            await ReplyAsync(embed: b.Build());
            throw exception;
        }

        [Command("Queue", RunMode = RunMode.Async)]
        public async Task Queuee()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle(lang.Not_connected);
                b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
                return;
            }

            if (player.PlayerState != PlayerState.Playing && player.PlayerState != PlayerState.Paused)
            {
                EmbedBuilder b = new EmbedBuilder();
                //hey python shitass wanna see me code
                b.WithTitle(lang.Not_playing);
                b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
                return;
            }
            int a = 0;
            StringBuilder Bob = new StringBuilder();
            Bob.Append(a);
            Bob.Append(". ");
            Bob.Append(player.Track.Title);
            Bob.Append(lang.Song_by_author);
            Bob.Append(player.Track.Author);
            if (islooped.ContainsKey(Context.Guild.Id))
            {
                if (islooped[Context.Guild.Id] == 1)
                {
                    Bob.Append(":repeat_one:");
                }
                if (islooped[Context.Guild.Id] == 2)
                {
                    Bob.Append(":repeat:");
                }
            }
            Bob.Append(Environment.NewLine);
            a++;
            foreach (LavaTrack track in player.Queue)
            {
                Bob.Append(a);
                Bob.Append(". ");
                Bob.Append(track.Title);
                Bob.Append(lang.Song_by_author);
                Bob.Append(track.Author);
                Bob.Append(Environment.NewLine);
                a++;
            }
            List<string> pages = new List<string>();
            PaginatedMessage paginatedMessage = new PaginatedMessage
            {
                Title = $"Queue:"
            };
            string[] splitqueue = Bob.ToString().Split('\n');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string line in splitqueue)
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

        [Command("remove", RunMode = RunMode.Async)]
        public async Task Rqueuee(int PPSIZE)
        {
            PPSIZE--;
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Not_connected);
                b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
                return;
            }

            if (player.PlayerState != PlayerState.Playing && player.PlayerState != PlayerState.Paused)
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Not_playing);
                b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
                return;
            }
            if (player.Queue.Count < PPSIZE + 1)
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Track_not_exist);
                b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync(embed: b.Build());
                return;
            }
            b.WithTitle(lang.Removed_front + player.Queue.ElementAt(PPSIZE).Title + lang.Song_by_author + player.Queue.ElementAt(PPSIZE).Author);
            player.Queue.RemoveAt(PPSIZE);
            b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("Pause", RunMode = RunMode.Async)]
        public async Task PauseAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            try
            {
                await player.PauseAsync();
                await ReplyAsync(lang.Paused_front + player.Track.Title);
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "pause").ConfigureAwait(false);
                throw;
            }
        }

        [Command("Stop", RunMode = RunMode.Async)]
        public async Task StopAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync(lang.Already_stoped);
                return;
            }

            try
            {
                switch (Isdj((SocketGuildUser)Context.User))
                {
                    case 0:
                        {
                            EmbedBuilder b = new EmbedBuilder();
                            b.WithTitle(lang.User_doesnt_have_dj);
                            await ReplyAsync(embed: b.Build());
                            break;
                        }
                    case 1:
                        {
                            try
                            {
                                EmbedBuilder b = new EmbedBuilder();
                                await player.StopAsync();
                                b.WithTitle(lang.No_longer_playing);
                                await ReplyAsync(embed: b.Build());
                            }
                            catch (Exception exception)
                            {
                                await Exeptionhandle(exception, "stop").ConfigureAwait(false);
                                throw;
                            }
                            break;
                        }
                    case 3:
                        {
                            try
                            {
                                EmbedBuilder b = new EmbedBuilder();
                                await player.StopAsync();
                                b.WithTitle(lang.No_longer_playing);
                                b.WithDescription(lang.Use_dj_message);
                                await ReplyAsync(embed: b.Build());
                            }
                            catch (Exception exception)
                            {
                                await Exeptionhandle(exception, "stop").ConfigureAwait(false);
                                throw;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "stop").ConfigureAwait(false);
                throw;
            }
        }

        public static readonly HashSet<ulong> VoteQueue = new HashSet<ulong>();
        private static readonly ConcurrentDictionary<ulong, HashSet<ulong>> thing1 = new ConcurrentDictionary<ulong, HashSet<ulong>>();

        /// <summary>
        /// 0 if not looping
        /// 1 if song looping
        /// 2 if queue looping
        /// </summary>
        private static readonly ConcurrentDictionary<ulong, ushort> islooped = new ConcurrentDictionary<ulong, ushort>();

        [Command("Skip", RunMode = RunMode.Async)]
        public async Task SkipAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }
            if (!thing1.ContainsKey(player.VoiceChannel.GuildId))
            {
                thing1.TryAdd(player.VoiceChannel.GuildId, new HashSet<ulong>());
            }
            SocketGuildUser[] voiceChannelUsers = (player.VoiceChannel as SocketVoiceChannel).Users.Where(x => !x.IsBot).ToArray();
            if (thing1[player.VoiceChannel.GuildId].Contains(Context.User.Id))
            {
                await ReplyAsync(lang.Already_voted);
                return;
            }

            thing1[player.VoiceChannel.GuildId].Add(Context.User.Id);
            int percentage = thing1[player.VoiceChannel.GuildId].Count / voiceChannelUsers.Length * 100;
            if (percentage < 85)
            {
                await ReplyAsync(lang.Voting_is_below_85_percent);
                return;
            }

            try
            {
                LavaTrack oldTrack = player.Track;
                LavaTrack currenTrack = await player.SkipAsync();
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle($"Skipped: {oldTrack.Title}\nNow Playing: {currenTrack.Title}");
                string david = await currenTrack.FetchArtworkAsync();
                b.WithThumbnailUrl(david);
                b.WithFooter("Requested by CONSOLE using votes", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await ReplyAsync(embed: b.Build());
                thing1.TryRemove(player.VoiceChannel.GuildId, out HashSet<ulong> bitch);
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "skip").ConfigureAwait(false);
                throw;
            }
        }

        [Command("ForceSkip", RunMode = RunMode.Async), Alias("fs")]
        public async Task ForceSkipAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            SocketGuildUser user = Context.User as SocketGuildUser;
            bool isatsilvercraft = Commands.Is_at_silvercraft(Context.User);
            bool isbotadmin = false;
            if (isatsilvercraft)
            {
                isbotadmin = Commands.Is_bot_admin(user);
            }
            if (isbotadmin || Isdj((SocketGuildUser)Context.User) == 1)
            {
                try
                {
                    LavaTrack oldTrack = player.Track;
                    LavaTrack currenTrack = await player.SkipAsync();
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithTitle($"Skipped: {oldTrack.Title}\nNow Playing: {currenTrack.Title}");
                    string david = await currenTrack.FetchArtworkAsync();
                    b.WithThumbnailUrl(david);
                    b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                }
                catch (Exception exception)
                {
                    await Exeptionhandle(exception, "forceskip").ConfigureAwait(false);
                    throw;
                }
            }
        }

        [Command("Volume", RunMode = RunMode.Async)]
        public async Task VolumeAsync(ushort volume)
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            try
            {
                await player.UpdateVolumeAsync(volume);
                await ReplyAsync($"I've changed the player volume to {volume}.");
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "volume").ConfigureAwait(false);
                throw;
            }
        }

        [Command("NowPlaying", RunMode = RunMode.Async), Alias("Np")]
        public async Task NowPlayingAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            LavaTrack track = player.Track;
            string artwork = await track.FetchArtworkAsync();

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = $"{track.Author} - {track.Title}",
                ThumbnailUrl = artwork,
                Url = track.Url
            }
                .AddField("Id", track.Id, true)
                .AddField("Duration", track.Duration, true)
                .AddField("Position", track.Position, true);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Loop", RunMode = RunMode.Async), Alias("loopsong")]
        public async Task LoopAsync(string sorq)
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            EmbedBuilder b = new EmbedBuilder();
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync(lang.Not_connected);
            }
            else
            {
                LavaPlayer player = _lavaNode.GetPlayer(Context.Guild);
                if (player.PlayerState != PlayerState.Playing && player.PlayerState != PlayerState.Paused)
                {
                    await ReplyAsync(lang.Not_playing);
                }
                else
                {
                    switch (Isdj((SocketGuildUser)Context.User))
                    {
                        case 0:
                            {
                                b.WithTitle(lang.User_doesnt_have_dj);
                                await ReplyAsync(embed: b.Build());
                                return;
                            }
                        case 1:
                            {
                                break;
                            }
                        case 3:
                            {
                                b.WithDescription(lang.Use_dj_message);

                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
                if (islooped.ContainsKey(Context.Guild.Id))
                {
                    if (sorq == "song")
                    {
                        islooped[Context.Guild.Id] = 1;
                        b.WithTitle(lang.Looping_song);
                    }
                    else if (sorq == "queue")
                    {
                        islooped[Context.Guild.Id] = 2;
                        b.WithTitle(lang.Looping_queue);
                    }
                    else if (sorq == "none")
                    {
                        islooped[Context.Guild.Id] = 0;
                        b.WithTitle(lang.Not_looping);
                    }
                }
                else
                {
                    if (sorq == "song")
                    {
                        islooped.TryAdd(Context.Guild.Id, 1);
                        b.WithTitle(lang.Looping_song);
                    }
                    else if (sorq == "queue")
                    {
                        islooped.TryAdd(Context.Guild.Id, 2);
                        b.WithTitle(lang.Looping_queue);
                    }
                    else if (sorq == "none")
                    {
                        islooped.TryAdd(Context.Guild.Id, 0);
                        b.WithTitle(lang.Not_looping);
                    }
                }
                await ReplyAsync(embed: b.Build());
            }
        }

        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        [Command("Genius", RunMode = RunMode.Async)]
        public async Task ShowGeniusLyrics()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            string lyrics = await player.Track.FetchLyricsFromGeniusAsync();
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync(lang.No_lyrics_found + player.Track.Title);
                return;
            }

            string[] splitLyrics = lyrics.Split('\n');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }

        [Command("OVH", RunMode = RunMode.Async)]
        public async Task ShowOVHLyrics()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            string lyrics = await player.Track.FetchLyricsFromOVHAsync();
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync(lang.No_lyrics_found + player.Track.Title);
                return;
            }

            string[] splitLyrics = lyrics.Split('\n');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }

        [Command("Seek", RunMode = RunMode.Async)]
        public async Task SeekAsync(string ts)
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }
            TimeSpan timeSpan = TimeSpan.Parse(ts);
            try
            {
                await player.SeekAsync(timeSpan);
                await ReplyAsync(lang.Seeked_front + player.Track.Title + lang.Seeked_middle + timeSpan + lang.Seeked_back);
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "seek").ConfigureAwait(false);
                throw;
            }
        }

        [Command("Resume", RunMode = RunMode.Async)]
        public async Task ResumeAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Paused)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            try
            {
                await player.ResumeAsync();
                await ReplyAsync(lang.Resumed_front + player.Track.Title);
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "resume").ConfigureAwait(false);
                throw;
            }
        }

        [Command("Play", RunMode = RunMode.Async), Alias("p")]
        public async Task PlayAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            if (player.PlayerState != PlayerState.Paused)
            {
                await ReplyAsync(lang.Not_playing);
                return;
            }

            try
            {
                await player.ResumeAsync();
                await ReplyAsync(lang.Resumed_front + player.Track.Title);
            }
            catch (Exception exception)
            {
                await Exeptionhandle(exception, "resume in play").ConfigureAwait(false);
                throw;
            }
        }

        [Command("playsoundcloud", RunMode = RunMode.Async), Alias("scp")]
        public async Task SoundCloudSearchAsync([Remainder] string searchQuery)
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                IVoiceState voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel == null)
                {
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithTitle(lang.Not_connected);
                    b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                    return;
                }

                try
                {
                    await JoinAsync();
                }
                catch (Exception exception)
                {
                    await Exeptionhandle(exception, "join in play").ConfigureAwait(false);
                    throw;
                }
            }
            LavaPlayer player = _lavaNode.GetPlayer(Context.Guild);
            if (player.PlayerState == PlayerState.Paused && string.IsNullOrEmpty(searchQuery))
            {
                try
                {
                    await player.ResumeAsync();
                    await ReplyAsync(lang.Resumed_front + player.Track.Title);
                    return;
                }
                catch (Exception exception)
                {
                    await Exeptionhandle(exception, "resume in play").ConfigureAwait(false);
                    throw;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    await ReplyAsync(lang.No_search_term);
                    return;
                }
            }

            Victoria.Responses.Rest.SearchResponse searchResponse = await _lavaNode.SearchSoundCloudAsync(searchQuery);

            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle(lang.No_results_front + searchQuery + lang.No_results_back);
                eb.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                await ReplyAsync($"", false, eb.Build());
                return;
            }

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    foreach (LavaTrack track in searchResponse.Tracks)
                    {
                        player.Queue.Enqueue(track);
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    LavaTrack track = searchResponse.Tracks[0];
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
            }
            else
            {
                LavaTrack track = searchResponse.Tracks[0];

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    for (int i = 0; i < searchResponse.Tracks.Count; i++)
                    {
                        if (i == 0)
                        {
                            await player.PlayAsync(track);
                            EmbedBuilder b = new EmbedBuilder();
                            b.WithTitle($"Now playing: {track.Title}");

                            b.AddField("source", "<:soundcloud:793405493204090911> search");

                            string art = await track.FetchArtworkAsync();
                            b.WithThumbnailUrl(art);
                            b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                            await ReplyAsync(embed: b.Build());
                        }
                        else
                        {
                            if (searchResponse.Tracks[i] == null)
                            {
                                await ReplyAsync(i + " was null");
                            }
                            player.Queue.Enqueue(searchResponse.Tracks[i]);
                        }
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    await player.PlayAsync(track);
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithTitle($"Now playing: {track.Title}");

                    b.AddField("source", "<:soundcloud:793405493204090911> search");

                    string art = await track.FetchArtworkAsync();
                    b.WithThumbnailUrl(art);
                    b.WithFooter(new Language().Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                }
            }
        }

        [Command("Play", RunMode = RunMode.Async), Alias("p")]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                IVoiceState voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel == null)
                {
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithTitle(lang.Not_connected);
                    b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                    return;
                }

                try
                {
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithTitle(lang.Joined_before + voiceState.VoiceChannel.Name + lang.Joined_after);
                    b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                }
                catch (Exception exception)
                {
                    await Exeptionhandle(exception, "join in play").ConfigureAwait(false);
                    throw;
                }
            }
            LavaPlayer player = _lavaNode.GetPlayer(Context.Guild);
            if (player.PlayerState == PlayerState.Paused && string.IsNullOrEmpty(searchQuery))
            {
                try
                {
                    await player.ResumeAsync();
                    await ReplyAsync(lang.Resumed_front + player.Track.Title);
                    return;
                }
                catch (Exception exception)
                {
                    await Exeptionhandle(exception, "resume in play").ConfigureAwait(false);
                    throw;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    await ReplyAsync(lang.No_search_term);
                    return;
                }
            }
            int searchtype = 0;

            Victoria.Responses.Rest.SearchResponse searchResponse = await _lavaNode.SearchAsync(searchQuery);

            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                searchResponse = await _lavaNode.SearchYouTubeAsync(searchQuery); searchtype = 1;
                if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
                {
                    searchResponse = await _lavaNode.SearchSoundCloudAsync(searchQuery); searchtype = 2;
                    if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
                    {
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.WithTitle(lang.No_results_front + searchQuery + lang.No_results_back);
                        eb.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                        await ReplyAsync($"", false, eb.Build());
                        return;
                    }
                }
            }

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    foreach (LavaTrack track in searchResponse.Tracks)
                    {
                        player.Queue.Enqueue(track);
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    LavaTrack track = searchResponse.Tracks[0];
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
            }
            else
            {
                LavaTrack track = searchResponse.Tracks[0];

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    for (int i = 0; i < searchResponse.Tracks.Count; i++)
                    {
                        if (i == 0)
                        {
                            await player.PlayAsync(track);
                            EmbedBuilder b = new EmbedBuilder();
                            b.WithTitle($"Now playing: {track.Title}");
                            if (searchtype == 1)
                            {
                                b.AddField("source", "<:youtube:793403957871247360> search");
                            }
                            if (searchtype == 2)
                            {
                                b.AddField("source", "<:soundcloud:793405493204090911> search");
                            }
                            string art = await track.FetchArtworkAsync();
                            b.WithThumbnailUrl(art);
                            b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                            await ReplyAsync(embed: b.Build());
                        }
                        else
                        {
                            if (searchResponse.Tracks[i] == null)
                            {
                                await ReplyAsync(i + " was null");
                            }
                            player.Queue.Enqueue(searchResponse.Tracks[i]);
                        }
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    await player.PlayAsync(track);
                    EmbedBuilder b = new EmbedBuilder();
                    b.WithTitle($"Now playing: {track.Title}");
                    if (searchtype == 1)
                    {
                        b.AddField("source", "<:youtube:793403957871247360> search");
                    }
                    if (searchtype == 2)
                    {
                        b.AddField("source", "<:soundcloud:793405493204090911> search");
                    }
                    string art = await track.FetchArtworkAsync();
                    b.WithThumbnailUrl(art);
                    b.WithFooter(new Language().Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    await ReplyAsync(embed: b.Build());
                }
            }
        }

        private static async Task OnTrackStarted(TrackStartEventArgs arg)
        {
            if (!_disconnectTokens.TryGetValue(arg.Player.VoiceChannel.Id, out CancellationTokenSource value))
            {
                return;
            }

            if (value.IsCancellationRequested)
            {
                return;
            }

            value.Cancel(true);
            await arg.Player.TextChannel.SendMessageAsync("Auto disconnect has been cancelled!");
        }

        private static async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan)
        {
            Language lang = Commands.GetLanguage(player.VoiceChannel.GuildId);
            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out CancellationTokenSource value))
            {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }
            else if (value.IsCancellationRequested)
            {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            await player.TextChannel.SendMessageAsync($"Auto disconnect initiated! Disconnecting in {timeSpan}...");
            bool isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
            if (isCancelled)
            {
                return;
            }

            await _lavaNode.LeaveAsync(player.VoiceChannel);

            await player.TextChannel.SendMessageAsync(lang.Goodbye);
        }

        [Command("Leave", RunMode = RunMode.Async), Alias("disconect", "dc")]
        public async Task LeaveAsync()
        {
            Language lang = Commands.GetLanguage(Context.Guild.Id);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
            {
                await ReplyAsync(lang.Not_connected);
                return;
            }

            IVoiceChannel voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null)
            {
                await ReplyAsync(lang.Not_sure_which_channel);
                return;
            }
            switch (Isdj((SocketGuildUser)Context.User))
            {
                case 0:
                    {
                        EmbedBuilder b = new EmbedBuilder();
                        b.WithTitle(lang.User_doesnt_have_dj);
                        await ReplyAsync(embed: b.Build());
                        break;
                    }
                case 1:
                    {
                        try
                        {
                            EmbedBuilder b = new EmbedBuilder();
                            await _lavaNode.LeaveAsync(voiceChannel);
                            b.WithTitle(lang.Left_front + voiceChannel.Name + lang.Left_back);
                            await ReplyAsync(embed: b.Build());
                        }
                        catch (Exception exception)
                        {
                            await Exeptionhandle(exception, "leave").ConfigureAwait(false);
                            throw;
                        }
                        break;
                    }
                case 3:
                    {
                        try
                        {
                            EmbedBuilder b = new EmbedBuilder();
                            b.WithTitle(lang.Left_front + voiceChannel.Name + lang.Left_back);
                            b.WithDescription(lang.Use_dj_message);
                            await ReplyAsync(embed: b.Build());
                            await _lavaNode.LeaveAsync(voiceChannel);
                        }
                        catch (Exception exception)
                        {
                            await Exeptionhandle(exception, "leave").ConfigureAwait(false);
                            throw;
                        }
                        break;
                    }
                default:
                    {
                        try
                        {
                            EmbedBuilder b = new EmbedBuilder();
                            b.WithTitle(lang.Left_front + voiceChannel.Name + lang.Left_back);
                            b.WithDescription(lang.Use_dj_message + " (warning: switch case returned default)");
                            await ReplyAsync(embed: b.Build());
                            await _lavaNode.LeaveAsync(voiceChannel);
                        }
                        catch (Exception exception)
                        {
                            await Exeptionhandle(exception, "leave").ConfigureAwait(false);
                            throw;
                        }
                        break;
                    }
            }
        }

        private static async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            if (!args.Reason.ShouldPlayNext())
            {
                return;
            }
            Language lang = Commands.GetLanguage(args.Player.VoiceChannel.GuildId);
            LavaPlayer player = args.Player;
            EmbedBuilder b = new EmbedBuilder();
            SocketGuildUser[] voiceChannelUsers = (player.VoiceChannel as SocketVoiceChannel).Users.Where(x => !x.IsBot).ToArray();
            if (voiceChannelUsers.Length == 0)
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Noone_in_vc);
                await args.Player.TextChannel.SendMessageAsync("", false, b.Build());

                _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromSeconds(10));
                return;
            }
            if (islooped.ContainsKey(args.Player.VoiceChannel.GuildId) && islooped[args.Player.VoiceChannel.GuildId] == 1)
            {
                await args.Player.PlayAsync(args.Track);
                b = new EmbedBuilder();
                b.WithTitle(string.Format(lang.Now_Playing_Loop, args.Track.Title));
                string artw = await args.Track.FetchArtworkAsync();
                b.WithThumbnailUrl(artw);
                b.WithFooter(lang.Requested_by + lang.Loopbot, "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await args.Player.TextChannel.SendMessageAsync(embed: b.Build());
                Console.WriteLine($"Now playing: {args.Track.Title} cause loopbot");
                return;
            }
            if (islooped.ContainsKey(args.Player.VoiceChannel.GuildId) && islooped[args.Player.VoiceChannel.GuildId] == 2)
            {
                player.Queue.Enqueue(args.Track);
            }
            if (!player.Queue.TryDequeue(out LavaTrack queueable))
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Queue_completed);
                await args.Player.TextChannel.SendMessageAsync("", false, b.Build());
                _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromSeconds(10));
                return;
            }

            if (queueable is not LavaTrack)
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Next_item_not_track);
                await args.Player.TextChannel.SendMessageAsync("", false, b.Build());
                return;
            }
            string art;
            if (queueable == null)
            {
                b = new EmbedBuilder();
                b.WithTitle(lang.Null_in_queue);
                await args.Player.TextChannel.SendMessageAsync("", false, b.Build());
                return;
            }

            await args.Player.PlayAsync(queueable);
            b = new EmbedBuilder();
            b.WithTitle(string.Format(lang.Now_Playing, args.Reason + ":" + args.Track.Title, queueable.Title));
            art = await queueable.FetchArtworkAsync();
            b.WithThumbnailUrl(art);
            b.WithFooter(lang.Requested_by + lang.Queuebot, "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
            await args.Player.TextChannel.SendMessageAsync("", false, b.Build());
        }
    }
}