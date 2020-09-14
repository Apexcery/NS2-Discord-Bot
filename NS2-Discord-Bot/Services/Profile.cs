using System;
using System.Threading.Tasks;
using HtmlAgilityPack;

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

            await UpdateProfile(doc);

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

        public static async Task UpdateProfile(HtmlDocument doc)
        {
            var header = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'page-header')]")?.InnerText;
            
            if (string.IsNullOrEmpty(header)) return;

            var lastUpdatedText = header
                .Substring(header.IndexOf("Updated:", StringComparison.Ordinal))
                .Replace("\n", string.Empty);

            if (lastUpdatedText.Contains("minute")) return;

            var updateButton = doc.DocumentNode.SelectSingleNode("//input[contains(@value, 'Update')]");

            

            return;
        }
    }
}
