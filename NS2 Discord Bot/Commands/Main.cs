using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;

namespace NS2_Discord_Bot.Commands
{
    public class Main : ModuleBase<SocketCommandContext>
    {
        [Command("hive")]
        public async Task HiveLookup(string steamId)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync($"https://observatory.morrolan.ch/players?filter={steamId}&last_active_after=2010-01-01");

            var profileUrl = doc.DocumentNode.SelectSingleNode("/html/body/div/div[5]/div/table/tbody/tr[1]/td[1]/a")?.GetAttributeValue("href", "N/A") ?? "N/A";

            if (profileUrl == "N/A")
            {
                await ReplyAsync("Could not find a profile with that Steam ID");
                return;
            }

            doc = await web.LoadFromWebAsync("https://observatory.morrolan.ch" + profileUrl);

            var hiveStats = GetHiveStats(doc);

            if (hiveStats.HiveSkill == null || hiveStats.HiveLevelExp == null)
            {
                await ReplyAsync("Could not find the Hive details for that profile");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle("Hive Stats For: ")
                .AddField("Skill:", hiveStats.HiveSkill)
                .AddField("Level / Exp:", hiveStats.HiveLevelExp)
                .AddField("Score:", hiveStats.HiveScore)
                .AddField("Score / Min:", hiveStats.HiveScoreMin)
                .WithColor(Color.Orange)
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        private (string HiveSkill, string HiveLevelExp, string HiveScore, string HiveScoreMin) GetHiveStats(HtmlDocument profilePage)
        {
            var hiveHeader = profilePage.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Hive')]");

            if (hiveHeader == null)
                return (null, null, null, null);

            var hiveTable = hiveHeader.NextSibling;
            while (!hiveTable.Name.Contains("table"))
            {
                hiveTable = hiveTable.NextSibling;
                if (hiveTable == null)
                    break;
            }

            if (hiveTable == null)
                return (null, null, null, null);

            var hiveTableBody = hiveTable.ChildNodes.FindFirst("tbody");

            var hiveSkillHeader = hiveTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Skill"))
                .FirstOrDefault(x => x != null);
            var hiveLevelExpHeader = hiveTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Level / Exp"))
                .FirstOrDefault(x => x != null);
            var hiveScoreHeader = hiveTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Score"))
                .FirstOrDefault(x => x != null);
            var hiveScoreMinHeader = hiveTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Score / min"))
                .FirstOrDefault(x => x != null);

            if (hiveSkillHeader == null || hiveLevelExpHeader == null || hiveScoreHeader == null || hiveScoreMinHeader == null)
                return (null, null, null, null);

            #region hiveSkill

            var hiveSkillNode = hiveSkillHeader.NextSibling;
            while (!hiveSkillNode.Name.Contains("td"))
            {
                hiveSkillNode = hiveSkillNode.NextSibling;
                if (hiveSkillNode == null)
                    break;
            }
            if (hiveSkillNode == null)
                return (null, null, null, null);

            var replacedSkillText = hiveSkillNode.InnerText.Replace("&#39;", ",");

            #endregion

            #region hiveLevelExp

            var hiveLevelExpNode = hiveLevelExpHeader.NextSibling;
            while (!hiveLevelExpNode.Name.Contains("td"))
            {
                hiveLevelExpNode = hiveLevelExpNode.NextSibling;
                if (hiveLevelExpNode == null)
                    break;
            }
            if (hiveLevelExpNode == null)
                return (null, null, null, null);

            var replacedLevelExpText = hiveLevelExpNode.InnerText.Replace("&#39;", ",");

            #endregion

            #region hiveScore

            var hiveScoreNode = hiveScoreHeader.NextSibling;
            while (!hiveScoreNode.Name.Contains("td"))
            {
                hiveScoreNode = hiveScoreNode.NextSibling;
                if (hiveScoreNode == null)
                    break;
            }
            if (hiveScoreNode == null)
                return (null, null, null, null);

            var replacedScoreText = hiveScoreNode.InnerText.Replace("&#39;", ",");

            #endregion

            #region hiveScoreMin

            var hiveScoreMinNode = hiveScoreMinHeader.NextSibling;
            while (!hiveScoreMinNode.Name.Contains("td"))
            {
                hiveScoreMinNode = hiveScoreMinNode.NextSibling;
                if (hiveScoreMinNode == null)
                    break;
            }
            if (hiveScoreMinNode == null)
                return (null, null, null, null);

            var replacedScoreMinText = hiveScoreMinNode.InnerText.Replace("&#39;", ",");

            #endregion

            return (HiveSkill: replacedSkillText, HiveLevelExp: replacedLevelExpText, HiveScore: replacedScoreText, HiveScoreMin: replacedScoreMinText);
        }
    }
}