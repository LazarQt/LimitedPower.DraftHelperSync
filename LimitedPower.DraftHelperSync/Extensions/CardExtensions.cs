using System.Collections.Generic;
using System.Linq;
using LimitedPower.DraftHelperSync.Model;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class CardExtensions
    {
        /// <summary>
        /// Evaluate the 5 best commons of a single color.
        /// </summary>
        /// <param name="cardRatings">All card ratings</param>
        /// <returns>List of best common cards</returns>
        public static List<SeventeenLandsCard> GetBestCommons(this List<SeventeenLandsCard> cardRatings)
        {
            var bestCommons = new List<SeventeenLandsCard>();
            foreach (var color in Const.Colors)
            {
                var commons = cardRatings.Commons().ExactColor(color);
                bestCommons.AddRange(commons.OrderByWinRate().Take(5)
                    .Union(commons.OrderByImprovementRate().Take(5)));
            }

            return bestCommons;
        }

        public static string CompareTerm(this Card card) => card.Name.ToUpper();
    }
}
