using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NS2_Discord_Bot.Models;
using NS2_Discord_Bot.Services;
using Color = NS2_Discord_Bot.Services.Color;

namespace NS2_Discord_Bot.Commands
{
    public class Accuracy : ModuleBase<SocketCommandContext>
    {
        [Command("accuracy")]
        public async Task AccuracyLookup(string steamId = null)
        {
            var linkedProfile = string.IsNullOrEmpty(steamId);

            if (string.IsNullOrEmpty(steamId))
            {
                steamId = await Profile.GetSteamIdFromDiscordId(Context.User.Id);

                if (steamId == null)
                {
                    await ReplyAsync("You need to specify a Steam ID or link your profile");
                    return;
                }
            }

            var (profileUrl, profilePage) = await Profile.GetProfilePage(steamId);

            if (profilePage == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync($"Could not find a profile with {(linkedProfile ? "the Steam ID you have linked" : "that Steam ID")}");
                return;
            }

            var profileName = Profile.GetProfileName(profilePage);

            var (alien30, alien100, marineExclOnos30, marineExclOnos100, marineInclOnos30, marineInclOnos100) = GetAccuracyStats(profilePage);

            if (string.IsNullOrEmpty(alien30) || string.IsNullOrEmpty(alien100) || string.IsNullOrEmpty(marineExclOnos30) || string.IsNullOrEmpty(marineExclOnos100) || string.IsNullOrEmpty(marineInclOnos30) || string.IsNullOrEmpty(marineInclOnos100))
            {
                await ReplyAsync($"Could not find the Accuracy details for {(linkedProfile ? "the Steam ID you have linked" : "that Steam ID")}");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle(profileName)
                .WithUrl(profileUrl)
                .WithDescription("Accuracy From Last n Rounds")
                .AddField("Alien Accuracy | 30:", alien30)
                .AddField("Alien Accuracy | 100:", alien100)
                .AddField("Marine (excl. Onos) Accuracy | 30", marineExclOnos30)
                .AddField("Marine (excl. Onos) Accuracy | 100", marineExclOnos100)
                .AddField("Marine (incl. Onos) Accuracy | 30", marineInclOnos30)
                .AddField("Marine (incl. Onos) Accuracy | 100", marineInclOnos100)
                .WithColor(Color.GetRandomDiscordColor())
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        private (string Alien30, string Alien100, string MarineExclOnos30, string MarineExclOnos100, string MarineInclOnos30, string MarineInclOnos100) GetAccuracyStats(HtmlDocument profilePage)
        {
            var inGameHeader = profilePage.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Ingame')]");

            if (inGameHeader == null)
                return (null, null, null, null, null, null);

            var kdrTable = inGameHeader.NextSibling;
            while (!kdrTable.Name.Contains("table"))
            {
                kdrTable = kdrTable.NextSibling;
                if (kdrTable == null)
                    break;
            }

            if (kdrTable == null)
                return (null, null, null, null, null, null);

            var accuracyTable = kdrTable.NextSibling;
            while (!accuracyTable.Name.Contains("table"))
            {
                accuracyTable = accuracyTable.NextSibling;
                if (accuracyTable == null)
                    break;
            }

            if (accuracyTable == null)
                return (null, null, null, null, null, null);

            var accuracyTableBody = accuracyTable.ChildNodes.FindFirst("tbody");

            var accuracyAlienHeader = accuracyTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Alien"))
                .FirstOrDefault(x => x != null);
            var accuracyMarineExclHeader = accuracyTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Marine (excl. Onos)"))
                .FirstOrDefault(x => x != null);
            var accuracyMarineInclHeader = accuracyTableBody.ChildNodes.Select(x => x.ChildNodes.FirstOrDefault(z => z.Name == "th" && z.InnerText == "Marine (incl. Onos)"))
                .FirstOrDefault(x => x != null);

            if (accuracyAlienHeader == null || accuracyMarineExclHeader == null || accuracyMarineInclHeader == null)
                return (null, null, null, null, null, null);

            #region alienAccuracy

            var accuracyAlien30Node = accuracyAlienHeader.NextSibling;
            while (!accuracyAlien30Node.Name.Contains("td"))
            {
                accuracyAlien30Node = accuracyAlien30Node.NextSibling;
                if (accuracyAlien30Node == null)
                    break;
            }
            if (accuracyAlien30Node == null)
                return (null, null, null, null, null, null);

            var alien30Text = accuracyAlien30Node.InnerText.Replace("&#39;", ",");

            var accuracyAlien100Node = accuracyAlien30Node.NextSibling;
            while (!accuracyAlien100Node.Name.Contains("td"))
            {
                accuracyAlien100Node = accuracyAlien100Node.NextSibling;
                if (accuracyAlien100Node == null)
                    break;
            }
            if (accuracyAlien100Node == null)
                return (null, null, null, null, null, null);

            var alien100Text = accuracyAlien100Node.InnerText.Replace("&#39;", ",");

            #endregion

            #region marineExclAccuracy

            var accuracyMarineExcl30Node = accuracyMarineExclHeader.NextSibling;
            while (!accuracyMarineExcl30Node.Name.Contains("td"))
            {
                accuracyMarineExcl30Node = accuracyMarineExcl30Node.NextSibling;
                if (accuracyMarineExcl30Node == null)
                    break;
            }
            if (accuracyMarineExcl30Node == null)
                return (null, null, null, null, null, null);

            var marineExcl30Text = accuracyMarineExcl30Node.InnerText.Replace("&#39;", ",");

            var accuracyMarineExcl100Node = accuracyMarineExcl30Node.NextSibling;
            while (!accuracyMarineExcl100Node.Name.Contains("td"))
            {
                accuracyMarineExcl100Node = accuracyMarineExcl100Node.NextSibling;
                if (accuracyMarineExcl100Node == null)
                    break;
            }
            if (accuracyMarineExcl100Node == null)
                return (null, null, null, null, null, null);

            var marineExcl100Text = accuracyMarineExcl100Node.InnerText.Replace("&#39;", ",");

            #endregion

            #region marineInclAccuracy

            var accuracyMarineIncl30Node = accuracyMarineInclHeader.NextSibling;
            while (!accuracyMarineIncl30Node.Name.Contains("td"))
            {
                accuracyMarineIncl30Node = accuracyMarineIncl30Node.NextSibling;
                if (accuracyMarineIncl30Node == null)
                    break;
            }
            if (accuracyMarineIncl30Node == null)
                return (null, null, null, null, null, null);

            var marineIncl30Text = accuracyMarineIncl30Node.InnerText.Replace("&#39;", ",");

            var accuracyMarineIncl100Node = accuracyMarineIncl30Node.NextSibling;
            while (!accuracyMarineIncl100Node.Name.Contains("td"))
            {
                accuracyMarineIncl100Node = accuracyMarineIncl100Node.NextSibling;
                if (accuracyMarineIncl100Node == null)
                    break;
            }
            if (accuracyMarineIncl100Node == null)
                return (null, null, null, null, null, null);

            var marineIncl100Text = accuracyMarineIncl100Node.InnerText.Replace("&#39;", ",");

            #endregion

            return (alien30Text, alien100Text, marineExcl30Text, marineExcl100Text, marineIncl30Text, marineIncl100Text);
        }
    }
}
