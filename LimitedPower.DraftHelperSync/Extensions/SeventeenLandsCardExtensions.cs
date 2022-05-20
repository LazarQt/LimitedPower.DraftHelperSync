using System.Collections.Generic;
using System.Linq;
using LimitedPower.DraftHelperSync.Model;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class SeventeenLandsCardExtensions
    {
        public static List<SeventeenLandsCard> Commons(this IEnumerable<SeventeenLandsCard> list) =>
            list.Where(c => c.Rarity == Const.Card.Common).ToList();

        public static List<SeventeenLandsCard> OrderByWinRate(this IEnumerable<SeventeenLandsCard> list) =>
            list.OrderByDescending(c => c.EverDrawnWinRate).ToList();

        public static List<SeventeenLandsCard> OrderByImprovementRate(this IEnumerable<SeventeenLandsCard> list) =>
            list.OrderByDescending(c => c.DrawnImprovementWinRate).ToList();

        public static List<SeventeenLandsCard> ExactColor(this List<SeventeenLandsCard> list, string color) =>
            list.Where(l => l.Color.ToLower() == color.ToLower()).ToList();

        public static List<SeventeenLandsCard> SameColors(this List<SeventeenLandsCard> list, string color)
        {
            if (color.Length == 1) return list.Where(x => x.Color.ToLower() == color.ToLower()).ToList();
            var res = new List<SeventeenLandsCard>();
            foreach (var c in color) res.AddRange(list.Where(l => l.Color.ToLower() == c.ToString().ToLower()));
            res.AddRange(list.Where(x => color.ToLower().ToCharArray().SequenceEqual(x.Color.ToLower().ToCharArray())));
            return res;
        }
    }
}
