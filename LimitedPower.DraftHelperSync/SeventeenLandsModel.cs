using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace LimitedPower.DraftHelperSync
{
    public class SeventeenLandsEvaluation
    {
        [JsonPropertyName("seen_count")]
        public int SeenCount { get; set; }

        [JsonPropertyName("avg_seen")]
        public double? AvgSeen { get; set; }

        [JsonPropertyName("pick_count")]
        public int PickCount { get; set; }

        [JsonPropertyName("avg_pick")]
        public double? AvgPick { get; set; }

        [JsonPropertyName("game_count")]
        public int GameCount { get; set; }

        [JsonPropertyName("win_rate")]
        public double? WinRate { get; set; }

        [JsonPropertyName("sideboard_game_count")]
        public int SideboardGameCount { get; set; }

        [JsonPropertyName("sideboard_win_rate")]
        public double? SideboardWinRate { get; set; }

        [JsonPropertyName("opening_hand_game_count")]
        public int OpeningHandGameCount { get; set; }

        [JsonPropertyName("opening_hand_win_rate")]
        public double? OpeningHandWinRate { get; set; }

        [JsonPropertyName("drawn_game_count")]
        public int DrawnGameCount { get; set; }

        [JsonPropertyName("drawn_win_rate")]
        public double? DrawnWinRate { get; set; }

        [JsonPropertyName("ever_drawn_game_count")]
        public int EverDrawnGameCount { get; set; }

        [JsonPropertyName("ever_drawn_win_rate")]
        public double? EverDrawnWinRate { get; set; }

        [JsonPropertyName("never_drawn_game_count")]
        public int NeverDrawnGameCount { get; set; }

        [JsonPropertyName("never_drawn_win_rate")]
        public double? NeverDrawnWinRate { get; set; }

        [JsonPropertyName("drawn_improvement_win_rate")]
        public double? DrawnImprovementWinRate { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("rarity")]
        public string Rarity { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("url_back")]
        public string UrlBack { get; set; }
    }
}
