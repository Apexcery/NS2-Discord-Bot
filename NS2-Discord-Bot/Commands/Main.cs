using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;

namespace NS2_Discord_Bot.Commands
{
    public class Main : ModuleBase<SocketCommandContext>
    {
        private async Task<(string ProfileUrl, HtmlDocument Document)> GetProfilePage(string steamId)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync($"https://observatory.morrolan.ch/players?filter={steamId}&last_active_after=2010-01-01");

            var profileUrl = doc.DocumentNode.SelectSingleNode("/html/body/div/div[5]/div/table/tbody/tr[1]/td[1]/a")?.GetAttributeValue("href", "N/A") ?? "N/A";

            if (profileUrl == "N/A")
            {
                await ReplyAsync("Could not find a profile with that Steam ID");
                return (null, null);
            }

            var url = "https://observatory.morrolan.ch" + profileUrl;
            doc = await web.LoadFromWebAsync(url);

            return (url, doc);
        }

        private string GetProfileName(HtmlDocument doc)
        {
            var header = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'page-header')]");
            var headerText = header.InnerText;

            headerText = headerText
                .Remove(headerText.IndexOf("Updated:", StringComparison.Ordinal))
                .Replace("\n", string.Empty);

            return headerText;
        }

        [Command("hive")]
        public async Task HiveLookup(string steamId)
        {
            var (profileUrl, document) = await GetProfilePage(steamId);

            if (document == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync("Could not find a profile with that Steam ID");
                return;
            }

            var profileName = GetProfileName(document);

            var (hiveSkill, hiveLevelExp, hiveScore, hiveScoreMin) = GetHiveStats(document);

            if (hiveSkill == null || hiveLevelExp == null || hiveScore == null || hiveScoreMin == null)
            {
                await ReplyAsync("Could not find the Hive details for that profile");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle(profileName)
                .WithUrl(profileUrl)
                .AddField("Skill:", hiveSkill)
                .AddField("Level / Exp:", hiveLevelExp)
                .AddField("Score:", hiveScore)
                .AddField("Score / Min:", hiveScoreMin)
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

        [Command("kdr")]
        public async Task KDRLookup(string steamId)
        {
            var (profileUrl, document) = await GetProfilePage(steamId);

            if (document == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync("Could not find a profile with that Steam ID");
                return;
            }

            var profileName = GetProfileName(document);

            var (alien30, alien100, marine30, marine100) = GetKdrStats(document);

            if (string.IsNullOrEmpty(alien30) || string.IsNullOrEmpty(alien100) || string.IsNullOrEmpty(marine30) || string.IsNullOrEmpty(marine100))
            {
                await ReplyAsync("Could not find the KDR details for that profile");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle(profileName)
                .WithUrl(profileUrl)
                .WithDescription("KDR From Last n Rounds")
                .AddField("Alien KDR | 30:", alien30)
                .AddField("Alien KDR | 100:", alien100)
                .AddField("Marine KDR | 30", marine30)
                .AddField("Marine KDR | 100", marine100)
                .WithColor(Color.Orange)
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        private (string Alien30, string Alien100, string Marine30, string Marine100) GetKdrStats(HtmlDocument profilePage)
        {
            var kdrHeader = profilePage.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Ingame')]");

            if (kdrHeader == null)
                return (null, null, null, null);

            var kdrTable = kdrHeader.NextSibling;
            while (!kdrTable.Name.Contains("table"))
            {
                kdrTable = kdrTable.NextSibling;
                if (kdrTable == null)
                    break;
            }

            if (kdrTable == null)
                return (null, null, null, null);

            var kdrTableBody = kdrTable.ChildNodes.FindFirst("tbody");

            var kdrAlienHeader = kdrTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Alien"))
                .FirstOrDefault(x => x != null);
            var kdrMarineHeader = kdrTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Marine"))
                .FirstOrDefault(x => x != null);

            if (kdrAlienHeader == null || kdrMarineHeader == null)
                return (null, null, null, null);

            #region alienKdr

            var kdrAlien30Node = kdrAlienHeader.NextSibling;
            while (!kdrAlien30Node.Name.Contains("td"))
            {
                kdrAlien30Node = kdrAlien30Node.NextSibling;
                if (kdrAlien30Node == null)
                    break;
            }
            if (kdrAlien30Node == null)
                return (null, null, null, null);

            var alien30Text = kdrAlien30Node.InnerText.Replace("&#39;", ",");

            var kdrAlien100Node = kdrAlien30Node.NextSibling;
            while (!kdrAlien100Node.Name.Contains("td"))
            {
                kdrAlien100Node = kdrAlien100Node.NextSibling;
                if (kdrAlien100Node == null)
                    break;
            }
            if (kdrAlien100Node == null)
                return (null, null, null, null);

            var alien100Text = kdrAlien100Node.InnerText.Replace("&#39;", ",");

            #endregion

            #region marineKdr

            var kdrMarine30Node = kdrMarineHeader.NextSibling;
            while (!kdrMarine30Node.Name.Contains("td"))
            {
                kdrMarine30Node = kdrMarine30Node.NextSibling;
                if (kdrMarine30Node == null)
                    break;
            }
            if (kdrMarine30Node == null)
                return (null, null, null, null);

            var marine30Text = kdrMarine30Node.InnerText.Replace("&#39;", ",");

            var kdrMarine100Node = kdrMarine30Node.NextSibling;
            while (!kdrMarine100Node.Name.Contains("td"))
            {
                kdrMarine100Node = kdrMarine100Node.NextSibling;
                if (kdrMarine100Node == null)
                    break;
            }
            if (kdrMarine100Node == null)
                return (null, null, null, null);

            var marine100Text = kdrMarine100Node.InnerText.Replace("&#39;", ",");

            #endregion

            return (alien30Text, alien100Text, marine30Text, marine100Text);
        }
    }
}