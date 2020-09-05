using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace NS2_Discord_Bot.Commands
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Usage")
                .AddField("Hive Stats", "!ns2 hive <steam ID>")
                .AddField("KDR Stats", "!ns2 kdr <steam ID>")
                .WithColor(Color.Gold)
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
