using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class MtgaHelperCardExtensions
    {
        public static MtgaHelperEvaluation GetCard(this List<MtgaHelperEvaluation> mtgaHelperCards, string cardName, string set)
        {
            var mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m =>
                m.Card.Name == cardName && m.Card.Set.ToUpper() == set.ToUpper());

            // alchemy cards fix
            if(mtgaHelperCard == null)
            {
                mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m =>
                m.Card.Name == cardName.Replace("A-","") && m.Card.Set.ToUpper() == set.ToUpper());
            }

            if (mtgaHelperCard == null)
            {
                var n = cardName.Substring(0, cardName.IndexOf("//") - 1);
                mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m =>
                    m.Card.Name == n && m.Card.Set.ToUpper() == set.ToUpper());
                if (n == null)
                {
                    throw new Exception($"can not find {cardName}");
                }
                
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
