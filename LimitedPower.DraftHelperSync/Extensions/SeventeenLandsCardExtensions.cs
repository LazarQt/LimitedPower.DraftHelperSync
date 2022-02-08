using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class SeventeenLandsCardExtensions
    {
        public static List<MyCard> Commons(this IEnumerable<MyCard> list) =>
            list.Where(c => c.Rarity == Const.Card.Common).ToList();

        public static List<MyCard> OrderByWinRate(this IEnumerable<MyCard> list) =>
            list.OrderByDescending(c => c.EverDrawnWinRate).ToList();

        public static List<MyCard> OrderByImprovementRate(this IEnumerable<MyCard> list) =>
            list.OrderByDescending(c => c.DrawnImprovementWinRate).ToList();

        public static List<MyCard> ExactColor(this List<MyCard> list, string color) =>
            list.Where(l => l.Color.ToLower() == color.ToLower()).ToList();

        public static List<MyCard> SameColors(this List<MyCard> list, string color)
        {
            if (color.Length == 1) return list.Where(x => x.Color.ToLower() == color.ToLower()).ToList();
            var res = new List<MyCard>();
            foreach (var c in color) res.AddRange(list.Where(l => l.Color.ToLower() == c.ToString().ToLower()));
            res.AddRange(list.Where(x => color.ToLower().ToCharArray().SequenceEqual(x.Color.ToLower().ToCharArray())));
            return res;
        }
    }
}
