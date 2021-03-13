using System.Net.Http;

namespace SIlverCraftBot.Modules
{
    public static class Webclient
    {
        private static readonly HttpClient HttpClient = NewhttpClientwithstrign();

        public static HttpClient NewhttpClientwithstrign()
        {
            HttpClient e = new HttpClient();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd($"SilverBot/v{Version.vnumber} (+{ThisAssembly.Git.RepositoryUrl},v{Version.vnumber})");
            return e;
        }

        public static HttpClient Get()
        {
            return HttpClient;
        }
    }
}