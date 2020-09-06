using System;
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
                .AddField("Link Profile", $"{prefix}link <steam ID> (leave blank to view linked profile)")
                .AddField("Hive Stats", $"{prefix}hive <steam ID> (leave blank to use linked profile)")
                .AddField("KDR Stats", $"{prefix}kdr <steam ID> (leave blank to use linked profile)")
                .WithColor(Color.GetRandomColor())
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
