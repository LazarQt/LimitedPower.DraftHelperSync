using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace LimitedPower.DraftHelperSync
{
    public class Card
    {
        [JsonPropertyName("set")]
        public string Set { get; set; }

        [JsonPropertyName("idArena")]
        public int IdArena { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("imageCardUrl")]
        public string ImageCardUrl { get; set; }
    }

    public class MtgaHelperEvaluation
    {
        [JsonPropertyName("card")]
        public Card Card { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("rating")]
        public int? Rating { get; set; }
    }
}
