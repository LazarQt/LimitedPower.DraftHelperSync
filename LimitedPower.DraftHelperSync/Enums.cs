using System.Collections.Generic;

namespace LimitedPower.DraftHelperSync
{
    enum DraftType
    {
        // ReSharper disable once UnusedMember.Global
        PremierDraft,
        TradDraft,
        Sealed
        // ReSharper restore UnusedMember.Global
    }

    enum TimespanType
    {
        PastDays,
        StartDate
    }

    public enum ReviewContributor
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable IdentifierTypo
        DraftaholicsAnonymous,
        DraftSim,
        Lolaman,
        Ham,
        Scottynada,
        Deathsie,
        SeventeenLands,
        // ReSharper restore InconsistentNaming
        // ReSharper restore IdentifierTypo
    }

    public class LimitedPowerRating
    {
        public double Rating { get; set; }
        public string Description { get; set; }
        public ReviewContributor ReviewContributor { get; set; }

        public LimitedPowerRating(double rating, string description, ReviewContributor reviewContributor)
        {
            Rating = rating;
            Description = description;
            ReviewContributor = reviewContributor;
        }
    }

    public class MyCard : SeventeenLandsCard
    {
        public List<LimitedPowerRating> Ratings { get; set; }
    }
}
