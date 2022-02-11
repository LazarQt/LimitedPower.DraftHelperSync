using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using LimitedPower.DraftHelperSync.Extensions;
using System.Configuration;
using System.IO;
using LimitedPower.Tool;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

            // general
            // setup client
            var client = new RestClient(Const.MtgaHelper.Url)
            {
                Timeout = Const.MtgaHelper.Timeout,
                UserAgent = Const.MtgaHelper.UserAgent
            };
            // load locally downloaded MTGAHelper stuff 
            var mtgaHelperCards =
                JsonSerializer.Deserialize<List<MtgaHelperEvaluation>>(
                    File.ReadAllText(Const.MtgaHelper.CardsJson));
            if (mtgaHelperCards == null) throw new Exception("could not read local MTGAHelper data");


            if (ConfigurationManager.AppSettings[Const.Settings.Source] == Const.App.ContentCreatorOpinion)
            {
                var tool = new LpTool();
                // ReSharper disable StringLiteralTypo
                tool.Run($"loadcards {set.ToLower()}");
                tool.Run($"loadratings {set.ToLower()}");
                // ReSharper restore StringLiteralTypo

                var cardRatings = JsonConvert.DeserializeObject<List<MyCard>>(File.ReadAllText(@$"{set}.json"));
                if (cardRatings == null) throw new Exception("no ratings found");

                var cardRatings2 = JsonConvert.DeserializeObject<List<Core.Card>>(File.ReadAllText(@$"{set}.json"));

                var condensedRatings = cardRatings2.Select(a =>
                {
                    a.CondensedRating = a.Ratings.Average(u => u.Rating);
                    return a;
                }).ToList();
                if (condensedRatings == null) throw new Exception("Cant load ratings");

                //File.WriteAllText("commons.html",string.Empty);
                foreach (var x in condensedRatings.OrderByDescending(a => a.CondensedRating))
                {
                    var name = x.Name;
                    if (name.Contains("//")) name = name.Substring(0, name.IndexOf("//")-1);
                    //File.AppendAllText(@"commons.html", $"<img src=\"neo/{name}.jpg\"> {Math.Round(x.CondensedRating,2)}");
                }

                foreach (var card in condensedRatings)
                {
                    
                    //var bc  = condensedRatings.PrintBestCommons();

                    var request = new RestRequest(Method.PUT);

                    var mtgaHelperCard = mtgaHelperCards.GetCard(card.Name, set);

                    var pickPosition = card.CondensedRating.TransformRating(
                        condensedRatings.Max(a => a.CondensedRating), condensedRatings.Min(a => a.CondensedRating));
                    if (mtgaHelperCard == null)
                    {
                        Console.WriteLine("NOT FOUND: " + card.Name);
                        continue;
                    }
                    request.Generate(mtgaHelperCard.RequestBody(string.Empty, pickPosition), cookie);
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
            else
            {

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


                // cards that are played less than 1% of the time are ignored
                var minPlay = cardRatings.Max(c => c.GameCount) * .01;

                // evaluate best commons
                var bestCommons = cardRatings.PrintBestCommons();

                var mostPlayed = cardRatings.Max(x => x.GameCount);
                var leastGamesRequired = mostPlayed * .0225;

                cardRatings = cardRatings.Where(c => c.GameCount >= leastGamesRequired).OrderByWinRate();

                foreach (var card in cardRatings)
                {
                    //if (!card.Name.Contains("Evolving")) continue;
                    var ratingType = ConfigurationManager.AppSettings[Const.Settings.RatingType];
                    List<MyCard> relatedRatings;
                    double? max;
                    double? min;
                    switch (ratingType)
                    {
                        case Const.Settings.AbsoluteWin:
                            relatedRatings = cardRatings;
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
                    if (card.GameCount <= minPlay) pickPosition = 0;
                    var pickPos = 0;
                    if (pickPosition != null) pickPos = (int)pickPosition;

                    request.Generate(mtgaHelperCard.RequestBody(note, pickPos), cookie);
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
        }

        private static List<MyCard> GetSeventeenLandsEvaluations(DateTime start, string set, DraftType draft)
        {
            var today = DateTime.Now;
            var url =
                $"https://www.17lands.com/card_ratings/data?expansion={set.ToUpper()}" +
                $"&format={draft.GetType().GetEnumName(draft)}&start_date={start.Year}-{start.Month:00}-{start.Day:00}&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
            var doc = new System.Net.WebClient().DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonSerializer.Deserialize<List<MyCard>>(doc);
            if (cardRatings == null) throw new Exception("could not load 17lands data");
            return cardRatings;
        }
    }
}
