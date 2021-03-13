using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace dotnetcorebot.Modules.infoclasses
{
    public class motd
    {
        [JsonPropertyName("raw")]
        public List<string> Raw { get; set; }

        [JsonPropertyName("clean")]
        public List<string> Clean { get; set; }

        [JsonPropertyName("html")]
        public List<string> Html { get; set; }
    }
}