using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using dotnetcorebot.Modules;
using dotnetcorebot.Modules.infoclasses;
using Fortnite_API;
using GiphyDotNet.Manager;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Sentry.Protocol;
using SilverBotData;
using SilverBotLiteData;
using SilverCraftBot.Modules;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using static dotnetcorebot.Modules.Commands;

namespace SIlverCraftBot
{
    internal class Program
    {
        private static string prefix = "sd!";
        private static DiscordShardedClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private static Config config = new Config();

        public static void Main()
        {
            // Console.OutputEncoding = Encoding.ASCII;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("+----------------------------+" + Environment.NewLine +
                              "| SilverBot                  |" + Environment.NewLine +
                              "| ©SilverDimond and The      |" + Environment.NewLine +
                              "| SilverCraft team           |" + Environment.NewLine +
                              "| Special thanks to          |" + Environment.NewLine +
                              "| ThePajamaSlime#9391        |" + Environment.NewLine +
                              "+ ---------------------------+");
            Console.ResetColor();
            config = Config.Get();
            if (String.IsNullOrEmpty(config.SentryURL))
            {
                Version.Checkforupdates();
                Commands.Giphymodule(new Giphy(config.Gtoken));
                Commands.SetConfig(config);
                Fortnite.Setapi(new FortniteApi(config.Fortnite_Api_Token));
                AudioModule.SetConfig(config);
                Imagemodule.SetConfig(config);
                Weather.SetClient(new Awesomio.Weather.WeatherClient(config.OpenWeatherMap));
                prefix = config.Prefix;
                new Program().RunBotAsync().GetAwaiter().GetResult();
            }
            else
            {
                using (SentrySdk.Init(config.SentryURL))
                {
                    User user = new User
                    {
                        Username = Environment.UserName
                    };
                    SentrySdk.ConfigureScope(s => s.User = user);
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.SetExtra("Version", Version.vnumber);
                    });
                    Version.Checkforupdates();
                    Commands.Giphymodule(new Giphy(config.Gtoken));
                    Commands.SetConfig(config);
                    Fortnite.Setapi(new FortniteApi(config.Fortnite_Api_Token));
                    AudioModule.SetConfig(config);
                    Imagemodule.SetConfig(config);
                    Weather.SetClient(new Awesomio.Weather.WeatherClient(config.OpenWeatherMap));
                    prefix = config.Prefix;
                    new Program().RunBotAsync().GetAwaiter().GetResult();
                }
            }
        }

        private Process compiler;

        public static DiscordShardedClient GetClient()
        {
            return _client;
        }

        public async Task RunBotAsync()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            _client = new DiscordShardedClient(new DiscordSocketConfig
            {
                ExclusiveBulkDelete = false,
                TotalShards = 2,
                AlwaysDownloadUsers = true,
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
                ThrowOnError = false
            });
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<InteractiveService>()
                .AddLavaNode(x =>
                {
                    x.ReconnectAttempts = 3;
                    x.ReconnectDelay = TimeSpan.FromSeconds(3);
                })
                .BuildServiceProvider();
            _client.Log += Client_Log;
            _commands.Log += Client_Log;
            await RegisterCommandsAsync().ConfigureAwait(false);
            _client.UserJoined += AnnounceJoinedUser;
            compiler = new Process();
            compiler.StartInfo.FileName = config.Location_to_java;
            compiler.StartInfo.Arguments = "-jar Lavalink.jar";
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.CreateNoWindow = false;
            compiler.StartInfo.RedirectStandardOutput = false;
            compiler.Start();
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();
            await Task.Delay(5000).ConfigureAwait(false);
            string streamUrlj = null;
            ActivityType type = ActivityType.Playing;
            await _client.SetGameAsync("Booting up", streamUrlj, type);
            await Task.Delay(_client.Shards.Count * 2500).ConfigureAwait(false);
            SocketTextChannel channel = _client.GetChannel(config.Log_channel) as SocketTextChannel;
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("Bot loading.");
            b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
            b.WithThumbnailUrl("https://cdn.discordapp.com/attachments/728360861483401240/728360959139381339/Spin-0.7s-255px.gif");
            await channel.SendMessageAsync("", false, b.Build());
            LavaNode _instanceOfLavaNode = _services.GetRequiredService<LavaNode>();
            if (!_instanceOfLavaNode.IsConnected)
            {
                _ = _instanceOfLavaNode.ConnectAsync();
            }
            AudioModule.Musicmodule(_instanceOfLavaNode);
            await SetSplashes().ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }

        public static Config GetConfig()
        {
            return config;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                compiler.Kill();
            }
            catch (Exception ee)
            {
                SentrySdk.CaptureException(ee);
            }
        }

        internal static async Task SetSplashes()
        {
            SocketTextChannel channel = _client.GetChannel(config.Log_channel) as SocketTextChannel;

            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("Embeded splash mode :(");
            b.WithDescription($"Reading from file is not implemented in this version. blame silverdimond! {Splashes.Get.Length} splashes are loaded");
            b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
            b.WithThumbnailUrl("https://cdn.discordapp.com/attachments/728360861483401240/728361135665053697/red_flag.png");

            await channel.SendMessageAsync("", false, b.Build());

            while (true)
            {
                Random rand = new Random();
                int d = Splashes.Get.Length;
                int _rnad = rand.Next(0, d);
                string streamUrlj = null;
                ActivityType type = Splashes.Get[_rnad].Item2;
                await _client.SetGameAsync(Splashes.Get[_rnad].Item1 + " ▪ " + prefix + "help", streamUrlj, type);
                await Task.Delay(10000).ConfigureAwait(false);
            }
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModuleAsync<Commands>(_services);
            await _commands.AddModuleAsync<AudioModule>(_services);
            await _commands.AddModuleAsync<Imagemodule>(_services);
            await _commands.AddModuleAsync<Fortnite>(_services);
            await _commands.AddModuleAsync<Devserveronly>(_services);
            await _commands.AddModuleAsync<RandomCommands>(_services);
            await _commands.AddModuleAsync<CompanionModule>(_services);
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.ForegroundColor = arg.Severity switch
            {
                LogSeverity.Critical or LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Warning => ConsoleColor.Yellow,
                LogSeverity.Info => ConsoleColor.White,
                LogSeverity.Verbose or LogSeverity.Debug => ConsoleColor.DarkGray,
                _ => ConsoleColor.DarkMagenta,
            };
            Console.WriteLine($"[SilverBotLog]{DateTime.Now} [{arg.Severity}] {arg.Source}: {arg.Message} {arg.Exception}");
            Console.ResetColor();
            // If you get an error saying 'CompletedTask' doesn't exist,
            // your project is targeting .NET 4.5.2 or lower. You'll need
            // to adjust your project's target framework to 4.6 or higher
            // (instructions for this are easily Googled).
            // If you *need* to run on .NET 4.5 for compat/other reasons,
            // the alternative is to 'return Task.Delay(0);' instead.
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message)
            {
                return;
            }
            ShardedCommandContext context = new ShardedCommandContext(_client, message);
            if (_client.CurrentUser == null || message.Author.Id == _client.CurrentUser.Id || message.Author.IsBot)
            {
                return;
            }
            int argPos = 0;
            if (message.HasStringPrefix(prefix, ref argPos) || message.HasStringPrefix(_client.CurrentUser.Mention, ref argPos))
            {
                IResult result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }

                SocketTextChannel channel = _client.GetChannel(config.Log_channel) as SocketTextChannel;
                if (!result.IsSuccess)
                {
                    EmbedBuilder b = new EmbedBuilder();
                    if (result.Error == CommandError.BadArgCount)
                    {
                        b = new EmbedBuilder();
                        b.WithTitle("You specified more or less arguments then needed");
                        b.WithDescription("More sent to bot-logs");
                        b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                        await message.Channel.SendMessageAsync("", false, b.Build());
                        b = new EmbedBuilder();
                        b.WithTitle("Error while executing command BAD ARG COUNT");
                        b.WithDescription("More detailed explination below");
                        b.AddField("Error reason", result.ErrorReason);
                        b.AddField("Message channel", message.Channel);
                        b.AddField("Message content", message.Content);
                        b.AddField("Message url", message.GetJumpUrl());
                        b.AddField("Error", result.Error);
                        b.AddField("IsSuccess", result.IsSuccess);
                        b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                        await channel.SendMessageAsync("", false, b.Build());
                        return;
                    }
                    else if (result.Error == CommandError.UnknownCommand)
                    {
                        //b = new EmbedBuilder();
                        //b.WithTitle("Unknown command");
                        //b.WithDescription("More sent to bot-logs");
                        //b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                        //await message.Channel.SendMessageAsync("", false, b.Build());
                        //b = new EmbedBuilder();
                        //b.WithTitle("Error while executing command unknown command");
                        //b.WithDescription("More detailed explination below");
                        //b.AddField("Error reason", result.ErrorReason);
                        //b.AddField("Message channel", message.Channel);
                        //b.AddField("Message content", message.Content);
                        //b.AddField("Message url", message.GetJumpUrl());
                        //b.AddField("Error", result.Error);
                        //b.AddField("IsSuccess", result.IsSuccess);
                        //b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                        //await channel.SendMessageAsync("", false, b.Build());
                        return;
                    }
                    else if (result.Error == CommandError.UnmetPrecondition)
                    {
                        b = new EmbedBuilder();
                        b.WithTitle(result.ErrorReason);
                        b.WithDescription("More sent to bot-logs");
                        b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                        await message.Channel.SendMessageAsync("", false, b.Build());
                        b = new EmbedBuilder();
                        b.WithTitle("Error while executing command unmet precondition");
                        b.WithDescription("More detailed explination below");
                        b.AddField("Error reason", result.ErrorReason);
                        b.AddField("Message channel", message.Channel);
                        b.AddField("Message content", message.Content);
                        b.AddField("Message url", message.GetJumpUrl());
                        b.AddField("Error", result.Error);
                        b.AddField("IsSuccess", result.IsSuccess);
                        b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                        await channel.SendMessageAsync("", false, b.Build());
                        return;
                    }
                    b.WithTitle("Error while executing command");
                    b.WithDescription("More detailed explination below");
                    b.AddField("Error reason", result.ErrorReason);
                    b.AddField("Message channel", message.Channel);
                    b.AddField("Message content", message.Content);
                    b.AddField("Message url", message.GetJumpUrl());
                    b.AddField("Error", result.Error);
                    b.AddField("IsSuccess", result.IsSuccess);
                    b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                    Discord.Rest.RestUserMessage e = await channel.SendMessageAsync("", false, b.Build());
                    b = new EmbedBuilder();

                    b.WithTitle("Error while executing command");
                    b.WithDescription("More sent to bot-logs");
                    b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                    await message.Channel.SendMessageAsync("", false, b.Build());

                    SentrySdk.CaptureException(new CommandException(result, " " + e.GetJumpUrl()));
                    throw new CommandException(result, " " + e.GetJumpUrl());
                    // SentrySdk.CaptureMessage(gay.GetJumpUrl());
                }
            }
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            SilverBotData.Serverthing thing = SilverBotLiteData.Serverthing.Getbyid(user.Guild.Id);
            if (thing != null)
            {
                ulong id = thing.ChannelId;
                if (_client.GetChannel(id) is not SocketTextChannel channel)
                {
                    Console.WriteLine("deleted " + thing.ChannelId + " as it returned null" + SilverBotLiteData.Serverthing.Removebyid(id, user.Guild.Id));
                    return;
                }
                EmbedBuilder b = new EmbedBuilder();
                b.WithTitle("Welcome " + user.Username + " to the " + user.Guild.Name + " discord server");
                b.WithDescription("You are the " + user.Guild.MemberCount + "th member.");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                b.WithThumbnailUrl(user.GetAvatarUrl());
                if (user.GetAvatarUrl() == null)
                {
                    b.WithThumbnailUrl(user.GetDefaultAvatarUrl());
                }
                try
                {
                    await channel.SendMessageAsync("", false, b.Build());
                }
                catch (Exception err)
                {
                    SentrySdk.CaptureException(err);
                    throw;
                }
            }
        }
    }

    [Serializable]
    internal class CommandException : Exception
    {
        public CommandException()
        {
        }

        public CommandException(IResult result, string info)
        : base(System.Text.Json.JsonSerializer.Serialize(result) + info)
        {
            Error = result.Error;
            ErrorReason = result.ErrorReason;
            IsSuccess = result.IsSuccess;
        }

        //
        // Summary:
        //     Describes the error type that may have occurred during the operation.
        //
        // Returns:
        //     A Discord.Commands.CommandError indicating the type of error that may have occurred
        //     during the operation; null if the operation was successful.
        public CommandError? Error { get; }

        //
        // Summary:
        //     Describes the reason for the error.
        //
        // Returns:
        //     A string containing the error reason.
        public string ErrorReason { get; }

        //
        // Summary:
        //     Indicates whether the operation was successful or not.
        //
        // Returns:
        //     true if the result is positive; otherwise false.
        public bool IsSuccess { get; }
    }
}