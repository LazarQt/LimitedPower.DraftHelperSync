namespace LimitedPower.DraftHelperSync
{
    public static class Const
    {
        public static string[] Colors = { "W", "U", "B", "R", "G", "" };

        public static class Settings
        {
            public const string TimespanType = "timespantype";
            public const string TimespanValue = "timespanvalue";
            public const string DraftType = "drafttype";
            public const string Set = "set";
            public const string Cookie = "cookie";
            public const string RatingType = "ratingtype";
            public const string AvgPick = "AvgPick";
            public const string AbsoluteWin = "AbsoluteWin";
            public const string RelativeWin = "RelativeWin";
        }

        public static class MtgaHelper
        {
            public const string CardsJson = "customDraftRatingsForDisplay.json";
            public const string Url = "https://mtgahelper.com/api/User/CustomDraftRating";
            public const int Timeout = -1;
            public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:92.0) Gecko/20100101 Firefox/92.0";
        }

        public static class Card
        {
            public const string Common = "common";
        }
    }
}
