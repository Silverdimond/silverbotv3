using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace dotnetcorebot.Modules.infoclasses
{
    public class plugins
    {
        [JsonPropertyName("names")]
        public List<string> Names { get; set; }

        [JsonPropertyName("raw")]
        public List<string> Raw { get; set; }
    }
}