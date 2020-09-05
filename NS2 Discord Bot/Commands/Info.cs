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
                .AddField("Usage", "!ns2 hive <steam ID>", false)
                .WithColor(Color.Gold)
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
