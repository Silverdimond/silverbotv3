using Discord;
using Discord.Addons.Interactive;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using dotnetcorebot.Modules;
using SIlverCraftBot;
using SIlverCraftBot.Modules;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SilverCraftBot.Modules
{
    internal class Devserveronly : InteractiveBase
    {
        [Command("useragent", RunMode = RunMode.Async)]
        public async Task Useragent()
        {
            if (Context.Guild.Id == 699361201586438235 && Commands.Is_at_silvercraft(Context.User) && Commands.Is_bot_admin(Context.User))
            {
                EmbedBuilder b = new EmbedBuilder();

                b.WithTitle("Useragent");
                b.WithDescription(Webclient.Get().DefaultRequestHeaders.UserAgent.ToString());
                await ReplyAsync(embed: b.Build());
                await Webclient.Get().GetAsync("https://silvercraftbotversion.glitch.me/");
            }
        }

        [Command("create category", RunMode = RunMode.Async)]
        public async Task King(SocketGuildUser eee)
        {
            if (Context.Guild.Id == 699361201586438235 && Commands.Is_at_silvercraft(Context.User) && Commands.Is_bot_admin(Context.User))
            {
                EmbedBuilder b = new EmbedBuilder();
                SocketUser user = Context.User;

                b.WithTitle("e");
                b.WithDescription("https://silverbot.silverdimond.tk/#/");
                b.WithFooter("req by lel" + user.Username, user.GetAvatarUrl());
                await ReplyAsync(embed: b.Build());
                string name = eee.Username;
                name = name.Replace(" ", "_");
                OverwritePermissions overwritePermissions = new OverwritePermissions(sendMessages: PermValue.Allow, manageChannel: PermValue.Allow, connect: PermValue.Allow, manageRoles: PermValue.Allow);
                OverwritePermissions overwritePermissionse = new OverwritePermissions(sendMessages: PermValue.Deny, manageChannel: PermValue.Deny, connect: PermValue.Deny, manageRoles: PermValue.Deny);
                Discord.Rest.RestCategoryChannel thing = await Context.Guild.CreateCategoryChannelAsync(name);
                await thing.AddPermissionOverwriteAsync(Context.Client.CurrentUser, overwritePermissions);
                await thing.AddPermissionOverwriteAsync(eee, overwritePermissions);
                await thing.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, overwritePermissionse);
                Discord.Rest.RestTextChannel channel = await Context.Guild.CreateTextChannelAsync(name);
                await channel.ModifyAsync(prop => prop.CategoryId = thing.Id);
                await channel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, overwritePermissions);
                await channel.AddPermissionOverwriteAsync(eee, overwritePermissions);

                await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, overwritePermissionse);
                Discord.Rest.RestVoiceChannel vchannel = await Context.Guild.CreateVoiceChannelAsync(name);
                await vchannel.ModifyAsync(prop => prop.CategoryId = thing.Id); await vchannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, overwritePermissions);
                await vchannel.AddPermissionOverwriteAsync(eee, overwritePermissions);

                await vchannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, overwritePermissionse);
            }
        }
    }
}