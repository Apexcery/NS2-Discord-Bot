using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;
using NS2_Discord_Bot.Services;

namespace NS2_Discord_Bot.Commands
{
    public class Hive : ModuleBase<SocketCommandContext>
    {
        [Command("hive")]
        public async Task HiveLookup(string steamId)
        {
            var (profileUrl, profilePage) = await Profile.GetProfilePage(steamId);

            if (profilePage == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync("Could not find a profile with that Steam ID");
                return;
            }

            var profileName = Profile.GetProfileName(profilePage);

            var (hiveSkill, hiveLevelExp, hiveScore, hiveScoreMin) = GetHiveStats(profilePage);

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
    }
}
