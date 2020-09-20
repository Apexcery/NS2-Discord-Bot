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
    public class KDR : ModuleBase<SocketCommandContext>
    {
        [Command("kdr")]
        public async Task KDRLookup(string steamId = null)
        {
            var linkedProfile = string.IsNullOrEmpty(steamId);

            if (string.IsNullOrEmpty(steamId))
            {
                steamId = await Profile.GetProfileLinkFromDiscordId(Context.User.Id);

                if (steamId == null)
                {
                    await ReplyAsync("You need to specify a Steam ID or link your profile");
                    return;
                }
            }

            var (profileUrl, document) = await Profile.GetProfilePage(steamId);

            if (document == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync($"Could not find a profile with {(linkedProfile ? "the Steam ID you have linked" : "that Steam ID")}");
                return;
            }

            var profileName = Profile.GetProfileName(document);

            var (alien30, alien100, marine30, marine100) = GetKdrStats(document);

            if (string.IsNullOrEmpty(alien30) || string.IsNullOrEmpty(alien100) || string.IsNullOrEmpty(marine30) || string.IsNullOrEmpty(marine100))
            {
                await ReplyAsync($"Could not find the KDR details for {(linkedProfile ? "the Steam ID you have linked" : "that Steam ID")}");
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
                .WithColor(Color.GetRandomColor())
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
