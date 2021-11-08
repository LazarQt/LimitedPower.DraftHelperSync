using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LimitedPower.DraftHelperSync.Extensions;
using System.Configuration;
using System.IO;

namespace LimitedPower.DraftHelperSync
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("--- Starting Sync (17Lands -> MTGAHelper) ---");

            // Load settings
            var cookie = ConfigurationManager.AppSettings[Const.Settings.Cookie];
            if (string.IsNullOrEmpty(cookie)) throw new Exception("No cookie set");
            var set = ConfigurationManager.AppSettings[Const.Settings.Set];
            if (set == null) throw new Exception("Configuration does not contain MTG set");
            var timespanType = ConfigurationManager.AppSettings[Const.Settings.TimespanType];
            var date = DateTime.Now;
            if (Enum.TryParse<TimespanType>(timespanType, out var res))
            {
                var timespanValue = ConfigurationManager.AppSettings[Const.Settings.TimespanValue];
                date = res switch
                {
                    TimespanType.PastDays => date.AddDays(-Convert.ToInt32(timespanValue)),
                    TimespanType.StartDate => DateTime.Parse(timespanValue),
                    _ => throw new Exception("Invalid timespan value"),
                };
            }
            else
            {
                throw new Exception("Can't parse timespan type");
            }

            DraftType draftType;
            if (Enum.TryParse<DraftType>(ConfigurationManager.AppSettings[Const.Settings.DraftType], out var t))
            {
                draftType = t;
            }
            else
            {
                throw new Exception("Can't parse draft type");
            }

            // get 17lands stuff
            var cardRatings = GetSeventeenLandsEvaluations(date, set, draftType);

            // load locally downloaded MTGAHelper stuff 
            var mtgaHelperCards =
                JsonSerializer.Deserialize<List<MtgaHelperEvaluation>>(File.ReadAllText(Const.MtgaHelper.CardsJson));
            if (mtgaHelperCards == null) throw new Exception("could not read local MTGAHelper data");

            // setup client
            var client = new RestClient(Const.MtgaHelper.Url)
            {
                Timeout = Const.MtgaHelper.Timeout,
                UserAgent = Const.MtgaHelper.UserAgent
            };

            // cards that are played less than 1% of the time are ignored
            var minPlay = cardRatings.Max(c => c.GameCount) * .01;

            // evaluate best commons
            var bestCommons = new List<SeventeenLandsCard>();
            foreach (var color in Const.Colors)
            {
                var commons = cardRatings.Commons().ExactColor(color);
                bestCommons.AddRange(commons.OrderByWinRate().Take(5).Union(commons.OrderByImprovementRate().Take(5)));
            }

            foreach (var card in cardRatings)
            {
                var relatedRatings = cardRatings.SameColors(card.Color);
                var max = relatedRatings.Max(g => g.EverDrawnWinRate);
                var min = relatedRatings.Min(g => g.EverDrawnWinRate);
                if (!max.HasValue || !min.HasValue) throw new Exception("ratings not complete (sample size low?)");

                var mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m =>
                    m.Card.Name == card.Name && m.Card.Set.ToUpper() == set.ToUpper());
                if (mtgaHelperCard == null)
                {
                    Console.WriteLine($"can not find {card.Name}");
                    continue;
                }

                var request = new RestRequest(Method.PUT);
                var note = $"WR: {card.EverDrawnWinRate?.ToStringValue(0)}% | IWD: {card.DrawnImprovementWinRate?.ToStringValue(2)}pp";

                // if card belongs to one of the best commons, add a star emoticon to indicate that
                if (bestCommons.Any(c => c.Name == card.Name)) note += "⭐";

                var pickPosition = card.EverDrawnWinRate?.TransformRating(max.Value, min.Value);

                // low sample size
                if (card.GameCount <= minPlay) pickPosition = 0;

                request.Generate(@"{idArena:" + mtgaHelperCard.Card.IdArena + ",note:" + "\"" + note + "\"" + ",rating:" + pickPosition + "}", cookie);
                try
                {
                    client.Execute(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Update not possible for card: {card.Name}. Error: {e.Message}");
                    continue;
                }

                Console.WriteLine($"Synced data for: {card.Name}");
            }
        }

        private static List<SeventeenLandsCard> GetSeventeenLandsEvaluations(DateTime start, string set, DraftType draft)
        {
            var today = DateTime.Now;
            var url =
                $"https://www.17lands.com/card_ratings/data?expansion={set.ToUpper()}" +
                $"&format={draft.GetType().GetEnumName(draft)}&start_date={start.Year}-{start.Month:00}-{start.Day:00}&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
            var doc = new System.Net.WebClient().DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonSerializer.Deserialize<List<SeventeenLandsCard>>(doc);
            if (cardRatings == null) throw new Exception("could not load 17lands data");
            return cardRatings;
        }
    }
}
