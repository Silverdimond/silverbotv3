using System.Text.Json.Serialization;

namespace dotnetcorebot.Modules.infoclasses
{
    public class Players
    {
        [JsonPropertyName("online")]
        public int Online { get; set; }

        [JsonPropertyName("max")]
        public int Max { get; set; }
    }
}