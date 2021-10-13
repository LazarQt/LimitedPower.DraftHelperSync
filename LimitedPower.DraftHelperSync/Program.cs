using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LimitedPower.DraftHelperSync.Extensions;
using System.Configuration;

namespace LimitedPower.DraftHelperSync
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("--- Starting Sync (17Lands -> MTGAHelper) ---");

            // Load settings
            var cookie = ConfigurationManager.AppSettings["cookie"];
            if (string.IsNullOrEmpty(cookie)) throw new Exception("No cookie set");
            var set = ConfigurationManager.AppSettings["set"];
            if (set == null) throw new Exception("Configuration does not contain MTG set");
            var daysBack = -5;
            var daysBackSetting = ConfigurationManager.AppSettings["pastdays"];
            if (daysBackSetting != null && int.TryParse(daysBackSetting, out int d)) daysBack = -d;

            // get 17lands stuff
            var premierDraftCardRatings = SeventeenLandsEvaluations(daysBack, set, DraftType.Premier);
            var tradDraftCardRatings = SeventeenLandsEvaluations(daysBack, set, DraftType.Trad);

            // load locally downloaded MTGAHelper stuff 
            var mtgaHelperCards = JsonSerializer.Deserialize<List<MtgaHelperEvaluation>>(System.IO.File.ReadAllText("customDraftRatingsForDisplay.json"));
            if (mtgaHelperCards == null)
            {
                Console.WriteLine("could not read local MTGAHelper data");
                return;
            }

            // setup client
            var client = new RestClient("https://mtgahelper.com/api/User/CustomDraftRating")
            {
                Timeout = -1,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:92.0) Gecko/20100101 Firefox/92.0"
            };

            // Math mumbo jumbo, don't @ me
            var minAvgSeen = premierDraftCardRatings.Min(c => c.AvgSeen);
            double lowestPick = minAvgSeen ?? default;
            var maxAvgSeen = premierDraftCardRatings.Max(c => c.AvgSeen);
            double highestPick = maxAvgSeen ?? default;

            // execute requests
            foreach (var premierRating in premierDraftCardRatings)
            {
                var mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m =>
                    m.Card.Name == premierRating.Name && m.Card.Set.ToUpper() == set.ToUpper());
                if (mtgaHelperCard == null)
                {
                    Console.WriteLine($"can not find {premierRating.Name}");
                    continue;
                }

                var request = new RestRequest(Method.PUT);
                var premierNote = $"WR: {premierRating.EverDrawnWinRate?.ToStringValue(0)}% | IWD: {premierRating.DrawnImprovementWinRate?.ToStringValue(2)}pp";

                var tradRating = tradDraftCardRatings.FirstOrDefault(f => f.Name == premierRating.Name);
                if (tradRating == null) throw new Exception($"No traditional rating found for {premierRating.Name}");
                var tradNote = $"WR: {tradRating.EverDrawnWinRate?.ToStringValue(0)}% | IWD: {tradRating.DrawnImprovementWinRate?.ToStringValue(2)}pp";

                var note = $"Premier[{premierNote}] Trad[{tradNote}]";

                request.Generate(@"{idArena:" + mtgaHelperCard.Card.IdArena + ",note:" + "\"" + note + "\"" + ",rating:" + premierRating.AvgSeen?.TransformRating(lowestPick, highestPick) + "}", cookie);
                try
                {
                    client.Execute(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Update not possible for card: {premierRating.Name}. Error: {e.Message}");
                    continue;
                }

                Console.WriteLine($"Synced data for: {premierRating.Name}");
            }
        }

        private static List<SeventeenLandsEvaluation> SeventeenLandsEvaluations(int daysBack, string set, DraftType draft)
        {
            var today = DateTime.Now;
            var last = DateTime.Now.AddDays(daysBack);
            var url =
                $"https://www.17lands.com/card_ratings/data?expansion={set.ToUpper()}" +
                $"&format={Enum.GetName(draft)}Draft&start_date={last.Year}-{last.Month:00}-{last.Day:00}&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
            var doc = new System.Net.WebClient().DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonSerializer.Deserialize<List<SeventeenLandsEvaluation>>(doc);
            if (cardRatings == null) throw new Exception("could not load 17lands data");
            return cardRatings;
        }
    }
}
