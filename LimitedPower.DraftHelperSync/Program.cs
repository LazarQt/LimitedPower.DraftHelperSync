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
            var today = DateTime.Now;
            var last = DateTime.Now.AddDays(daysBack);
            var url =
                $"https://www.17lands.com/card_ratings/data?expansion={set.ToUpper()}" +
                $"&format=PremierDraft&start_date={last.Year}-{last.Month:00}-{last.Day:00}&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
            var doc = new System.Net.WebClient().DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonSerializer.Deserialize<List<SeventeenLandsEvaluation>>(doc);
            if (cardRatings == null) throw new Exception("could not load 17lands data");

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
            var minAvgSeen = cardRatings.Min(c => c.AvgSeen);
            double lowestPick = minAvgSeen ?? default;
            var maxAvgSeen = cardRatings.Max(c => c.AvgSeen);
            double highestPick = maxAvgSeen ?? default;

            // execute requests
            foreach (var cardRating in cardRatings)
            {
                var mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m => m.Card.Name == cardRating.Name);
                if (mtgaHelperCard == null)
                {
                    Console.WriteLine($"can not find {cardRating.Name}");
                    continue;
                }

                var request = new RestRequest(Method.PUT);
                var note = $"WR: {cardRating.EverDrawnWinRate?.ToStringValue(0)}% | IWD: {cardRating.DrawnImprovementWinRate?.ToStringValue(2)}pp";
                request.Generate(@"{idArena:" + mtgaHelperCard.Card.IdArena + ",note:" + "\"" + note + "\"" + ",rating:" + cardRating.AvgSeen?.TransformRating(lowestPick, highestPick) + "}", cookie);
                try
                {
                    client.Execute(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Update not possible for card: {cardRating.Name}. Error: {e.Message}");
                    continue;
                }
                
                Console.WriteLine($"Synced data for: {cardRating.Name}");
            }
        }
    }
}
