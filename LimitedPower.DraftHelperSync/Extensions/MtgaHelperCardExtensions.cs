using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class MtgaHelperCardExtensions
    {
        public static MtgaHelperEvaluation GetCard(this List<MtgaHelperEvaluation> mtgaHelperCards, MyCard card, string set)
        {
            var mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m =>
                m.Card.Name == card.Name && m.Card.Set.ToUpper() == set.ToUpper());
            if (mtgaHelperCard == null)
            {
                throw new Exception($"can not find {card.Name}");
            }

            return mtgaHelperCard;
        }

        public static string RequestBody(this MtgaHelperEvaluation mtgaHelperCard, string note, int pickPosition)
        {
            return @"{idArena:" + mtgaHelperCard.Card.IdArena + ",note:" + "\"" + note + "\"" + ",rating:" +
                   pickPosition + "}";
        }
    }
}
