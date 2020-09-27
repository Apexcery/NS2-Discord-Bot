using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using HtmlAgilityPack;
using NS2_Discord_Bot.Services;

namespace NS2_Discord_Bot.Commands
{
    public class Chart : ModuleBase<SocketCommandContext>
    {
        private async Task<(List<string> dateList, List<double> skillList, List<double> scoreList, List<double> scorePerMinList, List<double> experienceList, List<double> levelList)> BaseCommand(string steamId)
        {
            var linkedProfile = string.IsNullOrEmpty(steamId);

            if (string.IsNullOrEmpty(steamId))
            {
                steamId = await Profile.GetSteamIdFromDiscordId(Context.User.Id);

                if (steamId == null)
                {
                    await ReplyAsync("You need to specify a Steam ID or link your profile");
                    return (null, null, null, null, null, null);
                }
            }

            var (profileUrl, profilePage) = await Profile.GetProfilePage(steamId);

            if (profilePage == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync($"Could not find a profile with {(linkedProfile ? "the Steam ID you have linked" : "that Steam ID")}");
                return (null, null, null, null, null, null);
            }

            var profileName = Profile.GetProfileName(profilePage);

            return GetSkillData(profilePage);
        }

        [Command("skill")]
        public async Task SkillChart(string steamId = null)
        {
            var (dateList, skillList, scoreList, scorePerMinList, experienceList, levelList) = await BaseCommand(steamId);

            if (dateList == null || skillList == null || scoreList == null || scorePerMinList == null || experienceList == null || levelList == null)
            {
                await ReplyAsync("An error occurred.");
                return;
            }

            Charts.SaveLineChart("Skill", dateList, skillList);
        }

        [Command("score")]
        public async Task ScoreChart(string steamId = null)
        {
            var (dateList, skillList, scoreList, scorePerMinList, experienceList, levelList) = await BaseCommand(steamId);

            if (dateList == null || skillList == null || scoreList == null || scorePerMinList == null || experienceList == null || levelList == null)
            {
                await ReplyAsync("An error occurred.");
                return;
            }

            Charts.SaveLineChart("Score", dateList, scoreList);
        }

        [Command("scorepermin")]
        public async Task ScorePerMinChart(string steamId = null)
        {
            var (dateList, skillList, scoreList, scorePerMinList, experienceList, levelList) = await BaseCommand(steamId);

            if (dateList == null || skillList == null || scoreList == null || scorePerMinList == null || experienceList == null || levelList == null)
            {
                await ReplyAsync("An error occurred.");
                return;
            }

            Charts.SaveLineChart("Score / Min", dateList, scorePerMinList);
        }

        [Command("experience"), Alias("exp", "xp")]
        public async Task ExperienceChart(string steamId = null)
        {
            var (dateList, skillList, scoreList, scorePerMinList, experienceList, levelList) = await BaseCommand(steamId);

            if (dateList == null || skillList == null || scoreList == null || scorePerMinList == null || experienceList == null || levelList == null)
            {
                await ReplyAsync("An error occurred.");
                return;
            }

            Charts.SaveLineChart("Experience", dateList, experienceList);
        }

        [Command("level"), Alias("lvl")]
        public async Task LevelChart(string steamId = null)
        {
            var (dateList, skillList, scoreList, scorePerMinList, experienceList, levelList) = await BaseCommand(steamId);

            if (dateList == null || skillList == null || scoreList == null || scorePerMinList == null || experienceList == null || levelList == null)
            {
                await ReplyAsync("An error occurred.");
                return;
            }

            Charts.SaveLineChart("Level", dateList, levelList);
        }

        private static (List<string> dateList, List<double> skillList, List<double> scoreList, List<double> scorePerMinList, List<double> experienceList, List<double> levelList) GetSkillData(HtmlDocument profilePage)
        {
            var skillProgressionHeader = profilePage.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Skill progression')]");
            if (skillProgressionHeader == null)
                return (null, null, null, null, null, null);

            var skillProgressionTable = skillProgressionHeader.NextSibling;
            while (!skillProgressionTable.Name.Contains("table"))
            {
                skillProgressionTable = skillProgressionTable.NextSibling;
                if (skillProgressionTable == null)
                    break;
            }
            if (skillProgressionTable == null)
                return (null, null, null, null, null, null);

            var skillProgressionTableBody = skillProgressionTable.ChildNodes.FindFirst("tbody");
            var numRows = skillProgressionTableBody.ChildNodes.Count(x => x.Name.Equals("tr", StringComparison.OrdinalIgnoreCase));
            var first5Rows = skillProgressionTableBody.ChildNodes
                .Where(x => x.Name.Equals("tr", StringComparison.OrdinalIgnoreCase))
                .Take(numRows >= 5 ? 5 : numRows)
                .Reverse()
                .Select(x => x.ChildNodes)
                .Select(x => x.Where(z => z.Name.Equals("td", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var dateList = first5Rows.Select(x => x.ElementAt(0).InnerText).ToList();
            var skillList = first5Rows.Select(x => double.Parse(x.ElementAt(1).InnerText.Replace("&#39;", ""))).ToList();
            var scoreList = first5Rows.Select(x => double.Parse(x.ElementAt(2).InnerText.Replace("&#39;", ""))).ToList();
            var scorePerMinList = first5Rows.Select(x => double.Parse(x.ElementAt(3).InnerText.Replace("&#39;", ""))).ToList();
            var experienceList = first5Rows.Select(x => double.Parse(x.ElementAt(4).InnerText.Replace("&#39;", ""))).ToList();
            var levelList = first5Rows.Select(x => double.Parse(x.ElementAt(5).InnerText.Replace("&#39;", ""))).ToList();

            return (dateList, skillList, scoreList, scorePerMinList, experienceList, levelList);
        }
    }
}
