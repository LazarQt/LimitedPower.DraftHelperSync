using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class CardExtensions
    {
        public static List<MyCard> PrintBestCommons(this List<MyCard> cardRatings)
        {
            var bestCommons = new List<MyCard>();
            foreach (var color in Const.Colors)
            {
                var commons = cardRatings.Commons().ExactColor(color);
                bestCommons.AddRange(commons.OrderByWinRate().Take(5)
                    .Union(commons.OrderByImprovementRate().Take(5)));
            }


            foreach (var c in bestCommons)
            {
                File.AppendAllText(@"commons.html", $"<img src=\"neo/{c.Name}.jpg\">");
            }

            return bestCommons;
        }

    }
}
