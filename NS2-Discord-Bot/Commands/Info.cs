using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Color = NS2_Discord_Bot.Services.Color;

namespace NS2_Discord_Bot.Commands
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var prefix = Environment.GetEnvironmentVariable("ns2-discord-prefix");

            var embed = new EmbedBuilder()
                .WithTitle("Usage")
                .AddField("Changelog", $"{prefix}changelog\nView the most recent changes to this bot.")
                .AddField("Link Profile", $"{prefix}link <steam ID> (leave blank to view linked profile)\nLink your profile to save typing it for every command.")
                .AddField("Hive Stats", $"{prefix}hive <steam ID> (leave blank to use linked profile)\nView the hive stats for the specified user.")
                .AddField("KDR Stats", $"{prefix}kdr <steam ID> (leave blank to use linked profile)\nView the kdr stats for the specified user.")
                .WithColor(Color.GetRandomColor())
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("changelog")]
        public async Task Changelog()
        {
            var allChanges = await File.ReadAllLinesAsync("recentChanges.txt");
            var recentChanges = allChanges.Select(x =>
            {
                var temp = x.Split(':');
                return (temp[0].Trim(), temp[1].Trim());
            }).ToList();

            var embed = new EmbedBuilder()
                .WithTitle("Changelog")
                .WithColor(Color.GetRandomColor());

            foreach (var (name, change) in recentChanges)
            {
                embed = embed.AddField(name, change);
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
