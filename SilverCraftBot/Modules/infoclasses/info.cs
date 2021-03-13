using dotnetcorebot.Modules.infoclasses;
using System.Text.Json.Serialization;

namespace dotnetcorebot.Modules
{
    public class eee
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("motd")]
        public motd Motd { get; set; }

        [JsonPropertyName("players")]
        public Players Players { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("online")]
        public bool Online { get; set; }

        [JsonPropertyName("protocol")]
        public int Protocol { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("software")]
        public string Software { get; set; }
    }
}