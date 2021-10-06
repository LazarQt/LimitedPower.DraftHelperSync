using RestSharp;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class RestExtensions
    {
        public static void Generate(this RestRequest request, string body, string cookie)
        {
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Language", "en-US,en;q=0.5");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Origin", "https://mtgahelper.com");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Referer", "https://mtgahelper.com/my/draftRatings");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
        }
    }
}
