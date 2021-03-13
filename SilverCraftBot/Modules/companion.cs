using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using NetCoreWeatherForecastLibrary.Models.CurrentWeather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverCraftBot.Modules
{
    internal class UserSettings
    {
        public readonly ulong UserId;
        public ulong Id;
        public string Location;
    }

    [Group("companion")]
    internal class CompanionModule : InteractiveBase
    {
        private static readonly TimeSpan fivemin = new TimeSpan(0, 5, 10);

        [Command("enroll", RunMode = RunMode.Async)]
        public async Task enroll()
        {
            await ReplyAsync("In what city do you live in?");
            SocketMessage response = await NextMessageAsync(timeout: fivemin);
            var weather = Weather.GetClient();

            var currentweather = await weather.GetCurrentWeather<CurrrentWeatherModel>(response.Content, "en");
            TimeSpan difference_from_utc = TimeSpan.FromSeconds(currentweather.Timezone);
            DateTime now = DateTime.UtcNow + difference_from_utc;
            await ReplyAsync($"You live in {currentweather.Name}? So the time is {now.ToShortTimeString()},  the weather feels like {currentweather.Main.FeelsLike}°C and its {currentweather.Main.Temp}°C {currentweather.Main.TempMin}°C-{currentweather.Main.TempMax}°C? (yes/no)");
        }
    }
}

public class RequireEnrolled : PreconditionAttribute
{
    // Create a constructor so the name can be specified
    public RequireEnrolled()
    {
    }

    // Override the CheckPermissions method
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        using LiteDatabase db = new LiteDatabase(@"Filename=usersettings.db; Connection=shared");
        ILiteCollection<SilverCraftBot.Modules.UserSettings> col = db.GetCollection<SilverCraftBot.Modules.UserSettings>();

        col.EnsureIndex(x => x.UserId);
        var thing = col.FindOne(x => x.UserId == context.User.Id);
        // If this command was executed by a user with the appropriate role, return a success
        if (thing != null)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
        else
            return Task.FromResult(PreconditionResult.FromError($"You must be enrolled in the companion beta"));
    }
}