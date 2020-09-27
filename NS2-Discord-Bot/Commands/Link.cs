using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using NS2_Discord_Bot.Models;
using NS2_Discord_Bot.Services;
using Color = NS2_Discord_Bot.Services.Color;

namespace NS2_Discord_Bot.Commands
{
    public class Link : ModuleBase<SocketCommandContext>
    {
        [Command("link")]
        public async Task LinkProfile(string steamId = null)
        {
#if DEBUG
            if (!Directory.Exists("./appdata"))
            {
                await ReplyAsync("'appdata' directory does not exist.");
                return;
            }
            if (!File.Exists("./appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("./appdata/profileLinks.json", "[]");
            }
            var allLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("./appdata/profileLinks.json"));
#else
            if (!Directory.Exists("../appdata"))
            {
                await ReplyAsync("'appdata' directory does not exist.");
                return;
            }
            if (!File.Exists("../appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("../appdata/profileLinks.json", "[]");
            }
            var allLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("../appdata/profileLinks.json"));
#endif

            var currentLink = allLinks.FirstOrDefault(x => x.DiscordID == Context.User.Id);

            if (string.IsNullOrEmpty(steamId))
            {
                if (currentLink == null)
                {
                    await ReplyAsync("You have not yet linked your profile");
                    return;
                }

                var currentLinkEmbed = new EmbedBuilder()
                    .WithTitle($"Linked To: {currentLink.ObservatoryProfileName}")
                    .WithUrl(currentLink.ObservatoryProfileUrl)
                    .WithColor(Color.GetRandomDiscordColor())
                    .Build();

                await Context.Channel.SendMessageAsync("", false, currentLinkEmbed);

                return;
            }

            if (currentLink != null)
                allLinks.Remove(currentLink);

            var (profileUrl, profilePage) = await Profile.GetProfilePage(steamId);

            if (profilePage == null || string.IsNullOrEmpty(profileUrl))
            {
                await ReplyAsync("Could not find a profile with that Steam ID");
                return;
            }

            var profileName = Profile.GetProfileName(profilePage);

            var newProfileLink = new ProfileLink
            {
                SteamID = steamId,
                DiscordID = Context.User.Id,
                ObservatoryProfileName = profileName,
                ObservatoryProfileUrl = profileUrl
            };

            allLinks.Add(newProfileLink);
#if DEBUG
            await File.WriteAllTextAsync("./appdata/profileLinks.json" ,JsonConvert.SerializeObject(allLinks, Formatting.Indented));
#else
            await File.WriteAllTextAsync("../appdata/profileLinks.json" ,JsonConvert.SerializeObject(allLinks, Formatting.Indented));
#endif

            var embed = new EmbedBuilder()
                .WithTitle("Successfully Linked")
                .WithUrl(profileUrl)
                .WithDescription($"You have successfully linked your profile to '{profileName}'")
                .WithColor(Color.GetRandomDiscordColor())
                .Build();

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("unlink")]
        public async Task UnLinkProfile()
        {
#if DEBUG
            if (!Directory.Exists("./appdata"))
            {
                await ReplyAsync("'appdata' directory does not exist.");
                return;
            }
            if (!File.Exists("./appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("./appdata/profileLinks.json", "[]");
            }
            var allLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("./appdata/profileLinks.json"));
#else
            if (!Directory.Exists("../appdata"))
            {
                await ReplyAsync("'appdata' directory does not exist.");
                return;
            }
            if (!File.Exists("../appdata/profileLinks.json"))
            {
                await File.WriteAllTextAsync("../appdata/profileLinks.json", "[]");
            }
            var allLinks = JsonConvert.DeserializeObject<List<ProfileLink>>(await File.ReadAllTextAsync("../appdata/profileLinks.json"));
#endif
            
            if (allLinks.Any(x => x.DiscordID == Context.User.Id))
            {
                allLinks.Remove(allLinks.First(x => x.DiscordID == Context.User.Id));

#if DEBUG
                await File.WriteAllTextAsync("./appdata/profileLinks.json", JsonConvert.SerializeObject(allLinks, Formatting.Indented));
#else
                await File.WriteAllTextAsync("../appdata/profileLinks.json" ,JsonConvert.SerializeObject(allLinks, Formatting.Indented));
#endif

                await ReplyAsync("Link successfully removed.");
            }
            else
            {
                await ReplyAsync("You are not currently linked to a profile.");
            }
        }
    }
}
