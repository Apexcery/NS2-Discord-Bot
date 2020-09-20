using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NS2_Discord_Bot.Models;

namespace NS2_Discord_Bot.Services
{
    public class Profile
    {
        public static async Task<(string ProfileUrl, HtmlDocument Document)> GetProfilePage(string steamId)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync($"https://observatory.morrolan.ch/players?filter={steamId}&last_active_after=2010-01-01");

            var profileUrl = doc.DocumentNode.SelectSingleNode("/html/body/div/div[5]/div/table/tbody/tr[1]/td[1]/a")?.GetAttributeValue("href", "N/A") ?? "N/A";

            if (profileUrl == "N/A")
            {
                return (null, null);
            }

            var url = "https://observatory.morrolan.ch" + profileUrl;
            doc = await web.LoadFromWebAsync(url);

            return (url, doc);
        }

        public static string GetProfileName(HtmlDocument doc)
        {
            var header = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'page-header')]");
            var headerText = header.InnerText;

            headerText = headerText
                .Remove(headerText.IndexOf("Updated:", StringComparison.Ordinal))
                .Replace("\n", string.Empty);

            return headerText;
        }

        public static async Task<string> GetSteamIdFromDiscordId(ulong discordId)
        {
#if DEBUG
            if (!Directory.Exists("./appdata"))
                return null;
            if (!File.Exists("./appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("./appdata/profileLinks.json", "[]");
                return null;
            }
            var allProfileLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("./appdata/profileLinks.json"));
#else
            if (!Directory.Exists("../appdata"))
            {
                return null;
            }
            if (!File.Exists("../appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("../appdata/profileLinks.json", "[]");
                return null;
            }
            var allProfileLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("../appdata/profileLinks.json"));
#endif

            var profileLink = allProfileLinks.FirstOrDefault(x => x.DiscordID == discordId);

            var steamId = profileLink?.SteamID;

            return steamId;
        }

        public static async Task<ulong?> GetDiscordIdFromSteamId(string steamId)
        {
#if DEBUG
            if (!Directory.Exists("./appdata"))
                return null;
            if (!File.Exists("./appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("./appdata/profileLinks.json", "[]");
                return null;
            }
            var allProfileLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("./appdata/profileLinks.json"));
#else
            if (!Directory.Exists("../appdata"))
            {
                return null;
            }
            if (!File.Exists("../appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("../appdata/profileLinks.json", "[]");
                return null;
            }
            var allProfileLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("../appdata/profileLinks.json"));
#endif
            var profileLink = allProfileLinks.FirstOrDefault(x => x.SteamID == steamId);

            var discordId = profileLink?.DiscordID;

            return discordId;
        }
    }
}
