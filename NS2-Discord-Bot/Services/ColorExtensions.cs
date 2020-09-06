using System;
using System.Collections.Generic;

namespace NS2_Discord_Bot.Services
{
    public class Color
    {
        public Discord.Color GetRandomColor()
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
    }
}
