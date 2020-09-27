using System;
using System.Collections.Generic;
using Random = System.Random;

namespace NS2_Discord_Bot.Services
{
    public class Color
    {
        public static Discord.Color GetRandomDiscordColor()
        {
            var allColors = new List<Discord.Color>
            {
                Discord.Color.Blue,
                Discord.Color.DarkBlue,
                Discord.Color.DarkGreen,
                Discord.Color.DarkGrey,
                Discord.Color.DarkMagenta,
                Discord.Color.DarkOrange,
                Discord.Color.DarkPurple,
                Discord.Color.DarkRed,
                Discord.Color.DarkTeal,
                Discord.Color.DarkerGrey,
                Discord.Color.Default,
                Discord.Color.Gold,
                Discord.Color.Green,
                Discord.Color.LightGrey,
                Discord.Color.LightOrange,
                Discord.Color.Magenta,
                Discord.Color.Orange,
                Discord.Color.Purple,
                Discord.Color.Red,
                Discord.Color.Teal
            };

            var randomColor = allColors[new Random().Next(0, allColors.Count - 1)];

            return randomColor;
        }

        public static System.Drawing.Color GetRandomDrawingColor(bool onlyLightColors = false)
        {
            const int lightColorThreshold = 150;
            const int minColorThreshold = 0;
            const int maxColorThreshold = 255;

            var randomColor = System.Drawing.Color.FromArgb(
                new Random().Next(onlyLightColors ? lightColorThreshold : minColorThreshold, maxColorThreshold),
                new Random().Next(onlyLightColors ? lightColorThreshold : minColorThreshold, maxColorThreshold),
                new Random().Next(onlyLightColors ? lightColorThreshold : minColorThreshold, maxColorThreshold));

            return randomColor;
        }

        public static System.Drawing.Color GetRandomPastelDrawingColor()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            var r = (int) (Math.Round(random.NextDouble() * 127) + 127);
            var g = (int) (Math.Round(random.NextDouble() * 127) + 127);
            var b = (int) (Math.Round(random.NextDouble() * 127) + 127);

            var color = System.Drawing.Color.FromArgb(r, g, b);
            return color;
        }
    }
}
