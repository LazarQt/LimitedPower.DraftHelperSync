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
                File.AppendAllText(@"commons.html", $"<img src=\"snc/{c.Name}.jpg\">");
            }

            return bestCommons;
        }

        public static List<Core.Card> PrintBestCommons(this List<Core.Card> cardRatings)
        {
            var bestCommons = new List<Core.Card>();
            var sorted = cardRatings.OrderByDescending(c => c.CondensedRating).Where(x => x.Rarity == "common");
            foreach (var color in Const.Colors)
            {
                if (color == "") continue;
                var x = 0;
                foreach (var s in sorted)
                {
                    if (x >= 5) break;
                    if (s.ColorIdentity.Count == 1 && s.ColorIdentity[0] == color)
                    {
                        bestCommons.Add(s);
                        x++;
                    }
                }
            }

            var u = 0;
            foreach (var c in bestCommons)
            {

                var cName = c.Name;
                if (cName.Contains("//"))
                {
                    cName = c.Name.Substring(0, cName.IndexOf("//")-1);
                }
                File.AppendAllText(@"commons.html", $"<img src=\"snc/{cName}.jpg\">");
                u++;
                if (u >= 5)
                {
                    File.AppendAllText(@"commons.html", $"<hr>");
                    u = 0;
                }
            }

            return bestCommons;
        }

        public static List<Core.Card> PrintBest(this List<Core.Card> cardRatings)
        {
            var bestCommons = new List<Core.Card>();
            var sorted = cardRatings.OrderByDescending(c => c.CondensedRating);

            foreach (var c in sorted)
            {

                var cName = c.Name;
                if (cName.Contains("//"))
                {
                    cName = c.Name.Substring(0, cName.IndexOf("//") - 1);
                }
                File.AppendAllText(@"best.html", $"<img src=\"snc/{cName}.jpg\">");
            }

            return bestCommons;
        }

        public static List<Core.Card> PrintBest(this List<MyCard> cardRatings)
        {
            var bestCommons = new List<Core.Card>();
            var sorted = cardRatings.OrderByDescending(c => c.EverDrawnWinRate);

            foreach (var c in sorted)
            {

                var cName = c.Name;
                if (cName.Contains("//"))
                {
                    cName = c.Name.Substring(0, cName.IndexOf("//") - 1);
                }
                File.AppendAllText(@"best.html", $"<img src=\"snc/{cName}.jpg\">");
            }

            return bestCommons;
        }

    }
}
