using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using LimitedPower.DraftHelperSync.Extensions;
using System.Configuration;
using System.IO;
using System.Net;
using LimitedPower.DraftHelperSync.Model;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LimitedPower.DraftHelperSync
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("--- Starting Sync (17Lands -> MTGAHelper) ---");

            Console.WriteLine("--- Read User Configuration ............. ---");

            // Load settings
            var cookie = ConfigurationManager.AppSettings[Const.Settings.Cookie];
            if (string.IsNullOrEmpty(cookie)) throw new Exception("No cookie set");
            var set = ConfigurationManager.AppSettings[Const.Settings.Set];
            if (set == null) throw new Exception("Configuration does not contain MTG set");

            // setup rest client for updating ratings
            var restClient = new RestClient(Const.MtgaHelper.Url)
            {
                Timeout = Const.MtgaHelper.Timeout,
                UserAgent = Const.MtgaHelper.UserAgent
            };

            Console.WriteLine("--- Download MTGAHelper Files ........... ---");

            // delete old file if exists
            if (File.Exists(Const.MtgaHelper.CardsJsonFile)) File.Delete(@Const.MtgaHelper.CardsJsonFile);

            // download card mapping
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(Const.MtgaHelper.CardsJsonUrl, Const.MtgaHelper.CardsJsonFile);
            }

            // read mapping file
            var mtgaHelperCards =
                JsonSerializer.Deserialize<List<MtgaHelperEvaluation>>(
                    File.ReadAllText(Const.MtgaHelper.CardsJsonFile));
            if (mtgaHelperCards == null) throw new Exception("could not read local MTGAHelper data");

            // parse timespan type
            var timespanType = ConfigurationManager.AppSettings[Const.Settings.TimespanType];
            var date = DateTime.Now;
            if (Enum.TryParse<TimespanType>(timespanType, out var res))
            {
                var timespanValue = ConfigurationManager.AppSettings[Const.Settings.TimespanValue];
                if (timespanValue == null) throw new Exception("TimeSpan not set in user configuration");

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

            // get 17lands data
            var cardRatings = GetSeventeenLandsEvaluations(date, set, draftType);

            // cards that are played less than 1% of the time are ignored
            var minPlay = cardRatings.Max(c => c.GameCount) * .01;

            // evaluate best commons
            var bestCommons = cardRatings.GetBestCommons();

            cardRatings = cardRatings.OrderByWinRate();

            foreach (var card in cardRatings)
            {
                var ratingType = ConfigurationManager.AppSettings[Const.Settings.RatingType];
                List<SeventeenLandsCard> relatedRatings;
                double? max;
                double? min;

                // eliminate cards with win rate = 100%, this means no data but shows up as 100% winrate
                cardRatings = cardRatings.Where(c => c.EverDrawnWinRate < 1).ToList();

                switch (ratingType)
                {
                    case Const.Settings.AbsoluteWin:
                        relatedRatings = cardRatings.ToList();
                        max = relatedRatings.Max(g => g.EverDrawnWinRate);
                        min = relatedRatings.Min(g => g.EverDrawnWinRate);
                        break;
                    case Const.Settings.RelativeWin:
                        relatedRatings = cardRatings.SameColors(card.Color);
                        max = relatedRatings.Max(g => g.EverDrawnWinRate);
                        min = relatedRatings.Min(g => g.EverDrawnWinRate);
                        break;
                    default:
                        relatedRatings = cardRatings;
                        max = relatedRatings.Min(g => g.AvgPick);
                        min = relatedRatings.Max(g => g.AvgPick);
                        break;
                }



                if (!max.HasValue || !min.HasValue) throw new Exception("ratings not complete (sample size low?)");

                var mtgaHelperCard = mtgaHelperCards.GetCard(card.Name, set);

                var request = new RestRequest(Method.PUT);
                var note =
                    $"WR: {card.EverDrawnWinRate?.ToStringValue(0)}% | IWD: {card.DrawnImprovementWinRate?.ToStringValue(2)}pp";

                // if card belongs to one of the best commons, add a star emoticon to indicate that
                if (bestCommons.Any(c => c.Name == card.Name)) note += "⭐";

                int? pickPosition = ratingType switch
                {
                    Const.Settings.AbsoluteWin => card.EverDrawnWinRate?.TransformRating(max.Value, min.Value),
                    Const.Settings.RelativeWin => card.EverDrawnWinRate?.TransformRating(max.Value, min.Value),
                    _ => card.AvgPick?.TransformRating(max.Value, min.Value)
                };

                // low sample size
                if (card.EverDrawnWinRate == 1) pickPosition = 0;
                var pickPos = 0;
                if (pickPosition != null) pickPos = (int)pickPosition;

                request.Generate(mtgaHelperCard.RequestBody(note, pickPos), cookie);
                try
                {
                    restClient.Execute(request);
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
            var doc = new WebClient().DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonSerializer.Deserialize<List<SeventeenLandsCard>>(doc);
            if (cardRatings == null) throw new Exception("could not load 17lands data");
            return cardRatings;
        }
    }
}
